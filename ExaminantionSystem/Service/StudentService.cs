using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.Dtos.Student;
using ExaminantionSystem.Entities.Enums;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Service
{
    public class StudentService
    {
        private readonly Repository<StudentCourse> _studentCourseRepository;
        private readonly Repository<StudentAnswer> _studentAnswerRepository;
        private readonly CourseRepository _courseRepository;
        private readonly StudentRepository _studentRepository;
        private readonly InstructorRepository _instructorRepository;
        private readonly ExamRepository _examRepository;
        private readonly Repository<ExamResult> _examResultRepository;
        private readonly ChoiceRepository _choiceRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly IMapper _mapper;
        public StudentService
         (
           Repository<StudentCourse> studentCourseRepository,
           Repository<StudentAnswer> studentAnswerRepository,
           CourseRepository courseRepository,
           StudentRepository studentRepository,
           InstructorRepository instructorRepository,
           ExamRepository examRepository,
           Repository<ExamResult> examResultRepository,
           ChoiceRepository choiceRepository,
           QuestionRepository questionRepository,
           IMapper mapper)
        {
            _studentCourseRepository = studentCourseRepository;
            _studentAnswerRepository = studentAnswerRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _instructorRepository = instructorRepository;
            _examRepository = examRepository;
            _examResultRepository = examResultRepository;
            _choiceRepository = choiceRepository;
            _questionRepository = questionRepository;
            _mapper = mapper;

        }

        #region StudentEnrollment

        public async Task<Response<StudentEnrollmentDto>> RequestEnrollmentAsync(int courseId, int currentUserId)
        {

            var courseInfo = await _courseRepository.GetAll(c => c.Id == courseId)
                .Select(c => new
                {
                    ExistingEnrollment = c.Enrollments
                        .FirstOrDefault(sc => sc.StudentId == currentUserId && sc.IsActive && !sc.IsDeleted &&
                                            (sc.Status == RequestStatus.Pending || sc.Status == RequestStatus.Approved))
                }).FirstOrDefaultAsync();


            // Check if course exists
            if (courseInfo == null)
                return Response<StudentEnrollmentDto>.Fail(ErrorType.COURSE_NOT_FOUND,
                    new ErrorDetail("Course not found"));

            // Check if student already has a pending or approved request

            if (courseInfo.ExistingEnrollment != null)
            {
                var errorMessage = courseInfo.ExistingEnrollment.Status == RequestStatus.Approved
                    ? "You are already enrolled in this course"
                    : "You already have a pending enrollment request for this course";

                return Response<StudentEnrollmentDto>.Fail(ErrorType.ENROLLMENT_EXIST,
                    new ErrorDetail(errorMessage));
            }

            var enrollment = new StudentCourse
            {
                StudentId = currentUserId,
                CourseId = courseId,
                Status = RequestStatus.Pending,
                RequestDate = DateTime.Now,
                IsActive = false,
                CreatedBy = currentUserId
            };

            await _studentCourseRepository.AddAsync(enrollment);
            await _studentCourseRepository.SaveChangesAsync();

            var result = await _studentCourseRepository.GetAll(e => e.Id == enrollment.Id).ProjectTo<StudentEnrollmentDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();


            return Response<StudentEnrollmentDto>.Success(result);
        }

        public async Task<Response<bool>> CancelEnrollmentRequestAsync(int enrollmentId, int studentId)
        {
            var enrollmentInfo = await _studentCourseRepository.GetAll(e => e.Id == enrollmentId)
                .Select(e => new
                {
                    Enrollment = e,
                    IsOwner = e.StudentId == studentId,
                    CanCancel = e.Status == RequestStatus.Pending
                })
                .FirstOrDefaultAsync();
            
            if (enrollmentInfo == null)
                return Response<bool>.Fail(ErrorType.ENROLLMENT_NOT_FOUND,
                    new ErrorDetail("Enrollment request not found"));

            // Check ownership
            if (!enrollmentInfo.IsOwner)
                return Response<bool>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only cancel your own enrollment requests"));

            // Only pending requests can be cancelled
            if (!enrollmentInfo.CanCancel)
                return Response<bool>.Fail(ErrorType.CANCEL_NOT_ALLOWED,
                    new ErrorDetail("Only pending requests can be cancelled"));

            enrollmentInfo.Enrollment.Status = RequestStatus.Cancelled;
            enrollmentInfo.Enrollment.UpdatedAt = DateTime.UtcNow;

            await _studentCourseRepository.UpdateAsync(enrollmentInfo.Enrollment);
            await _studentCourseRepository.SaveChangesAsync();

            return Response<bool>.Success(true);

        }

        public async Task<Response<PagedResponse<StudentEnrollmentDto>>> GetStudentEnrollmentsAsync(int studentId, RequestStatus? status = null, int pageNumber = 1, int pageSize = 10)
        {
            //here can use predicate instaed of that 
            var query = _studentCourseRepository.GetAll(e => e.StudentId == studentId);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);


            query = query.OrderByDescending(e => e.RequestDate);

            var totalRecords = await query.CountAsync();

            //the .ProjectTo<StudentEnrollmentDto>(_mapper.ConfigurationProvider) mapping not in memory 
            var studentEnrollments = await query
            .ProjectTo<StudentEnrollmentDto>(_mapper.ConfigurationProvider)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var pagedResponse = new PagedResponse<StudentEnrollmentDto>(studentEnrollments, pageNumber, pageSize, totalRecords);
            return Response<PagedResponse<StudentEnrollmentDto>>.Success(pagedResponse);


        }

        #endregion

        #region studentExams
        public async Task<Response<StudentExamDto>> StartExamAsync(int examId, int currentUserId)
        {
            var examInfo = await _examRepository.GetAll()
                .Where(e => e.Id == examId && !e.IsDeleted)
                .Select(e => new
                {
                    Exam = e,
                    IsEnrolled = e.Course.Enrollments
                        .Any(sc => sc.StudentId == currentUserId && sc.Status == RequestStatus.Approved &&!sc.IsDeleted && sc.IsActive),
                    HasActiveExam = e.ExamResults
                        .Any(er => er.StudentId == currentUserId && er.SubmittedAt == null &&!er.IsDeleted && er.IsActive)
                }).FirstOrDefaultAsync();

            if (examInfo == null)
                return Response<StudentExamDto>.Fail(ErrorType.EXAM_NOT_FOUND,
                    new ErrorDetail( "Exam not found"));

            // Check if student is enrolled in the course
            //var isEnrolled = await _studentRepository.IsStudentEnrolledInCourseAsync(exam.CourseId, currenUserId);

            if (!examInfo.IsEnrolled)
                return Response<StudentExamDto>.Fail(ErrorType.NOT_ENROLLED,
                    new ErrorDetail("You are not enrolled in this course"));

            // Check if student already has an active exam result
            //var existingResult = await _studentRepository.IsStudentHasActiveExamAsync(examId, currenUserId);

            if (!examInfo.HasActiveExam)
                return Response<StudentExamDto>.Fail(ErrorType.EXAM_ALREADY_STARTED,
                    new ErrorDetail( "You have already started this exam"));

            var examResult = new ExamResult
            {
                StudentId = currentUserId,
                ExamId = examId,
                StartedAt = DateTime.Now,
                CreatedBy = currentUserId
            };

            await _examResultRepository.AddAsync(examResult);
            await _examResultRepository.SaveChangesAsync();

            var result = await _examResultRepository.GetAll(er => er.Id == examResult.Id).ProjectTo<StudentExamDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

            return Response<StudentExamDto>.Success(result);
        }


        public async Task<Response<ExamResultDto>> SubmitExamAnswersAsync(SubmitStudentAnswerDto answerDto, int currentUserId)
        {
            // Get exam result with validation in a single query
            var examResultInfo = await _examResultRepository.GetAll(er => er.Id == answerDto.examId && er.StudentId == currentUserId)
                .Select(er => new
                {
                    ExamInfo = er.Exam,
                    ExamResult = er,
                    IsSubmitted = er.SubmittedAt != null,
                    TimeElapsed = DateTime.Now - er.StartedAt.Value,
                    ExamDuration = er.Exam.Duration
                })
                .FirstOrDefaultAsync();

            if (examResultInfo == null)
                return Response<ExamResultDto>.Fail(ErrorType.EXAM_RESULT_NOT_FOUND,
                    new ErrorDetail("Exam result not found"));

            if (examResultInfo.IsSubmitted)
                return Response<ExamResultDto>.Fail(ErrorType.EXAM_ALREADY_SUBMITTED,
                    new ErrorDetail("Exam has already been submitted"));

            if (examResultInfo.TimeElapsed.TotalMinutes > examResultInfo.ExamDuration)
                return Response<ExamResultDto>.Fail(ErrorType.EXAM_TIME_EXPIRED,
                    new ErrorDetail("Exam time has expired"));

            // Save student answers
            var studentAnswers = _mapper.Map<List<StudentAnswer>>(answerDto.answers);
            studentAnswers.ForEach(sa =>
            {
                sa.ExamResultId = examResultInfo.ExamResult.Id;
                sa.CreatedBy = currentUserId;
            });

            await _studentAnswerRepository.AddRangeAsync(studentAnswers);
            await _studentAnswerRepository.SaveChangesAsync();

            // Evaluate the exam
            return await EvaluateExamAsync(examResultInfo.ExamResult.Id);


        }


        private async Task<Response<ExamResultDto>> EvaluateExamAsync(int examResultId)
        {
            var examResultInfo = await _examResultRepository.GetAll(er => er.Id == examResultId)
                .Select(er => new
                {
                    ExamResult = er,
                    StudentAnswers = er.StudentAnswers
                        .Where(sa => sa.IsActive && !sa.IsDeleted),
                    Questions = er.Exam.ExamQuestions
                        .Where(eq => eq.Question.IsActive && !eq.Question.IsDeleted)
                        .Select(eq => eq.Question),
                    AllChoices = er.Exam.ExamQuestions
                        .SelectMany(eq => eq.Question.Choices
                            .Where(ch => ch.IsActive && !ch.IsDeleted))
                })
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if (examResultInfo == null)
                return Response<ExamResultDto>.Fail(ErrorType.EXAM_RESULT_NOT_FOUND,
                    new ErrorDetail("Exam result not found"));

            // Calculate score
            double totalScore = 0;
            double totalMarks = 0;
            var questionResults = new List<EvaluateQuestionResultDto>();

            foreach (var answer in examResultInfo.StudentAnswers)
            {
                var question = examResultInfo.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null) continue;

                totalMarks += question.Mark;

                // Check if answer is correct
                bool isCorrect = false;
                string selectedChoiceContent = null;

                if (answer.ChoiceId.HasValue)
                {
                    var choice = examResultInfo.AllChoices.FirstOrDefault(c => c.Id == answer.ChoiceId.Value);
                    if (choice != null)
                    {
                        selectedChoiceContent = choice.Text;
                        isCorrect = choice.IsCorrect;
                    }
                }

                var earnedMarks = isCorrect ? question.Mark : 0.0;
                totalScore += earnedMarks;

                questionResults.Add(new EvaluateQuestionResultDto
                {
                    QuestionId = answer.QuestionId,
                    Content = question.Content,
                    Mark = question.Mark,
                    EarnedMarks = earnedMarks,
                    IsCorrect = isCorrect,
                    SelectedChoiceId = answer.ChoiceId,
                    SelectedChoiceContent = selectedChoiceContent
                });
            }

            // Update exam result
            examResultInfo.ExamResult.Score = totalScore;
            examResultInfo.ExamResult.SubmittedAt = DateTime.UtcNow;

            await _examResultRepository.UpdateAsync(examResultInfo.ExamResult);
            await _examResultRepository.SaveChangesAsync();

            // Map to DTO
            var examResultDto = _mapper.Map<ExamResultDto>(examResultInfo.ExamResult);
            examResultDto.TotalScore = totalScore;
            examResultDto.QuestionResults = questionResults;

            return Response<ExamResultDto>.Success(examResultDto);

        }

        #endregion


    }
}

