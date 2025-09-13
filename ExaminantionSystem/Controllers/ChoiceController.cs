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

        [HttpPost("question/{questionId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<List<ChoiceVM>>> CreateChoices(int questionId, [FromBody] List<CreateChoiceVM> model)
        {
            var createChoice = _mapper.Map<List<CreateChoiceDto>>(model);
            var result = await _choiceService.CreateChoicesForQuestionAsync(createChoice, questionId, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<List<ChoiceVM>>>(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<ChoiceVM>> UpdateChoice( [FromBody] UpdateChoiceVM model)
        {
            if ( model.ChoiceId != 0 && model.ChoiceId != null )
            
                return ResponseViewModel<ChoiceVM>.Fail(
                GlobalErrorType.Validation,
                    new ErrorDetailViewModel("ID_MISMATCH", "ID mismatch", "Route ID and body ChoiceId must match", "id")
                );
            

            var updateChoice = _mapper.Map<UpdateChoiceDto>(model);
            var result = await _choiceService.UpdateChoiceAsync(updateChoice, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<ChoiceVM>>(result);
        }

        [HttpDelete("{choiceId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<bool>> DeleteChoice(int choiceId)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _choiceService.DeleteChoiceAsync(choiceId, currentUserId);
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
