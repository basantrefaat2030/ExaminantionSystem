
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Shared;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Service
{
    public class ExamService
    {
        private readonly ExamRepository _examRepository;
        private readonly CourseRepository _courseRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly Repository<ExamQuestion> _examQuestionRepository;
        private readonly InstructorRepository _instructorRepository;
        private readonly ChoiceRepository _choiceRepository;
        private readonly IMapper _mapper;


        public ExamService(
        ExamRepository examRepository,
        CourseRepository courseRepository,
        QuestionRepository questionRepository,
        Repository<ExamQuestion> examQuestionRepository,
        InstructorRepository instructorRepository,
        ChoiceRepository choiceRepository,
        IMapper mapper)
        {
            _examRepository = examRepository;
            _courseRepository = courseRepository;
            _questionRepository = questionRepository;
            _examQuestionRepository = examQuestionRepository;
            _instructorRepository = instructorRepository;
            _choiceRepository = choiceRepository;
            _mapper = mapper;
        }

        #region ExamCRUDOperation
        public async Task<Response<ExamDto>> CreateExamAsync(CreateExamDto dto, int currentUserId)
        {

            var courseInfo = await _courseRepository.GetAll(c => c.Id == dto.CourseId)
                                  .Select(c => new
                                  {
                                      IsAuthorized = c.InstructorId == currentUserId
                                  })
                                  .FirstOrDefaultAsync();
            if (courseInfo == null)
                return Response<ExamDto>.Fail(ErrorType.COURSE_NOT_FOUND, new ErrorDetail("Course not found"));

            if (!courseInfo.IsAuthorized)
                return Response<ExamDto>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only create exams for your own courses"));

            // For final exams, check if one already exists
            if (dto.Type == ExamType.Final)
            {
                var existingFinalExam = await _examRepository.GetAll(e => e.CourseId == dto.CourseId && e.ExamType == ExamType.Final).AnyAsync();

                if (!existingFinalExam)
                    return Response<ExamDto>.Fail(ErrorType.FINAL_EXAM_EXISTS,
                        new ErrorDetail("A final exam already exists for this course"));
            }

            var exam = _mapper.Map<Exam>(dto);
            exam.CreatedBy = currentUserId;

            await _examRepository.AddAsync(exam);
            await _examRepository.SaveChangesAsync();

            var result = _mapper.Map<ExamDto>(exam);

            return Response<ExamDto>.Success(result);
        }

        public async Task<Response<ExamDto>> UpdateExamAsync(UpdateExamDto examDto, int currentUserId)
        {
            // Get exam

            var examInfo = await _examRepository.GetAll(e => e.Id == examDto.ExamId).Select(exam => new
            {
                Exam = exam,
                ExamStarted = exam.StartDate > DateTime.Now,
                IsAuthorized = exam.Course.InstructorId == currentUserId,

            }).FirstOrDefaultAsync();

            if (examInfo == null)
                return Response<ExamDto>.Fail(ErrorType.EXAM_NOT_FOUND,
                    new ErrorDetail("Exam not found"));

            if (examInfo.ExamStarted)
                return Response<ExamDto>.Fail(ErrorType.EXAM_ALREADY_STARTED,
                    new ErrorDetail("Cannot update an exam that has already started"));

            if (!examInfo.IsAuthorized)
                return Response<ExamDto>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only update your own exams"));

            // Update exam
            _mapper.Map(examDto, examInfo.Exam);

            // exam.UpdatedAt = DateTime.UtcNow;

            await _examRepository.UpdateAsync(examInfo.Exam);
            await _examRepository.SaveChangesAsync();

            var result = _mapper.Map<ExamDto>(examInfo.Exam);
            return Response<ExamDto>.Success(result);
        }


        public async Task<Response<bool>> DeleteExamAsync(int examId, int currentUserId)
        {
            var examInfo = await _examRepository.GetAll(e => e.Id == examId).Select(exam => new
            {
                IsAuthorized = exam.Course.InstructorId == currentUserId,

            }).FirstOrDefaultAsync();

            if (examInfo == null)
                return Response<bool>.Fail(ErrorType.EXAM_NOT_FOUND,
                    new ErrorDetail("Exam not found"));

            // Check ownership
            if (!examInfo.IsAuthorized)
                return Response<bool>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only delete your own exams"));

            // Check if exam has student attempts
            var hasAttempts = await _examRepository.IsExamHasSubmissionsAsync(examId);

            if (hasAttempts)
                return Response<bool>.Fail(ErrorType.EXAM_HAS_ATTEMPTS,
                    new ErrorDetail("Cannot delete exam with student attempts"));

            await _examRepository.DeleteAsync(examId);
            await _examRepository.SaveChangesAsync();

            return Response<bool>.Success(true);
        }

        #endregion


        //i used fixed precentage 
        public async Task<Response<ExamWithQuestionsDto>> AutoGenerateExamQuestionsAsync(int examId)
        {
            // Get exam
            var examInfo = await _examRepository.GetAll(e => e.Id == examId).Select(ex => new
            {
                courseId = ex.CourseId,
                numberOfQuetion = ex.NumberOfQuestion,

            }).FirstOrDefaultAsync();


            if (examInfo == null)
                return Response<ExamWithQuestionsDto>.Fail(ErrorType.EXAM_NOT_FOUND,
                    new ErrorDetail("Exam not found"));

            // Get questions by level
            var questionsByLevel = await _questionRepository.GetAll(q => q.CourseId == examInfo.courseId)
                .GroupBy(q => q.QuestionLevel)
                .ToDictionaryAsync(g => g.Key, g => g.ToList());

            if (questionsByLevel.Values.Sum(list => list.Count) < examInfo.numberOfQuetion)

                return Response<ExamWithQuestionsDto>.Fail(ErrorType.INVALID_QUESTION_NUMBER,
                    new ErrorDetail("Not enough questions available for auto-generation"));

            // Balance questions by level (40% simple, 40% medium, 20% hard)
            var selectedQuestions = new List<Question>();

            // Add simple questions(40 %)
            var easyCount = (int)Math.Ceiling(examInfo.numberOfQuetion * 0.4);

            // Add medium questions (40%)
            var mediumCount = (int)Math.Ceiling(examInfo.numberOfQuetion * 0.4);

            // Add hard questions (20%)
            var hardCount = examInfo.numberOfQuetion - easyCount - mediumCount;

            //TryGetValue is a methid avaliable in C# dictioary to check if a key exists and get its value in a single operation
            //return true or false 

            if (questionsByLevel.TryGetValue(QuestionLevel.Easy, out var easyQuestions))
                selectedQuestions.AddRange(easyQuestions.Take(easyCount));

            if (questionsByLevel.TryGetValue(QuestionLevel.Medium, out var mediumQuestions))
                selectedQuestions.AddRange(mediumQuestions.Take(mediumCount));

            if (questionsByLevel.TryGetValue(QuestionLevel.Hard, out var hardQuestions))
                selectedQuestions.AddRange(hardQuestions.Take(hardCount));

            // If we don't have enough questions, fill with available ones
            if (selectedQuestions.Count < examInfo.numberOfQuetion)
            {
                var allQuestions = questionsByLevel.Values.SelectMany(x => x).ToList();
                var remainingQuestions = allQuestions.Except(selectedQuestions).Take(examInfo.numberOfQuetion - selectedQuestions.Count);
                selectedQuestions.AddRange(remainingQuestions);
            }


            // Add exam questions using separated function
            var addResult = await AddQuestionsToExamAsync(examId, selectedQuestions);
            if (!addResult.Succeeded)
                return Response<ExamWithQuestionsDto>.Fail(addResult.Error.Type, addResult.Error.Errors.ToArray());

            // Get the complete exam with questions and choices for response

            var examWithDetails = await _examRepository.GetAll(e => e.Id == examId)
                  .ProjectTo<ExamWithQuestionsDto>(_mapper.ConfigurationProvider)
                  .FirstOrDefaultAsync();

            return Response<ExamWithQuestionsDto>.Success(examWithDetails);

        }

        //currentUserId will be admin >> for autogenerate

        private async Task<Response<bool>> AddQuestionsToExamAsync(int examId, List<Question> questions)
        {
            var examQuestions = questions.Select(q => new ExamQuestion
            {
                ExamId = examId,
                QuestionId = q.Id
            }).ToList();

            await _examQuestionRepository.AddRangeAsync(examQuestions);
            await _examQuestionRepository.SaveChangesAsync();

            return Response<bool>.Success(true);
        }

        public async Task<Response<ExamWithQuestionsDto>> GetExamWithQuestionsAsync(int examId)
        {
            var examWithDetails = await _examRepository.GetAll(e => e.Id == examId)
                  .ProjectTo<ExamWithQuestionsDto>(_mapper.ConfigurationProvider)
                  .FirstOrDefaultAsync();

            if (examWithDetails == null)
                return Response<ExamWithQuestionsDto>.Fail(ErrorType.EXAM_NOT_FOUND,
                    new ErrorDetail("Exam not found"));

            return Response<ExamWithQuestionsDto>.Success(examWithDetails);

        }

        public async Task<Response<List<ExamDto>>> GetExamsByCourseAsync(int courseId, int currentUserId)
        {
            var courseExams = await _courseRepository.GetAll(c => c.Id == courseId)
                .Select(c => new
                {
                    Course = c,
                    IsAuthorized = c.InstructorId == currentUserId,
                    CourseExamsInfo = c.Exams
                        .Where(e => e.IsActive && !e.IsDeleted)
                        .Select(e => new ExamDto
                        {
                            Id = e.Id,
                            Title = e.Title,
                            Description = e.Description,
                            Duration = e.Duration,
                            EndDate = e.EndDate,
                            StartDate = e.StartDate,
                            IsAutoGenerated = e.IsAutoGenerated,
                            NumberOfQuestion = e.NumberOfQuestion,
                            CourseInfo = new ExamCourseInfoDto()
                            {
                                CourseId = c.Id,
                                CourseTitle = c.Title,
                            }
                           
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (courseExams.Course == null)
                return Response<List<ExamDto>>.Fail(ErrorType.COURSE_NOT_FOUND, new ErrorDetail("Course not found"));

            if (!courseExams.IsAuthorized)
                return Response<List<ExamDto>>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only view exams for your own courses"));

            if (!courseExams.CourseExamsInfo.Any())
                return Response<List<ExamDto>>.Fail(ErrorType.THIS_COURSE_NOT_HAS_EXAMS,
                    new ErrorDetail("No exams found for this course"));

            return Response<List<ExamDto>>.Success(courseExams.CourseExamsInfo);
        }

    }
}
