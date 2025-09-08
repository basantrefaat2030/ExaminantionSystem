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
           
                var choice = await _choiceRepository.GetByIdAsync(dto.choiceId);
                if (choice == null)
                    return Response<ChoiceDto>.Fail(ErrorType.CHOICE_NOT_FOUND,
                        new ErrorDetail( "Choice not found"));

                var question = await _questionRepository.GetByIdAsync(choice.QuestionId);
                var course = await _courseRepository.GetByIdAsync(question.CourseId);

                if (course.InstructorId != currentUserId)
                    return Response<ChoiceDto>.Fail(ErrorType.ACCESS_DENIED,
                        new ErrorDetail("You can only update choices for your own questions"));

                // If setting this choice as correct, ensure no other correct choices exist
                if (dto.IsCorrect)
                {
                    var otherCorrectChoices = await _choiceRepository.GetAll(c => c.QuestionId == choice.QuestionId && c.Id != dto.choiceId && c.IsCorrect).ToListAsync();

                    if (otherCorrectChoices.Any())
                    {
                        // Set all other choices as incorrect
                        await _choiceRepository.UpdateAsync(
                            c => c.QuestionId == choice.QuestionId &&
                                 c.Id != dto.choiceId && c.IsCorrect,
                            s => s.SetProperty(c => c.IsCorrect, false)
                                  .SetProperty(c => c.UpdatedAt, DateTime.UtcNow)
                                  
                        );
                    }
                }

                choice.Text = dto.Text.Trim();
                choice.IsCorrect = dto.IsCorrect;
                choice.UpdatedAt = DateTime.UtcNow;

                await _choiceRepository.UpdateAsync(choice);
                await _choiceRepository.SaveChangesAsync();

                var result = _mapper.Map<ChoiceDto>(choice);

                return Response<ChoiceDto>.Success(result);
        }

        public async Task<Response<bool>> DeleteChoiceAsync(int choiceId, int currentUserId)
        {
                var choice = await _choiceRepository.GetByIdAsync(choiceId);
                if (choice == null)
                    return Response<bool>.Fail(ErrorType.CHOICE_NOT_FOUND,
                        new ErrorDetail( "Choice not found"));

                var question = await _questionRepository.GetByIdAsync(choice.QuestionId);
                var course = await _courseRepository.GetByIdAsync(question.CourseId);

                if (course.InstructorId != currentUserId)
                    return Response<bool>.Fail(ErrorType.ACCESS_DENIED,
                        new ErrorDetail("You can only delete choices from your own questions"));

                // Check if this is the only correct choice
                if (choice.IsCorrect)
                {
                    var otherCorrectChoices = await _choiceRepository.GetAll()
                        .Where(c => c.QuestionId == choice.QuestionId &&
                                   c.Id != choiceId &&
                                   c.IsCorrect)
                        .CountAsync();

                    if (otherCorrectChoices == 0)
                        return Response<bool>.Fail(ErrorType.LAST_CORRECT_CHOICE,
                            new ErrorDetail("LAST_CORRECT_CHOICE", "Cannot delete the only correct choice"));
                }

                await _choiceRepository.DeleteAsync(choiceId);
                await _choiceRepository.SaveChangesAsync();

                return Response<bool>.Success(true);
            

        }



    }
}
