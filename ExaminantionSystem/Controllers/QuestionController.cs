using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.ViewModels.Question;
using ExaminantionSystem.Entities.ViewModels;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExaminantionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : BaseController
    {
        private readonly QuestionService _questionService;
        private readonly ChoiceService _choiceService;
        private readonly IMapper _mapper;

        public QuestionController(QuestionService questionService, ChoiceService choiceService, IMapper mapper)
        {
            _questionService = questionService;
            _choiceService = choiceService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<QuestionVM>> CreateQuestion([FromBody] CreateQuestionVM model)
        {
            var createQuestion = _mapper.Map<CreateQuestionDto>(model);
            var result = await _questionService.CreateQuestionAsync(createQuestion, GetCurrentUserId());

            // After creating question, create choices
            if (result.Succeeded && model.Choices.Any())
            {
                var choicesResult = await _choiceService.CreateChoicesForQuestionAsync(
                    _mapper.Map<List<CreateChoiceDto>>(model.Choices),
                    result.Data.Id,
                    GetCurrentUserId()
                );

                if (!choicesResult.Succeeded)
                    return _mapper.Map<ResponseViewModel<QuestionVM>>(choicesResult);
                
            }

            return _mapper.Map<ResponseViewModel<QuestionVM>>(result);
        }

        [HttpPut("{questionId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<QuestionVM>> UpdateQuestion([FromBody] UpdateQuestionVM model)
        {
            if (model.QuestionId != 0 && model.QuestionId != null)
            
                return ResponseViewModel<QuestionVM>.Fail(
                    GlobalErrorType.Validation,
                    new ErrorDetailViewModel("ID Not Found ", "id")
                );
            
            var dto = _mapper.Map<UpdateQuestionDto>(model);
            var result = await _questionService.UpdateQuestionAsync(dto, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<QuestionVM>>(result);
        }

        [HttpDelete("{questionId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<bool>> DeleteQuestion(int questionId)
        {

            // First delete all choices for this question using the new bulk method
            var choicesResult = await _questionService.DeleteAllChoicesForQuestionAsync(questionId, GetCurrentUserId());
            if (!choicesResult.Succeeded)
                return _mapper.Map<ResponseViewModel<bool>>(choicesResult);
            

            // Then delete the question
            var questionResult = await _questionService.DeleteQuestionAsync(questionId, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<bool>>(questionResult);
        }

        [HttpGet("{questionId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<QuestionVM>> GetQuestion(int questionId)
        {
            // This would be a new method in your service
            var result = await _choiceService.GetChoicesForQuestionAsync(questionId, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<QuestionVM>>(result);
        }


    }
}
