using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.Enums;
using ExaminantionSystem.Entities.Enums.Errors;
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
        //private readonly InstructorRepository _instructorRepository;
        private readonly IMapper _mapper;

        public QuestionService(QuestionRepository questionRepository,
            ChoiceRepository choiceRepository,
            ExamRepository examRepository,
            CourseRepository courseRepository,
            IMapper mapper)
        {
            _questionRepository = questionRepository;
            _choiceRepository = choiceRepository;
            _examRepository = examRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<Response<QuestionDto>> CreateQuestionAsync(CreateQuestionDto dto, int currentUserId)
        {
            var course = await _courseRepository.GetByIdAsync(dto.CourseId);

            if (course == null)
            {
                if (course.InstructorId != currentUserId)
                    return Response<QuestionDto>.Fail(ErrorType.ACCESS_DENIED,
                        new ErrorDetail("You can only create questions for your own courses"));
            }

            var question = _mapper.Map<Question>(dto);
            question.CreatedBy = currentUserId;

            await _questionRepository.AddAsync(question);
            await _questionRepository.SaveChangesAsync();

            //var choicesResult = await CreateChoicesForQuestionAsync(dto.Choices, question.Id, currentUserId);


            //var instructor = await _instructorRepository.GetByIdAsync(course.InstructorId);
            //var choiceDtos = choicesResult.Data.Select(c => new ChoiceDto
            //{
            //    Id = c.Id,
            //    Text = c.Text,
            //    IsCorrect = c.IsCorrect,
            //    QuestionId = c.QuestionId,
            //}).ToList();


          var result = _mapper.Map<QuestionDto>(question);

            return Response<QuestionDto>.Success(result);
        }

        public async Task<Response<bool>> DeleteQuestionAsync(int questionId, int currentUserId)
        {

            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return Response<bool>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail( "Question not found"));

            var course = await _courseRepository.GetByIdAsync(question.CourseId);
            if (course.InstructorId != currentUserId)
                return Response<bool>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only delete questions from your own courses"));

            var isUsedInExams = await _questionRepository.IsQuestionInExamExistAsync(questionId);

            if (isUsedInExams)
                return Response<bool>.Fail(ErrorType.QUESTION_IN_USE,
                    new ErrorDetail("Cannot delete question that is used in exams"));

                // Soft delete question
                await _questionRepository.DeleteAsync(questionId);
                await _questionRepository.SaveChangesAsync();


                return Response<bool>.Success(true);

        }



        public async Task<Response<QuestionDto>> UpdateQuestionWithChoicesAsync(UpdateQuestionDto dto, int currentUserId)
        {
            var question = await _questionRepository.GetByIdAsync(dto.QuestionId);
            if (question == null)
                return Response<QuestionDto>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail("Question not found"));

            var course = await _courseRepository.GetByIdAsync(question.CourseId);
            if (course.InstructorId != currentUserId)
                return Response<QuestionDto>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only update questions for your own courses"));

            var isUsedInActiveExams = await _examRepository.IsQuestionInExamActiveAsync(dto.QuestionId);

            if (isUsedInActiveExams)
                return Response<QuestionDto>.Fail(ErrorType.QUESTION_IN_USE,
                    new ErrorDetail("Cannot update question that is used in exams"));

                // Update question
                question.Content = dto.Content.Trim();
                question.QuestionLevel = dto.Level;
                question.Mark = dto.Mark;

                await _questionRepository.UpdateAsync(question);
                await _questionRepository.SaveChangesAsync();

                 var result = _mapper.Map<QuestionDto>(question);  

                return Response<QuestionDto>.Success(result);
            
        }

        public async Task<Response<List<ChoiceDto>>> CreateChoicesForQuestionAsync(List<CreateChoiceDto> choicesDto, int questionId, int currentUserId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return Response<List<ChoiceDto>>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail("Question not found"));

            var choices = choicesDto.Select(choiceDto => new Choice
                {
                    Text = choiceDto.Text.Trim(),
                    IsCorrect = choiceDto.IsCorrect,
                    QuestionId = questionId,
                    CreatedBy = currentUserId
                }).ToList();

                await _choiceRepository.AddRangeAsync(choices);
                await _choiceRepository.SaveChangesAsync();

                var result = _mapper.Map<List<ChoiceDto>>(choices);

                return Response<List<ChoiceDto>>.Success(result);
            
        }

        //public async Task<Response<List<ChoiceDto>>> GetChoicesForQuestionAsync(int questionId, int currentUserId)
        //{
        //    var question = await _questionRepository.GetByIdAsync(questionId);
        //    if (question == null)
        //        return Response<List<ChoiceDto>>.Fail(ErrorType.QUESTION_NOT_FOUND,
        //            new ErrorDetail("Question not found"));

        //    var choices = await _choiceRepository.GetAll()
        //            .Where(c => c.QuestionId == questionId && !c.IsDeleted)
        //            .OrderBy(c => c.Id)
        //            .ToListAsync();

        //        // Manual mapping
        //        var choiceDtos = choices.Select(c => new ChoiceDto
        //        {
        //            Id = c.Id,
        //            Text = c.Text,
        //            IsCorrect = c.IsCorrect,
        //            QuestionId = c.QuestionId,
        //        }).ToList();

        //        return Response<List<ChoiceDto>>.Success(choiceDtos);

        //}


    }


}

