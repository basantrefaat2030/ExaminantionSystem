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
        private readonly InstructorRepository _instructorRepository;

        public QuestionService(QuestionRepository questionRepository,
            ChoiceRepository choiceRepository,
            ExamRepository ExamRepository,
            CourseRepository courseRepository,
            InstructorRepository instructorRepository)
        {
            _questionRepository = questionRepository;
            _choiceRepository = choiceRepository;
            _examRepository = _examRepository;
            _courseRepository = courseRepository;
            _instructorRepository = instructorRepository;
        }

        public async Task<Response<QuestionDto>> CreateQuestionWithChoicesAsync(CreateQuestionDto dto, int currentUserId)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(dto.CourseId);

                if (course == null)
                {
                    if (course.InstructorId != currentUserId)
                        return Response<QuestionDto>.Fail(ErrorType.Forbidden,
                            new ErrorDetail("ACCESS_DENIED", "You can only create questions for your own courses"));
                }

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
                        QuestionLevel = dto.Level,
                        Mark = dto.Mark,
                        CourseId = dto.CourseId,
                        CreatedBy = 2
                    };

                    await _questionRepository.AddAsync(question);
                    await _questionRepository.SaveChangesAsync();

                    // Create choices
                    var choices = dto.Choices.Select(choiceDto => new Choice
                    {
                        Text = choiceDto.Text.Trim(),
                        IsCorrect = choiceDto.IsCorrect,
                        QuestionId = question.Id,
                        CreatedBy = 2
                    }).ToList();

                    await _choiceRepository.AddRangeAsync(choices);
                    await _choiceRepository.SaveChangesAsync();

                    // Commit transaction using repository method
                    await _questionRepository.CommitTransactionAsync();

                    var instructor = await _instructorRepository.GetByIdAsync(course.InstructorId);
                    var choiceDtos = choices.Select(c => new ChoiceDto
                    {
                        Id = c.Id,
                        Text = c.Text,
                        IsCorrect = c.IsCorrect,
                        QuestionId = c.QuestionId,
                    }).ToList();

                    var result = new QuestionDto
                    {
                        Id = question.Id,
                        QuestionContent = question.Content,
                        Level = question.QuestionLevel,
                        Mark = question.Mark,
                        CourseName = course.Title,
                        InstructorId = course.InstructorId,
                        InstructorName = instructor?.FullName,
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

        public async Task<Response<bool>> DeleteQuestionWithChoicesAsync(int questionId, int currentUserId)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question == null || question.IsDeleted)
                    return Response<bool>.Fail(ErrorType.NotFound,
                        new ErrorDetail("QUESTION_NOT_FOUND", "Question not found"));

                var course = await _courseRepository.GetByIdAsync(question.CourseId);
                if (course.InstructorId != currentUserId)
                    return Response<bool>.Fail(ErrorType.Forbidden,
                        new ErrorDetail("ACCESS_DENIED", "You can only delete questions from your own courses"));

                var isUsedInExams = await _examQuestionRepository
                    .GetAll(eq => eq.QuestionId == questionId && !eq.IsDeleted)
                    .AnyAsync();

                if (isUsedInExams)
                    return Response<bool>.Fail(ErrorType.BusinessRule,
                        new ErrorDetail("QUESTION_IN_USE", "Cannot delete question that is used in exams"));

                // Start transaction using repository method
                await _questionRepository.BegainTransactionAsync();

                try
                {
                    // Soft delete question
                    await _questionRepository.DeleteAsync(questionId);

                    // Soft delete all choices for this question
                    await _choiceRepository.UpdateRangeAsync(
                        c => c.QuestionId == questionId && !c.IsDeleted,
                        s => s.SetProperty(c => c.IsDeleted, true)
                              .SetProperty(c => c.DeletedAt, DateTime.UtcNow)
                    );

                    await _questionRepository.SaveChangesAsync();

                    // Commit transaction using repository method
                    await _questionRepository.CommitTransactionAsync();

                    return Response<bool>.Success(true);
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
                return Response<bool>.Fail(ErrorType.Critical,
                    new ErrorDetail("DELETE_QUESTION_ERROR", "Failed to delete question with choices", ex.Message));
            }
        }

        public async Task<Response<QuestionDto>> UpdateQuestionWithChoicesAsync(int questionId, UpdateQuestionWithChoicesDto dto, int currentUserId)
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

                var correctChoicesCount = dto.Choices.Count(c => c.IsCorrect);
                if (correctChoicesCount != 1)
                    return Response<QuestionDto>.Fail(ErrorType.Validation,
                        new ErrorDetail("INVALID_CORRECT_ANSWERS", "Question must have exactly one correct answer"));

                // Start transaction using repository method
                await _questionRepository.BegainTransactionAsync();

                try
                {
                    // Update question
                    question.Content = dto.Content.Trim();
                    question.Level = dto.Level;
                    question.Points = dto.Points;
                    question.UpdatedAt = DateTime.UtcNow;
                    question.UpdatedBy = currentUserId.ToString();

                    await _questionRepository.UpdateAsync(question);

                    // Delete existing choices
                    await _choiceRepository.UpdateRangeAsync(
                        c => c.QuestionId == questionId && !c.IsDeleted,
                        s => s.SetProperty(c => c.IsDeleted, true)
                              .SetProperty(c => c.DeletedAt, DateTime.UtcNow)
                    );

                    // Create new choices
                    var choices = dto.Choices.Select(choiceDto => new Choice
                    {
                        Content = choiceDto.Content.Trim(),
                        IsCorrect = choiceDto.IsCorrect,
                        QuestionId = questionId,
                        CreatedBy = currentUserId.ToString()
                    }).ToList();

                    await _choiceRepository.AddRangeAsync(choices);
                    await _questionRepository.SaveChangesAsync();

                    // Commit transaction using repository method
                    await _questionRepository.CommitTransactionAsync();

                    // Manual mapping
                    var instructor = await _userRepository.GetByIdAsync(course.InstructorId);
                    var usageCount = await _examQuestionRepository
                        .GetAll(eq => eq.QuestionId == questionId && !eq.IsDeleted)
                        .CountAsync();

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
                    new ErrorDetail("UPDATE_QUESTION_ERROR", "Failed to update question with choices", ex.Message));
            }
        }
}

