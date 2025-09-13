using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Service
{
    public class ChoiceService
    {
        private readonly ChoiceRepository _choiceRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly CourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public ChoiceService(
            ChoiceRepository choiceRepository,
            QuestionRepository questionRepository,
            CourseRepository courseRepository,
            IMapper mapper)
        {
            _choiceRepository = choiceRepository;
            _questionRepository = questionRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        #region ChoiceCRUDOPeration
        public async Task<Response<List<ChoiceDto>>> CreateChoicesForQuestionAsync(List<CreateChoiceDto> choicesDto, int questionId, int currentUserId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return Response<List<ChoiceDto>>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail("Question not found"));


            // Validate exactly one correct answer
            var correctChoicesCount = choicesDto.Count(c => c.IsCorrect);
            if (correctChoicesCount != 1)
                return Response<List<ChoiceDto>>.Fail(ErrorType.INVALID_CORRECT_ANSWERS,new ErrorDetail("Question must have exactly one correct answer"));

            // Validate minimum and maximum choices
            if (choicesDto.Count < 2)
                return Response<List<ChoiceDto>>.Fail(ErrorType.MIN_CHOICES_REQUIRED,new ErrorDetail("Question must have at least 2 choices"));

            if (choicesDto.Count > 4)
                return Response<List<ChoiceDto>>.Fail(ErrorType.MAX_CHOICES_EXCEEDED,
                    new ErrorDetail("Question cannot have more than 4 choices"));

            // Validate choice content
            foreach (var choice in choicesDto)
            {
                if (string.IsNullOrWhiteSpace(choice.Text))
                    return Response<List<ChoiceDto>>.Fail(ErrorType.EMPTY_CHOICE_CONTENT,
                        new ErrorDetail("Choice content cannot be empty"));
            }

                var choices = _mapper.Map<List<Choice>>(choicesDto);
                choices.ForEach(choice =>
                {
                    choice.QuestionId = questionId;
                    choice.CreatedBy = currentUserId;
                });

                await _choiceRepository.AddRangeAsync(choices);
                await _choiceRepository.SaveChangesAsync();

                var choiceDtos = _mapper.Map<List<ChoiceDto>>(choices);
                return Response<List<ChoiceDto>>.Success(choiceDtos);

            }
        
        public async Task<Response<ChoiceDto>> UpdateChoiceAsync(UpdateChoiceDto dto, int currentUserId)
        {

            var choiceInfo = await _choiceRepository.GetAll(c => c.Id == dto.choiceId)
                 .Select(c => new
                 {
                     Choice = c,
                     IsAuthorized = c.Question.Course.InstructorId == currentUserId
                 }) .FirstOrDefaultAsync();


            if (choiceInfo == null)
                    return Response<ChoiceDto>.Fail(ErrorType.CHOICE_NOT_FOUND,
                        new ErrorDetail( "Choice not found"));

                if (!choiceInfo.IsAuthorized)
                    return Response<ChoiceDto>.Fail(ErrorType.ACCESS_DENIED,
                        new ErrorDetail("You can only update choices for your own questions"));

                // If setting this choice as correct, ensure no other correct choices exist
                if (dto.IsCorrect)
                {
                    var otherCorrectChoices = await _choiceRepository.GetAll(c => c.QuestionId == choiceInfo.Choice.QuestionId && c.Id != dto.choiceId && c.IsCorrect).ToListAsync();

                    if (otherCorrectChoices.Any())
                    {
                        // Set all other choices as incorrect
                        await _choiceRepository.UpdateAsync(c => c.QuestionId == choiceInfo.Choice.QuestionId && c.Id != dto.choiceId && c.IsCorrect,
                             s => s.SetProperty(c => c.IsCorrect, false)
                                  .SetProperty(c => c.UpdatedAt, DateTime.UtcNow)
                                  
                        );
                    }
                }

                choiceInfo.Choice.Text = dto.Text.Trim();
                choiceInfo.Choice.IsCorrect = dto.IsCorrect;
                choiceInfo.Choice.UpdatedAt = DateTime.UtcNow;

                await _choiceRepository.UpdateAsync(choiceInfo.Choice);
                await _choiceRepository.SaveChangesAsync();

                var result = _mapper.Map<ChoiceDto>(choiceInfo);

                return Response<ChoiceDto>.Success(result);
        }

        public async Task<Response<bool>> DeleteChoiceAsync(int choiceId, int currentUserId)
        {
            var choiceInfo = await _choiceRepository.GetAll()
                .Where(c => c.Id == choiceId)
                .Select(c => new
                {
                    Choice = c,
                    IsAuthorized = c.Question.Course.InstructorId == currentUserId,
                    questionId = c.QuestionId,
                    IsOnlyCorrectChoice = c.IsCorrect &&
                        c.Question.Choices.Count(other => other.Id != choiceId && other.IsCorrect) == 0
                })
                .FirstOrDefaultAsync();

            if (choiceInfo == null)
                    return Response<bool>.Fail(ErrorType.CHOICE_NOT_FOUND,
                        new ErrorDetail( "Choice not found"));


             if (!choiceInfo.IsAuthorized)
                    return Response<bool>.Fail(ErrorType.ACCESS_DENIED,
                        new ErrorDetail("You can only delete choices from your own questions"));

            var hasActiveExams = await _choiceRepository.IsChoiceRelatedWithActiveExam(choiceInfo.questionId);
            if (hasActiveExams)
                return Response<bool>.Fail(ErrorType.CHOICE_IN_ACTIVE_EXAM,
                    new ErrorDetail("Cannot delete choice that is part of an active exam"));

            // Check if this is the only correct choice
            if (choiceInfo.IsOnlyCorrectChoice)
                {
                    var otherCorrectChoices = await _choiceRepository.GetAll()
                        .Where(c => c.QuestionId == choiceInfo.questionId &&
                                   c.Id != choiceId &&
                                   c.IsCorrect)
                        .CountAsync();

                    if (otherCorrectChoices == 0)
                        return Response<bool>.Fail(ErrorType.LAST_CORRECT_CHOICE,
                            new ErrorDetail("Cannot delete the only correct choice"));
                }

                await _choiceRepository.DeleteAsync(choiceId);
                await _choiceRepository.SaveChangesAsync();

                return Response<bool>.Success(true);
            
        }
        #endregion

        public async Task<Response<List<ChoiceDto>>> GetChoicesForQuestionAsync(int questionId, int currentUserId)
        {
            // Validate question exists and user has access
            var questionInfo = await _questionRepository.GetAll(q => q.Id == questionId)
                .Select(q => new
                {
                    Question = q,
                    CourseInfo = q.Course,
                    Choices = q.Choices.Where(c => !c.IsDeleted && c.IsActive),
                    IsAuthorized = q.Course.InstructorId == currentUserId
                }).FirstOrDefaultAsync();

            if (questionInfo == null)
                return Response<List<ChoiceDto>>.Fail(
                    ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail($"Question with ID {questionId} not found"));

            if (!questionInfo.IsAuthorized)
                return Response<List<ChoiceDto>>.Fail(
                    ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only view choices for your own questions"));

            var choiceInfo = _mapper.Map<List<ChoiceDto>>(questionInfo.Choices);
            return Response<List<ChoiceDto>>.Success(choiceInfo);

        }
    }
}
