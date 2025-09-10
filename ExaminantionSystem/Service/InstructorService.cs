
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Dtos.Student;
using ExaminantionSystem.Entities.Enums;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Shared;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure.Data;
using ExaminantionSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Service
{
    public class InstructorService
    {

        private readonly CourseRepository _courseRepository;
        private readonly StudentRepository _studentRepository;
        private readonly Repository<StudentCourse> _studentCourseRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly Repository<ExamQuestion> _examQuestionRepository;
        private readonly ExamRepository _examRepository;
        private readonly Repository<ExamResult> _examResultRepository;
        private readonly ChoiceRepository _choiceRepository;
        private readonly IMapper _mapper;

        // private readonly InstructorRepository _instructorRepository;

        public InstructorService(ChoiceRepository choiceRepository,
            Repository<StudentCourse> studentCourseRepository,
            Repository<ExamResult> examResultRepository,
            CourseRepository courseRepository,
            StudentRepository studentRepository,
            Repository<ExamQuestion> examQuestionRepository,
            ExamRepository examRepository,
            QuestionRepository questionRepository,
            IMapper mapper)
        {
            _studentCourseRepository = studentCourseRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _examQuestionRepository = examQuestionRepository;
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _examResultRepository = examResultRepository;
            _choiceRepository = choiceRepository;
            _mapper = mapper;
            // _instructorRepository = instructorRepository;
        }

        #region ManageInstructorEnrollments
        public async Task<Response<StudentCourseDto>> ApproveEnrollmentAsync(int enrollmentId, int currenUserId)
        {
            var enrollment = await _studentCourseRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null)
                return Response<StudentCourseDto>.Fail(ErrorType.ENROLLMENT_NOT_FOUND,
                    new ErrorDetail("Enrollment request not found"));

            // Check if instructor owns the course
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId);
            if (course.InstructorId != currenUserId)
                return Response<StudentCourseDto>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only manage enrollments for your own courses"));

            // Only pending requests can be approved
            if (enrollment.Status != RequestStatus.Pending)
                return Response<StudentCourseDto>.Fail(ErrorType.INVALID_ENROLLMENT_STATUS,
                    new ErrorDetail("Only pending requests can be approved"));

            enrollment.Status = RequestStatus.Approved;
            enrollment.EnrollmentDate = DateTime.UtcNow;
            enrollment.IsActive = true;
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _studentCourseRepository.UpdateAsync(enrollment);
            await _studentCourseRepository.SaveChangesAsync();


            var result = await _studentCourseRepository.GetAll(e => e.Id == enrollmentId)
            .ProjectTo<StudentCourseDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
            return Response<StudentCourseDto>.Success(result);
        }

        public async Task<Response<StudentCourseDto>> RejectEnrollmentAsync(int enrollmentId, int currentuserId)
        {

            var enrollment = await _studentCourseRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null)
                return Response<StudentCourseDto>.Fail(ErrorType.ENROLLMENT_NOT_FOUND,
                    new ErrorDetail( "Enrollment request not found"));

            // Check if instructor owns the course
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId);
            if (course.InstructorId != currentuserId)
                return Response<StudentCourseDto>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only manage enrollments for your own courses"));

            // Only pending requests can be rejected
            if (enrollment.Status != RequestStatus.Pending)
                return Response<StudentCourseDto>.Fail(ErrorType.INVALID_ENROLLMENT_STATUS,
                    new ErrorDetail( "Only pending requests can be rejected"));

            enrollment.Status = RequestStatus.Rejected;
            enrollment.EnrollmentDate = null; // Clear enrollment date for rejected requests
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _studentCourseRepository.UpdateAsync(enrollment);
            await _studentCourseRepository.SaveChangesAsync();

            //Mapping
            var result = await _studentCourseRepository.GetAll(e => e.Id == enrollmentId)
             .ProjectTo<StudentCourseDto>(_mapper.ConfigurationProvider)
             .FirstOrDefaultAsync();

            return Response<StudentCourseDto>.Success(result);
        }


        public async Task<Response<PagedResponse<StudentCourseDto>>> GetCourseEnrollmentRequestsAsync(int courseId, int instructorId, RequestStatus? status = null, int pageNumber = 1, int pageSize = 10)
        {

            // Check course ownership
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null || course.IsDeleted)
                return Response<PagedResponse<StudentCourseDto>>.Fail(ErrorType.COURSE_NOT_FOUND,
                    new ErrorDetail("Course not found"));

            if (course.InstructorId != instructorId)
                return Response<PagedResponse<StudentCourseDto>>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only view enrollments for your own courses"));

            var query = _studentCourseRepository.GetAll()
                .Where(e => e.CourseId == courseId);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);


            // var enrollmentList = query.

            var totalRecords = await query.CountAsync();

            var InstructorRequests = await query
             .ProjectTo<StudentCourseDto>(_mapper.ConfigurationProvider) 
             .Skip((pageNumber - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync();

            var pagedResponse = new PagedResponse<StudentCourseDto>(InstructorRequests, pageNumber, pageSize, totalRecords);
            return Response<PagedResponse<StudentCourseDto>>.Success(pagedResponse);
        }
        #endregion

        #region ManageInstructorExams
        public async Task<Response<ManualExamDto>> ManualExamQuestionsAsync(AddManualExamDto manualExam, int currentUserId)
        {

            // Get exam
            var exam = await _examRepository.GetByIdAsync(manualExam.examId);
            if (exam == null)
                return Response<ManualExamDto>.Fail(ErrorType.EXAM_NOT_FOUND,
                    new ErrorDetail( "Exam not found"));

            // Verify all questions belong to same course
            var QuestionsCourse = await _questionRepository
                .GetAll(q => manualExam.questionIds.Contains(q.Id) && q.CourseId == exam.CourseId)
                .ToListAsync();

            if (QuestionsCourse.Count != manualExam.questionIds.Count)
                return Response<ManualExamDto>.Fail(ErrorType.INVALID_QUESTIONS,
                    new ErrorDetail("Some questions don't belong to your course or don't exist"));


            // Check for duplicates by comparing counts
            var duplicateQuestionIds = new List<int>();
            if (manualExam.questionIds.Count != manualExam.questionIds.Distinct().Count())
            {
                duplicateQuestionIds = manualExam.questionIds
                 .Where(id => manualExam.questionIds.Count(x => x == id) > 1)
                 .Distinct()
                 .ToList();

                return Response<ManualExamDto>.Fail(ErrorType.DUPLICATE_QUESTIONS_IN_REQUEST,
                    new ErrorDetail($"Duplicate question IDs in request: {string.Join(", ", duplicateQuestionIds)}"));
            }


            // Add exam questions
            var examQuestions = manualExam.questionIds.Select(questionId => new ExamQuestion
            {
                ExamId = manualExam.examId,
                QuestionId = questionId,
                CreatedBy = currentUserId
            }).ToList();

            await _examQuestionRepository.AddRangeAsync(examQuestions);
            await _examQuestionRepository.SaveChangesAsync();

           

            var examQuestionIds = await _examQuestionRepository.GetAll(eq => eq.ExamId == manualExam.examId).Select(eq => eq.QuestionId).ToListAsync();

            var choices = await _choiceRepository.GetAll(c => examQuestionIds.Contains(c.QuestionId)).ToListAsync();

            var questionsList = await _questionRepository.GetAll(q => examQuestionIds.Contains(q.Id)).ToListAsync();

            var questionList = new List<ExamQuestionDto>();
            foreach (var question in questionsList)
            {
                var questionChoices = choices.Where(c => c.QuestionId == question.Id).ToList();

                var choicesInfo = questionChoices.Select(c => new ExamWithQuestionsChoicesDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect,
                    QuestionId = c.QuestionId
                }).ToList();

                questionList.Add(new ExamQuestionDto
                {
                    QuestionId = question.Id,
                    Content = question.Content,
                    Level = question.QuestionLevel,
                    Mark = question.Mark,
                    Choices = choicesInfo
                });
            }

            // Manual mapping for exam
            var result =  new ManualExamDto
            {
                ExamId = exam.Id,
                Title = exam.Title,
                ExamType = exam.ExamType.ToString(),
                CourseId = exam.CourseId,
                CourseTitle = exam.Course.Title,
                Duration = exam.Duration,
                QuestionCount = examQuestionIds.Count(),
                IsAutoGenerated = exam.IsAutoGenerated,
                Questions = questionList
            };
            return Response<ManualExamDto>.Success(result);

        }
    

        public async Task<Response<PagedResponse<StudentExamResultDto>>> GetAllStudentResultAsync(int courseId, int currentUserId, int pageNumber = 1, int pageSize = 10)
        {
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course == null)
                    return Response<PagedResponse<StudentExamResultDto>>.Fail(ErrorType.COURSE_NOT_FOUND,
                        new ErrorDetail("COURSE_NOT_FOUND", "Course not found"));

                if (course.InstructorId != currentUserId)
                    return Response<PagedResponse<StudentExamResultDto>>.Fail(ErrorType.ACCESS_DENIED,
                        new ErrorDetail("ACCESS_DENIED", "You can only view scores for your own courses"));


            // Get all completed exam results for this course
            var query = _examResultRepository.GetAll(er => er.Exam.CourseId == courseId && er.SubmittedAt != null)
                .OrderByDescending(er => er.SubmittedAt);

            var totalRecords = await query.CountAsync();
            var result = await query
            .ProjectTo<StudentExamResultDto>(_mapper.ConfigurationProvider) // Single efficient query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var pagedResponse = new PagedResponse<StudentExamResultDto>(result, pageNumber, pageSize, totalRecords);
            return Response<PagedResponse<StudentExamResultDto>>.Success(pagedResponse);



        }
        #endregion
    }
}


        
    

