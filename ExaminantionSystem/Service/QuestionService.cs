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

        #region QuesionCRUDOperation
        public async Task<Response<QuestionDto>> CreateQuestionAsync(CreateQuestionDto dto, int currentUserId)
        {
            var courseInfo = await _courseRepository.GetAll(c => c.Id == dto.CourseId)
                .Select(c => new { IsAuthorized =  c.InstructorId == currentUserId })
                .FirstOrDefaultAsync();

            if (courseInfo == null)
                return Response<QuestionDto>.Fail(ErrorType.COURSE_NOT_FOUND,
                    new ErrorDetail("Course not found"));

            if (!courseInfo.IsAuthorized)
                    return Response<QuestionDto>.Fail(ErrorType.ACCESS_DENIED,
                        new ErrorDetail("You can only create questions for your own courses"));
            

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

            var questionInfo = await _questionRepository.GetAll()
                .Where(q => q.Id == questionId && !q.IsDeleted)
                .Select(q => new
                {
                    Question = q,
                    IsAuthorized = q.Course.InstructorId == currentUserId,

                }).FirstOrDefaultAsync();

            if (questionInfo == null)
                return Response<bool>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail( "Question not found"));

            if (!questionInfo.IsAuthorized)
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


        public async Task<Response<QuestionDto>> UpdateQuestionAsync(UpdateQuestionDto dto, int currentUserId)
        {

            var questionInfo = await _questionRepository.GetAll(q => q.Id == dto.QuestionId)
                .Select(q => new
                {
                    Question = q,
                    IsAuthorized = q.Course.InstructorId == currentUserId,
                    
                }).FirstOrDefaultAsync();
            
            if (questionInfo == null)
                return Response<QuestionDto>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail("Question not found"));

            if (!questionInfo.IsAuthorized)
                return Response<QuestionDto>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only update questions for your own courses"));

            var isUsedInActiveExams = await _examRepository.IsQuestionInExamActiveAsync(dto.QuestionId);

            if (isUsedInActiveExams)
                return Response<QuestionDto>.Fail(ErrorType.QUESTION_IN_USE,
                    new ErrorDetail("Cannot update question that is used in exams"));

                // Update question
                questionInfo.Question.Content = dto.Content.Trim();
                questionInfo.Question.QuestionLevel = dto.Level;
                questionInfo.Question.Mark = dto.Mark;

                await _questionRepository.UpdateAsync(questionInfo.Question);
                await _questionRepository.SaveChangesAsync();

                 var result = _mapper.Map<QuestionDto>(questionInfo.Question);  

                return Response<QuestionDto>.Success(result);
            
        }

        public async Task<Response<List<ChoiceDto>>> CreateChoicesForQuestionAsync(List<CreateChoiceDto> choicesDto, int questionId, int currentUserId)
        {
            var questionInfo = await _questionRepository.GetAll(q => q.Id == questionId)
                .Select(q => new { IsAuthorized = q.Course.InstructorId == currentUserId })
                .FirstOrDefaultAsync();

            if (questionInfo == null)
                return Response<List<ChoiceDto>>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail("Question not found"));


            if (!questionInfo.IsAuthorized)
                return Response<List<ChoiceDto>>.Fail(ErrorType.ACCESS_DENIED,
                    new ErrorDetail("You can only add choices to your own questions"));

            var correctChoicesCount = choicesDto.Count(c => c.IsCorrect);
            if (correctChoicesCount != 1)
                return Response<List<ChoiceDto>>.Fail(ErrorType.INVALID_CORRECT_ANSWERS,
                    new ErrorDetail("Question must have exactly one correct answer"));

            // Validate minimum and maximum choices
            if (choicesDto.Count < 2)
                return Response<List<ChoiceDto>>.Fail(ErrorType.MIN_CHOICES_REQUIRED,
                    new ErrorDetail("Question must have at least 2 choices"));

            if (choicesDto.Count > 4)
                return Response<List<ChoiceDto>>.Fail(ErrorType.MAX_CHOICES_EXCEEDED,
                    new ErrorDetail("Question cannot have more than 4 choices"));

            if (choicesDto.Any(choice => string.IsNullOrWhiteSpace(choice.Text)))
                return Response<List<ChoiceDto>>.Fail(ErrorType.EMPTY_CHOICE_CONTENT,
                    new ErrorDetail("Choice content cannot be empty"));

                var choices = _mapper.Map<List<Choice>>(choicesDto);
                choices.ForEach(choice =>
                {
                    choice.QuestionId = questionId;
                    choice.CreatedBy = currentUserId;
                });

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

                // Manual mapping
               //var choices = await _choiceRepository.GetAll()
               // .Where(c => c.QuestionId == questionId && !c.IsDeleted)
               // .OrderBy(c => c.Id)
               // .ProjectTo<ChoiceDto>(_mapper.ConfigurationProvider)
               // .ToListAsync();

        //        return Response<List<ChoiceDto>>.Success(choiceDtos);

        //}


    }
    #endregion


}

