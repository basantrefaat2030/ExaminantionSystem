using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.ViewModels.Choice;
using ExaminantionSystem.Entities.ViewModels;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace ExaminantionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChoiceController : BaseController
    {
        private readonly ChoiceService _choiceService;
        private readonly IMapper _mapper;
        public ChoiceController(ChoiceService choiceService, IMapper mapper) 
        {  
            _choiceService = choiceService;
            _mapper = mapper;
        }

        [HttpPost("CreateChoices/{questionId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<List<ChoiceVM>>> CreateChoices(int questionId, [FromBody] List<CreateChoiceVM> model)
        {
            var createChoice = _mapper.Map<List<CreateChoiceDto>>(model);
         
            var result = await _choiceService.CreateChoicesForQuestionAsync(createChoice, questionId, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<List<ChoiceVM>>>(result);
        }

        [HttpPut("{choiceId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<ChoiceVM>> UpdateChoice(int choiceId , [FromBody] UpdateChoiceVM model)
        {
            if ( choiceId != 0 || choiceId != null)
            
                return ResponseViewModel<ChoiceVM>.Fail(
                GlobalErrorType.Validation,
                    new ErrorDetailViewModel("ID_VALIDATION", "choiceId must has a value !", "choiceId")
                );
            

            var updateChoice = _mapper.Map<UpdateChoiceDto>(model);
            updateChoice.choiceId = choiceId;
            
            var result = await _choiceService.UpdateChoiceAsync(updateChoice);
            return _mapper.Map<ResponseViewModel<ChoiceVM>>(result);
        }

        [HttpDelete("{choiceId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<bool>> DeleteChoice(int choiceId)
        {
            var result = await _choiceService.DeleteChoiceAsync(choiceId);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        //[HttpGet("question/{questionId}")]
        //[Authorize(Roles = "Instructor")]
        //public async Task<ResponseViewModel<List<ChoiceVM>>> GetChoicesByQuestion(int questionId)
        //{
        //    var currentUserId = GetCurrentUserId();
        //    // This would be a new method in your service
        //    var result = await _choiceService.GetChoicesByQuestionAsync(questionId, currentUserId);
        //    return _mapper.Map<ResponseViewModel<List<ChoiceVM>>>(result);
        //}
    }
}
