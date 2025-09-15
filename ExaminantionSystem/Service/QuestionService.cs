using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        public async Task<Response<QuestionDto>> CreateQuestionAsync(CreateQuestionDto dto , int currentUserId)
        {
            var courseInfo = await _courseRepository.GetByIdAsync(dto.CourseId);

            if (courseInfo == null)
                return Response<QuestionDto>.Fail(ErrorType.COURSE_NOT_FOUND,
                    new ErrorDetail("Course not found"));

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

        public async Task<Response<bool>> DeleteQuestionAsync(int questionId)
        {

            var questionInfo = await _questionRepository.GetWithTrancking(q => q.Id == questionId)
                .Select(q => new
                {
                    Question = q,
                    //IsAuthorized = q.Course.InstructorId == currentUserId,

                }).FirstOrDefaultAsync();

            if (questionInfo == null)
                return Response<bool>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail( "Question not found"));

            var isUsedInExams = await _questionRepository.IsQuestionInExamExistAsync(questionId);

            if (isUsedInExams)
                return Response<bool>.Fail(ErrorType.QUESTION_IN_USE,
                    new ErrorDetail("Cannot delete question that is used in exams"));

                // Soft delete question
                await _questionRepository.DeleteAsync(questionId);
                await _questionRepository.SaveChangesAsync();


                return Response<bool>.Success(true);

        }


        public async Task<Response<QuestionDto>> UpdateQuestionAsync(UpdateQuestionDto dto)
        {

            var questionInfo = await _questionRepository.GetWithTrancking(q => q.Id == dto.QuestionId)
                .Select(q => new
                {
                    Question = q,
                    //IsAuthorized = q.Course.InstructorId == currentUserId,
                    
                }).FirstOrDefaultAsync();
            
            if (questionInfo == null)
                return Response<QuestionDto>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail("Question not found"));

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

        public async Task<Response<List<ChoiceDto>>> CreateChoicesForQuestionAsync(List<CreateChoiceDto> choicesDto, int questionId , int currentUserId)
        {
            var questionInfo = await _questionRepository.GetByIdAsync(questionId);

            if (questionInfo == null)
                return Response<List<ChoiceDto>>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail("Question not found"));

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

        public async Task<Response<bool>> DeleteAllChoicesForQuestionAsync(int questionId)
        {
            // First verify the question exists and user has access
            var questionInfo = await  _questionRepository.GetAll(a => a.Id == questionId).Select(q => new 
            {
                courseId = q.CourseId,
                CourseInfo = q.Course,
                //IsAuthorized = q.Course.InstructorId == currentUserId,
                choices = q.Choices.Where(a => a.IsActive && !a.IsDeleted)

            
            }).FirstOrDefaultAsync();

            if (questionInfo == null)
                return Response<bool>.Fail(ErrorType.QUESTION_NOT_FOUND,
                    new ErrorDetail("QUESTION_NOT_FOUND", "Question not found"));

            if (questionInfo.CourseInfo == null)
                return Response<bool>.Fail(ErrorType.COURSE_NOT_FOUND,
                    new ErrorDetail("COURSE_NOT_FOUND", "Course not found"));

            // Check if any choice is used in active exams
            var hasActiveExams = await _examRepository.IsQuestionInExamActiveAsync(questionId);
            if (hasActiveExams)
                return Response<bool>.Fail(ErrorType.CHOICE_IN_ACTIVE_EXAM,
                    new ErrorDetail("CHOICE_IN_ACTIVE_EXAM", "Cannot delete choices that are part of active exams"));

                // Soft delete all choices for this question
                await _choiceRepository.DeleteRangeAsync(questionInfo.choices);
                await _choiceRepository.SaveChangesAsync();

                return Response<bool>.Success(true);

        }


    }
    #endregion


}

