using ExaminantionSystem.Entities.Dtos.Courcse;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.Dtos.Student;
using ExaminantionSystem.Entities.Enums;
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
           QuestionRepository questionRepository)
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

        }

        #region StudentEnrollment

        public async Task<Response<StudentEnrollmentDto>> RequestEnrollmentAsync(int courseId, int currentUserId)
        {
            // Check if course exists
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                return Response<StudentEnrollmentDto>.Fail(ErrorType.NotFound,
                    new ErrorDetail("COURSE_NOT_FOUND", "Course not found"));

            // Check if student already has a pending or approved request
            var existingEnrollment = await _studentCourseRepository
                          .GetAll(e => e.StudentId == currentUserId && e.CourseId == courseId &&
                           (e.Status == RequestStatus.Pending || e.Status == RequestStatus.Approved)).FirstOrDefaultAsync();

            if (existingEnrollment != null)
            {
                var errorMessage = existingEnrollment.Status == RequestStatus.Approved
                    ? "You are already enrolled in this course"
                    : "You already have a pending enrollment request for this course";

                return Response<StudentEnrollmentDto>.Fail(ErrorType.Conflict,
                    new ErrorDetail("ENROLLMENT_EXISTS", errorMessage));
            }

            var enrollment = new StudentCourse
            {
                StudentId = currentUserId,
                CourseId = courseId,
                Status = RequestStatus.Pending,
                RequestDate = DateTime.UtcNow,
                IsActive = false,
                CreatedBy = currentUserId
            };

            await _studentCourseRepository.AddAsync(enrollment);
            await _studentCourseRepository.SaveChangesAsync();

            // Manual mapping in the same function
            var studentInfo = await _studentRepository.GetByIdAsync(enrollment.StudentId);
            var courseInfo = await _courseRepository.GetByIdAsync(enrollment.CourseId);
            var instructorInfo = await _instructorRepository.GetByIdAsync(courseInfo.InstructorId);

            var result = new StudentEnrollmentDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = studentInfo.FullName,
                CourseId = enrollment.CourseId,
                CourseTitle = courseInfo.Title,
                Status = enrollment.Status,
                RequestDate = enrollment.RequestDate,
                EnrollmentDate = enrollment.EnrollmentDate,
                InstructorName = instructorInfo.FullName,
            };
            return Response<StudentEnrollmentDto>.Success(result);
        }

        public async Task<Response<bool>> CancelEnrollmentRequestAsync(int enrollmentId, int studentId)
        {
            var enrollment = await _studentCourseRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null || enrollment.IsDeleted)
                return Response<bool>.Fail(ErrorType.NotFound,
                    new ErrorDetail("ENROLLMENT_NOT_FOUND", "Enrollment request not found"));

            // Check ownership
            if (enrollment.StudentId != studentId)
                return Response<bool>.Fail(ErrorType.Forbidden,
                    new ErrorDetail("ACCESS_DENIED", "You can only cancel your own enrollment requests"));

            // Only pending requests can be cancelled
            if (enrollment.Status != RequestStatus.Pending)
                return Response<bool>.Fail(ErrorType.BusinessRule,
                    new ErrorDetail("CANCEL_NOT_ALLOWED", "Only pending requests can be cancelled"));

            enrollment.Status = RequestStatus.Cancelled;
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _studentCourseRepository.UpdateAsync(enrollment);
            await _studentCourseRepository.SaveChangesAsync();

            return Response<bool>.Success(true);

        }

        public async Task<Response<PagedResponse<StudentEnrollmentDto>>> GetStudentEnrollmentsAsync(int studentId, RequestStatus? status = null, int pageNumber = 1, int pageSize = 10)
        {

            var query = _studentCourseRepository.GetAll()
                .Where(e => e.StudentId == studentId);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);


            query = query.OrderByDescending(e => e.RequestDate);

            var totalRecords = await query.CountAsync();
            var enrollments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var studentEnrollments = new List<StudentEnrollmentDto>();
            foreach (var enrollment in enrollments)
            {
                var studentInfo = await _studentRepository.GetByIdAsync(enrollment.StudentId);
                var courseInfo = await _courseRepository.GetByIdAsync(enrollment.CourseId);
                var instructorInfo = await _instructorRepository.GetByIdAsync(enrollment.CourseId);

                studentEnrollments.Add(new StudentEnrollmentDto
                {
                    Id = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    StudentName = studentInfo.FullName,
                    CourseId = enrollment.CourseId,
                    CourseTitle = courseInfo.Title,
                    Status = enrollment.Status,
                    RequestDate = enrollment.RequestDate,
                    EnrollmentDate = enrollment.EnrollmentDate,
                    InstructorName = instructorInfo.FullName
                });
            }

            var pagedResponse = new PagedResponse<StudentEnrollmentDto>(studentEnrollments, pageNumber, pageSize, totalRecords);
            return Response<PagedResponse<StudentEnrollmentDto>>.Success(pagedResponse);


        }

        #endregion

        #region studentExams
        public async Task<Response<StudentExamDto>> StartExamAsync(int examId, int currenUserId)
        {
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                return Response<StudentExamDto>.Fail(ErrorType.NotFound,
                    new ErrorDetail("EXAM_NOT_FOUND", "Exam not found"));

            // Check if student is enrolled in the course
            var isEnrolled = await _studentRepository.IsStudentEnrolledInCourseAsync(exam.CourseId, currenUserId);

            if (!isEnrolled)
                return Response<StudentExamDto>.Fail(ErrorType.Forbidden,
                    new ErrorDetail("NOT_ENROLLED", "You are not enrolled in this course"));

            // Check if student already has an active exam result
            var existingResult = await _studentRepository.IsStudentHasActiveExamAsync(examId, currenUserId);

            if (existingResult)
                return Response<StudentExamDto>.Fail(ErrorType.Conflict,
                    new ErrorDetail("EXAM_ALREADY_STARTED", "You have already started this exam"));

            var examResult = new ExamResult
            {
                StudentId = currenUserId,
                ExamId = examId,
                StartedAt = DateTime.UtcNow,
                CreatedBy = currenUserId
            };

            await _examResultRepository.AddAsync(examResult);
            await _examResultRepository.SaveChangesAsync();

            // Manual mapping
            var studentInfo = await _studentRepository.GetByIdAsync(currenUserId);
            var courseInfo = await _courseRepository.GetByIdAsync(exam.CourseId);

            var result = new StudentExamDto
            {
                ExamResultId = examResult.Id,
                ExamId = examResult.ExamId,
                ExamTitle = exam.Title,
                StudentId = examResult.StudentId,
                StudentName = studentInfo.FullName,
                StartedAt = examResult.StartedAt.Value,
                CourseName = courseInfo.Title,
                Duration = exam.Duration
            };

            return Response<StudentExamDto>.Success(result);
        }


        public async Task<Response<ExamResultDto>> SubmitExamAnswersAsync(SubmitStudentAnswerDto answerDto, int currentUserId)
        {

            var examResult = await _studentRepository.StudentHasActiveExamAsync(answerDto.examId, currentUserId);
            if (examResult == null)
                return Response<ExamResultDto>.Fail(ErrorType.NotFound,
                    new ErrorDetail("EXAM_RESULT_NOT_FOUND", "Exam result not found"));

            if (examResult.StudentId != currentUserId)
                return Response<ExamResultDto>.Fail(ErrorType.Forbidden,
                    new ErrorDetail("ACCESS_DENIED", "You can only submit answers for your own exam"));

            //if (examResult.SubmittedAt != null)
            //    return Response<bool>.Fail(ErrorType.BusinessRule,
            //        new ErrorDetail("EXAM_ALREADY_SUBMITTED", "Exam has already been submitted"));

            var exam = await _examRepository.GetByIdAsync(examResult.ExamId);

            // Check if time has expired
            var timeElapsed = DateTime.UtcNow - examResult.StartedAt.Value;
            if (timeElapsed.TotalMinutes > exam.Duration)
                return Response<ExamResultDto>.Fail(ErrorType.BusinessRule,
                    new ErrorDetail("EXAM_TIME_EXPIRED", "Exam time has expired"));

            try
            {
                // Save student answers
                var studentAnswers = answerDto.answers.Select(answer => new StudentAnswer
                {
                    ExamResultId = examResult.Id,
                    QuestionId = answer.QuestionId,
                    ChoiceId = answer.ChoiceId,
                    CreatedBy = currentUserId
                }).ToList();

                // Evaluate the exam
                var evaluationResult = await EvaluateExamAsync(examResult.Id, currentUserId);
                if (!evaluationResult.Succeeded)
                    throw new Exception("Failed to evaluate exam: " + string.Join(", ", evaluationResult.Error.Errors.Select(e => e.Detail)));


                return evaluationResult;
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        private async Task<Response<ExamResultDto>> EvaluateExamAsync(int examResultId, int studentId)
        {
            // Get student answers
            var studentAnswers = _studentAnswerRepository.GetAll(sa => sa.ExamResultId == examResultId);

            // Get questions and choices for evaluation
            var questionIds = studentAnswers.Select(sa => sa.QuestionId).ToList();
            var choiceIds = studentAnswers.Where(sa => sa.ChoiceId.HasValue).Select(sa => sa.ChoiceId.Value).ToList();

            var questions = await _questionRepository.GetAll()
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();

            var choices = await _choiceRepository.GetAll()
                .Where(c => choiceIds.Contains(c.Id))
                .ToListAsync();

            // Calculate score
            double totalScore = 0;
            double totalMarks = 0;
            var questionResults = new List<EvaluateQuestionResultDto>();

            foreach (var answer in studentAnswers)
            {
                var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null) continue;

                totalMarks += question.Mark;

                // Check if answer is correct
                bool isCorrect = false;
                string selectedChoiceContent = null;

                if (answer.ChoiceId.HasValue)
                {
                    var choice = choices.FirstOrDefault(c => c.Id == answer.ChoiceId.Value);
                    if (choice != null)
                    {
                        selectedChoiceContent = choice.Text;
                        isCorrect = choice.IsCorrect;
                    }
                }

                var Marks = isCorrect ? question.Mark : 0.0;
                totalScore += Marks;

                questionResults.Add(new EvaluateQuestionResultDto
                {
                    QuestionId = answer.QuestionId,
                    Content = question.Content,
                    Mark = question.Mark,
                    EarnedMarks = Marks,
                    IsCorrect = isCorrect,
                    SelectedChoiceId = answer.ChoiceId,
                    SelectedChoiceContent = selectedChoiceContent
                });
            }

            //double percentage = totalMarks > 0 ? (totalScore / totalMarks) * 100 : 0;

            // Update exam result with score

            await _examResultRepository.UpdateAsync(er => er.Id == examResultId,
                up => up.SetProperty(c => c.Score, totalScore)
                        .SetProperty(c => c.SubmittedAt, DateTime.UtcNow));
            await _examResultRepository.SaveChangesAsync();

            // Manual mapping for final result
            var examResultInfo = await _examResultRepository.GetByIdAsync(examResultId);

            var result = new ExamResultDto
            {
                examInfomationDto = new ExamInfomationDto()
                {
                    ExamId = examResultInfo.Exam.Id,
                    ExamTitle = examResultInfo.Exam.Title,
                    CourseName = examResultInfo.Exam.Course.Title
                },
                studentExamInformation = new StudentExamInformation()
                {
                    StudentId = examResultInfo.StudentId,
                    StudentName = examResultInfo.Student.FullName
                },

                TotalScore = totalScore,

                ExamStartAt = examResultInfo.StartedAt.Value,
                ExamCompletedAt = examResultInfo.SubmittedAt.Value,
                QuestionResults = questionResults
            };

            return Response<ExamResultDto>.Success(result);
        }

        #endregion


    }
}

