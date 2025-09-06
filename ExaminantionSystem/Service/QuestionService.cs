using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.Enums;
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
            ExamRepository examRepository,
            CourseRepository courseRepository,
            InstructorRepository instructorRepository)
        {
            _questionRepository = questionRepository;
            _choiceRepository = choiceRepository;
            _examRepository = examRepository;
            _courseRepository = courseRepository;
            _instructorRepository = instructorRepository;
        }

        public async Task<Response<QuestionDto>> CreateQuestionWithChoicesAsync(CreateQuestionDto dto, int currentUserId)
        {
            var course = await _courseRepository.GetByIdAsync(dto.CourseId);

            if (course == null)
            {
                if (course.InstructorId != currentUserId)
                    return Response<QuestionDto>.Fail(ErrorType.Forbidden,
                        new ErrorDetail("ACCESS_DENIED", "You can only create questions for your own courses"));
            }

            // Validate choices using separate function
            var validationResult = ValidateChoices(dto.Choices);
            if (!validationResult.Succeeded)
                return validationResult;

            // Create question
            var question = new Question
            {
                Content = dto.Content.Trim(),
                QuestionLevel = dto.Level,
                Mark = dto.Mark,
                CourseId = dto.CourseId,
                CreatedBy = currentUserId
            };

            await _questionRepository.AddAsync(question);
            await _questionRepository.SaveChangesAsync();

            var choicesResult = await CreateChoicesForQuestionAsync(dto.Choices, question.Id, currentUserId);


            var instructor = await _instructorRepository.GetByIdAsync(course.InstructorId);
            var choiceDtos = choicesResult.Data.Select(c => new ChoiceDto
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

        public async Task<Response<bool>> DeleteQuestionWithChoicesAsync(int questionId, int currentUserId)
        {

            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return Response<bool>.Fail(ErrorType.NotFound,
                    new ErrorDetail("QUESTION_NOT_FOUND", "Question not found"));

            var course = await _courseRepository.GetByIdAsync(question.CourseId);
            if (course.InstructorId != currentUserId)
                return Response<bool>.Fail(ErrorType.Forbidden,
                    new ErrorDetail("ACCESS_DENIED", "You can only delete questions from your own courses"));

            var isUsedInExams = await _questionRepository.IsQuestionInExamExistAsync(questionId);

            if (isUsedInExams)
                return Response<bool>.Fail(ErrorType.BusinessRule,
                    new ErrorDetail("QUESTION_IN_USE", "Cannot delete question that is used in exams"));


            try
            {
                // Soft delete question
                await _questionRepository.DeleteAsync(questionId);

                // Soft delete all choices for this question
                await _choiceRepository.DeleteRangeAsync(question.Choices);
                await _questionRepository.SaveChangesAsync();


                return Response<bool>.Success(true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public async Task<Response<QuestionDto>> UpdateQuestionWithChoicesAsync(UpdateQuestionDto dto, int currentUserId)
        {
            var question = await _questionRepository.GetByIdAsync(dto.QuestionId);
            if (question == null)
                return Response<QuestionDto>.Fail(ErrorType.NotFound,
                    new ErrorDetail("QUESTION_NOT_FOUND", "Question not found"));

            var course = await _courseRepository.GetByIdAsync(question.CourseId);
            if (course.InstructorId != currentUserId)
                return Response<QuestionDto>.Fail(ErrorType.Forbidden,
                    new ErrorDetail("ACCESS_DENIED", "You can only update questions for your own courses"));

            var isUsedInActiveExams = await _examRepository.IsQuestionInExamActiveAsync(dto.QuestionId);

            if (isUsedInActiveExams)
                return Response<QuestionDto>.Fail(ErrorType.BusinessRule,
                    new ErrorDetail("QUESTION_IN_USE", "Cannot update question that is used in exams"));

            var correctChoicesCount = dto.Choices.Count(c => c.IsCorrect);
            if (correctChoicesCount != 1)
                return Response<QuestionDto>.Fail(ErrorType.Validation,
                    new ErrorDetail("INVALID_CORRECT_ANSWERS", "Question must have exactly one correct answer"));

            // Start transaction using repository method
            //await _questionRepository.BegainTransactionAsync();

            try
            {
                // Update question
                question.Content = dto.Content.Trim();
                question.QuestionLevel = dto.Level;
                question.Mark = dto.Mark;

                await _questionRepository.UpdateAsync(question);

                // Delete existing choices
                await _choiceRepository.UpdateAsync(
                    c => c.QuestionId == dto.QuestionId && !c.IsDeleted && c.IsActive,
                    s => s.SetProperty(c => c.IsDeleted, true)
                          .SetProperty(c => c.DeletedAt, DateTime.UtcNow)
                );

                // Create new choices
                var choices = dto.Choices.Select(choiceDto => new Choice
                {
                    Text = choiceDto.Text.Trim(),
                    IsCorrect = choiceDto.IsCorrect,
                    QuestionId = dto.QuestionId,
                    CreatedBy = currentUserId,
                }).ToList();

                await _choiceRepository.AddRangeAsync(choices);
                await _questionRepository.SaveChangesAsync();

                // Commit transaction using repository method
                //await _questionRepository.CommitTransactionAsync();

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
                    Content = question.Content,
                    Level = question.QuestionLevel,
                    Mark = question.Mark,
                    CourseTitle = course.Title,
                    InstructorId = course.InstructorId,
                    InstructorName = instructor.FullName,
                    Choices = choiceDtos
                };

                return Response<QuestionDto>.Success(result);
            }
            catch (Exception ex)
            {
                // Rollback transaction using repository method
                //await _questionRepository.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task<Response<List<ChoiceDto>>> CreateChoicesForQuestionAsync(List<CreateChoiceDto> choicesDto, int questionId, int currentUserId)
        {
            try
            {
                var choices = choicesDto.Select(choiceDto => new Choice
                {
                    Text = choiceDto.Text.Trim(),
                    IsCorrect = choiceDto.IsCorrect,
                    QuestionId = questionId,
                    CreatedBy = currentUserId
                }).ToList();

                await _choiceRepository.AddRangeAsync(choices);
                await _choiceRepository.SaveChangesAsync();

                // Manual mapping
                var choiceDtos = choices.Select(c => new ChoiceDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect,
                    QuestionId = c.QuestionId,
                }).ToList();

                return Response<List<ChoiceDto>>.Success(choiceDtos);
            }
            catch (Exception ex)
            {
                return Response<List<ChoiceDto>>.Fail(ErrorType.Critical,
                    new ErrorDetail("CREATE_CHOICES_ERROR", "Failed to create choices", ex.Message));
            }
        }

        private async Task<Response<List<ChoiceDto>>> GetChoicesForQuestionAsync(int questionId, int currentUserId)
        {
            try
            {
                var choices = await _choiceRepository.GetAll()
                    .Where(c => c.QuestionId == questionId && !c.IsDeleted)
                    .OrderBy(c => c.Id)
                    .ToListAsync();

                // Manual mapping
                var choiceDtos = choices.Select(c => new ChoiceDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect,
                    QuestionId = c.QuestionId,
                }).ToList();

                return Response<List<ChoiceDto>>.Success(choiceDtos);
            }
            catch (Exception ex)
            {
                return Response<List<ChoiceDto>>.Fail(ErrorType.Critical,
                    new ErrorDetail("GET_CHOICES_ERROR", "Failed to retrieve choices", ex.Message));
            }
        }


        #region Validation Helper

        private Response<QuestionDto> ValidateChoices(List<CreateChoiceDto> choices)
        {
            // Validate exactly one correct answer
            var correctChoicesCount = choices.Count(c => c.IsCorrect);
            if (correctChoicesCount != 1)
                return Response<QuestionDto>.Fail(ErrorType.Validation,
                    new ErrorDetail("INVALID_CORRECT_ANSWERS", "Question must have exactly one correct answer"));

            // Validate minimum and maximum choices
            if (choices.Count < 2)
                return Response<QuestionDto>.Fail(ErrorType.Validation,
                    new ErrorDetail("MIN_CHOICES_REQUIRED", "Question must have at least 2 choices"));

            if (choices.Count > 4)
                return Response<QuestionDto>.Fail(ErrorType.Validation,
                    new ErrorDetail("MAX_CHOICES_EXCEEDED", "Question cannot have more than 6 choices"));

            // Validate choice content
            foreach (var choice in choices)
            {
                if (string.IsNullOrWhiteSpace(choice.Text))
                    return Response<QuestionDto>.Fail(ErrorType.Validation,
                        new ErrorDetail("EMPTY_CHOICE_CONTENT", "Choice content cannot be empty"));
            }

            return null; // Validation passed
        }
        #endregion


    }


}

