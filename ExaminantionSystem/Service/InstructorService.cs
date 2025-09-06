using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Dtos.Courcse;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Dtos.Student;
using ExaminantionSystem.Entities.Enums;
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

        // private readonly InstructorRepository _instructorRepository;

        public InstructorService(ChoiceRepository choiceRepository,
            Repository<StudentCourse> studentCourseRepository,
            Repository<ExamResult> examResultRepository,
            CourseRepository courseRepository,
            StudentRepository studentRepository,
            Repository<ExamQuestion> examQuestionRepository,
            ExamRepository examRepository,
            QuestionRepository questionRepository)
        {
            _studentCourseRepository = studentCourseRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _examQuestionRepository = examQuestionRepository;
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _examResultRepository = examResultRepository;
            _choiceRepository = choiceRepository;
            // _instructorRepository = instructorRepository;
        }

        #region ManageInstructorEnrollments
        public async Task<Response<StudentCourseDto>> ApproveEnrollmentAsync(int enrollmentId, int currenUserId)
        {
            var enrollment = await _studentCourseRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null)
                return Response<StudentCourseDto>.Fail(ErrorType.NotFound,
                    new ErrorDetail("ENROLLMENT_NOT_FOUND", "Enrollment request not found"));

            // Check if instructor owns the course
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId);
            if (course.InstructorId != currenUserId)
                return Response<StudentCourseDto>.Fail(ErrorType.Forbidden,
                    new ErrorDetail("ACCESS_DENIED", "You can only manage enrollments for your own courses"));

            // Only pending requests can be approved
            if (enrollment.Status != RequestStatus.Pending)
                return Response<StudentCourseDto>.Fail(ErrorType.BusinessRule,
                    new ErrorDetail("INVALID_STATUS", "Only pending requests can be approved"));

            enrollment.Status = RequestStatus.Approved;
            enrollment.EnrollmentDate = DateTime.UtcNow;
            enrollment.IsActive = true;
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _studentCourseRepository.UpdateAsync(enrollment);
            await _studentCourseRepository.SaveChangesAsync();

            var student = await _studentRepository.GetByIdAsync(enrollment.StudentId);
            var courseInfo = await _courseRepository.GetByIdAsync(enrollment.CourseId);
            var result = new StudentCourseDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = student.FullName,
                CourseId = enrollment.CourseId,
                CourseTitle = courseInfo.Title,
                Status = enrollment.Status,
                RequestDate = enrollment.RequestDate,
                EnrollmentDate = enrollment.EnrollmentDate,
            };
            return Response<StudentCourseDto>.Success(result);
        }

        public async Task<Response<StudentCourseDto>> RejectEnrollmentAsync(int enrollmentId, int currentuserId)
        {

            var enrollment = await _studentCourseRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null)
                return Response<StudentCourseDto>.Fail(ErrorType.NotFound,
                    new ErrorDetail("ENROLLMENT_NOT_FOUND", "Enrollment request not found"));

            // Check if instructor owns the course
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId);
            if (course.InstructorId != currentuserId)
                return Response<StudentCourseDto>.Fail(ErrorType.Forbidden,
                    new ErrorDetail("ACCESS_DENIED", "You can only manage enrollments for your own courses"));

            // Only pending requests can be rejected
            if (enrollment.Status != RequestStatus.Pending)
                return Response<StudentCourseDto>.Fail(ErrorType.BusinessRule,
                    new ErrorDetail("INVALID_STATUS", "Only pending requests can be rejected"));

            enrollment.Status = RequestStatus.Rejected;
            enrollment.EnrollmentDate = null; // Clear enrollment date for rejected requests
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _studentCourseRepository.UpdateAsync(enrollment);
            await _studentCourseRepository.SaveChangesAsync();

            //Mapping
            var student = await _studentRepository.GetByIdAsync(enrollment.StudentId);
            var courseInfo = await _courseRepository.GetByIdAsync(enrollment.CourseId);
            var result = new StudentCourseDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = student.FullName,
                CourseId = enrollment.CourseId,
                CourseTitle = courseInfo.Title,
                Status = enrollment.Status,
                RequestDate = enrollment.RequestDate,
                EnrollmentDate = enrollment.EnrollmentDate,
            };
            return Response<StudentCourseDto>.Success(result);
        }


        public async Task<Response<PagedResponse<StudentCourseDto>>> GetCourseEnrollmentRequestsAsync(int courseId, int instructorId, RequestStatus? status = null, int pageNumber = 1, int pageSize = 10)
        {

            // Check course ownership
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null || course.IsDeleted)
                return Response<PagedResponse<StudentCourseDto>>.Fail(ErrorType.NotFound,
                    new ErrorDetail("COURSE_NOT_FOUND", "Course not found"));

            if (course.InstructorId != instructorId)
                return Response<PagedResponse<StudentCourseDto>>.Fail(ErrorType.Forbidden,
                    new ErrorDetail("ACCESS_DENIED", "You can only view enrollments for your own courses"));

            var query = _studentCourseRepository.GetAll()
                .Where(e => e.CourseId == courseId);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);


            // var enrollmentList = query.

            var totalRecords = await query.CountAsync();
            var enrollments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var InstructorRequests = new List<StudentCourseDto>();
            foreach (var enrollment in enrollments)
            {
                var student = await _studentRepository.GetByIdAsync(enrollment.StudentId);
                var courseInfo = await _courseRepository.GetByIdAsync(enrollment.CourseId);
                InstructorRequests.Add(new StudentCourseDto
                {
                    Id = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    StudentName = student.FullName,
                    CourseId = enrollment.CourseId,
                    CourseTitle = courseInfo.Title,
                    Status = enrollment.Status,
                    RequestDate = enrollment.RequestDate,
                    EnrollmentDate = enrollment.EnrollmentDate,
                });
            }
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
                return Response<ManualExamDto>.Fail(ErrorType.NotFound,
                    new ErrorDetail("EXAM_NOT_FOUND", "Exam not found"));

            // Verify all questions belong to same course
            var QuestionsCourse = await _questionRepository
                .GetAll(q => manualExam.questionIds.Contains(q.Id) && q.CourseId == exam.CourseId)
                .ToListAsync();

            if (QuestionsCourse.Count != manualExam.questionIds.Count)
                return Response<ManualExamDto>.Fail(ErrorType.Validation,
                    new ErrorDetail("INVALID_QUESTIONS", "Some questions don't belong to your course or don't exist"));


            // Check for duplicates by comparing counts
            var duplicateQuestionIds = new List<int>();
            if (manualExam.questionIds.Count != manualExam.questionIds.Distinct().Count())
            {
                duplicateQuestionIds = manualExam.questionIds
                 .Where(id => manualExam.questionIds.Count(x => x == id) > 1)
                 .Distinct()
                 .ToList();

                return Response<ManualExamDto>.Fail(ErrorType.Validation,
                    new ErrorDetail("DUPLICATE_QUESTIONS_IN_REQUEST",
                        $"Duplicate question IDs in request: {string.Join(", ", duplicateQuestionIds)}"));
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
                    return Response<PagedResponse<StudentExamResultDto>>.Fail(ErrorType.NotFound,
                        new ErrorDetail("COURSE_NOT_FOUND", "Course not found"));

                if (course.InstructorId != currentUserId)
                    return Response<PagedResponse<StudentExamResultDto>>.Fail(ErrorType.Forbidden,
                        new ErrorDetail("ACCESS_DENIED", "You can only view scores for your own courses"));


            // Get all completed exam results for this course
            var query = _examResultRepository.GetAll(er => er.Exam.CourseId == courseId && er.SubmittedAt != null)
                .OrderByDescending(er => er.SubmittedAt);

            var totalRecords = await query.CountAsync();
            var examResults = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Manual mapping
            var result = new List<StudentExamResultDto>();
            foreach (var examResult in examResults)
            {
                var studentInfo = await _studentRepository.GetByIdAsync(examResult.StudentId);
                var examInfo = await _examRepository.GetByIdAsync(examResult.ExamId);

                result.Add(new StudentExamResultDto
                {
                    StudentId = examResult.StudentId,
                    StudentName =  studentInfo.FullName,
                    ExamId = examResult.ExamId,
                    ExamTitle = examInfo.Title,
                    ExamType = examInfo.ExamType.ToString(),
                    Score = examResult.Score,
                    SubmittedAt = examResult.SubmittedAt.Value
                });
            }

            var pagedResponse = new PagedResponse<StudentExamResultDto>(result, pageNumber, pageSize, totalRecords);
            return Response<PagedResponse<StudentExamResultDto>>.Success(pagedResponse);



        }
        #endregion
    }
}


        
    

