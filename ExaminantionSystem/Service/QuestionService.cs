using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Service
{
    public class QuestionService
    {
        private readonly QuestionRepository _questionRepository;
        private readonly ChoiceRepository _choiceRepository;
        private readonly ExamRepository _examRepository;
        private readonly CourseRepository _courseRepository;

        public QuestionService(QuestionRepository questionRepository, 
            ChoiceRepository choiceRepository, 
            ExamRepository ExamRepository,
            CourseRepository courseRepository)
        {
            _questionRepository = questionRepository;
            _choiceRepository = choiceRepository;
            _examRepository = _examRepository;
            _courseRepository = courseRepository;
        }

        public async Task<Response<QuestionDto>> CreateQuestionWithChoicesAsync(CreateQuestionDto dto, int currentUserId)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(dto.CourseId);
                if (course == null || course.IsDeleted)
                    return Response<QuestionDto>.Fail(ErrorType.NotFound,
                        new ErrorDetail("COURSE_NOT_FOUND", "Course not found"));

                if (course.InstructorId != currentUserId)
                    return Response<QuestionDto>.Fail(ErrorType.Forbidden,
                        new ErrorDetail("ACCESS_DENIED", "You can only create questions for your own courses"));

                var correctChoicesCount = dto.Choices.Count(c => c.IsCorrect);
                if (correctChoicesCount != 1)
                    return Response<QuestionDto>.Fail(ErrorType.Validation,
                        new ErrorDetail("INVALID_CORRECT_ANSWERS", "Question must have exactly one correct answer"));

                // Start transaction using repository method
                await _questionRepository.BegainTransactionAsync();

                try
                {
                    // Create question
                    var question = new Question
                    {
                        Content = dto.Content.Trim(),
                        Level = dto.Level,
                        Points = dto.Points,
                        CourseId = dto.CourseId,
                        CreatedBy = currentUserId.ToString()
                    };

                    await _questionRepository.AddAsync(question);
                    await _questionRepository.SaveChangesAsync();

                    // Create choices
                    var choices = dto.Choices.Select(choiceDto => new Choice
                    {
                        Content = choiceDto.Content.Trim(),
                        IsCorrect = choiceDto.IsCorrect,
                        QuestionId = question.Id,
                        CreatedBy = currentUserId.ToString()
                    }).ToList();

                    await _choiceRepository.AddRangeAsync(choices);
                    await _choiceRepository.SaveChangesAsync();

                    // Commit transaction using repository method
                    await _questionRepository.CommitTransactionAsync();

                    // Manual mapping
                    var instructor = await _userRepository.GetByIdAsync(course.InstructorId);
                    var usageCount = 0; // New question, no usage yet

                    var choiceDtos = choices.Select(c => new ChoiceDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        IsCorrect = c.IsCorrect,
                        QuestionId = c.QuestionId,
                        CreatedAt = c.CreatedAt
                    }).ToList();

                    var result = new QuestionDto
                    {
                        Id = question.Id,
                        Content = question.Content,
                        Level = question.Level,
                        Points = question.Points,
                        CourseId = question.CourseId,
                        CourseTitle = course.Title,
                        InstructorId = course.InstructorId,
                        InstructorName = $"{instructor.FirstName} {instructor.LastName}",
                        CreatedAt = question.CreatedAt,
                        UsageCount = usageCount,
                        Choices = choiceDtos
                    };

                    return Response<QuestionDto>.Success(result);
                }
                catch (Exception ex)
                {
                    // Rollback transaction using repository method
                    await _questionRepository.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Response<QuestionDto>.Fail(ErrorType.Critical,
                    new ErrorDetail("CREATE_QUESTION_ERROR", "Failed to create question with choices", ex.Message));
            }
        }

        public async Task<Response<QuestionDto>> UpdateQuestionAsync(int questionId, UpdateQuestionDto dto, int currentUserId)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question == null || question.IsDeleted)
                    return Response<QuestionDto>.Fail(ErrorType.NotFound,
                        new ErrorDetail("QUESTION_NOT_FOUND", "Question not found"));

                var course = await _courseRepository.GetByIdAsync(question.CourseId);
                if (course.InstructorId != currentUserId)
                    return Response<QuestionDto>.Fail(ErrorType.Forbidden,
                        new ErrorDetail("ACCESS_DENIED", "You can only update questions for your own courses"));

                var isUsedInExams = await _examQuestionRepository
                    .GetAll(eq => eq.QuestionId == questionId && !eq.IsDeleted)
                    .AnyAsync();

                if (isUsedInExams)
                    return Response<QuestionDto>.Fail(ErrorType.BusinessRule,
                        new ErrorDetail("QUESTION_IN_USE", "Cannot update question that is used in exams"));

                question.Content = dto.Content.Trim();
                question.Level = dto.Level;
                question.Points = dto.Points;
                question.UpdatedAt = DateTime.UtcNow;
                question.UpdatedBy = currentUserId.ToString();

                await _questionRepository.UpdateAsync(question);
                await _questionRepository.SaveChangesAsync();

                var result = await MapQuestionToDtoAsync(question);
                return Response<QuestionDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Response<QuestionDto>.Fail(ErrorType.Critical,
                    new ErrorDetail("UPDATE_QUESTION_ERROR", "Failed to update question", ex.Message));
            }
        }
    }
    }
}
