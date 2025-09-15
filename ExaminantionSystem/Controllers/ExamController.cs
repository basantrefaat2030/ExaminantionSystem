using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.ViewModels.Exam;
using ExaminantionSystem.Entities.ViewModels;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExaminantionSystem.Entities.ViewModels.Choice;

namespace ExaminantionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : BaseController
    {
        private readonly ExamService _examService;
        private readonly IMapper _mapper;
        public ExamController(ExamService examService, IMapper mapper)
        {
            _examService = examService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<ExamVM>> CreateExam([FromBody] CreateExamVM model)
        {
            var createExamDto = _mapper.Map<CreateExamDto>(model);
            var result = await _examService.CreateExamAsync(createExamDto, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<ExamVM>>(result);
        }

        [HttpPut("UpdateExam/{examId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<ExamVM>> UpdateExam(int examId ,[FromBody] UpdateExamVM model)
        {
            if (examId != 0 && examId != null)

                return ResponseViewModel<ExamVM>.Fail(GlobalErrorType.Validation,
                   new ErrorDetailViewModel("ID_VALIDATION", "choiceId must has a value !", "choiceId"));


            var updateExamInfo = _mapper.Map<UpdateExamDto>(model);
            updateExamInfo.ExamId = examId;

            var result = await _examService.UpdateExamAsync(updateExamInfo, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<ExamVM>>(result);
        }

        [HttpDelete("DeleteExam/{examId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<bool>> DeleteExam(int examId)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _examService.DeleteExamAsync(examId, currentUserId);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        //[HttpGet("{examId}")]
        //public async Task<ResponseViewModel<ExamVM>> GetExam(int examId)
        //{
        //    var result = await _examService.GetExamByIdAsync(id);
        //    return _mapper.Map<ResponseViewModel<ExamVM>>(result);
        //}

        [HttpGet("{examId}/ExamDetails")]
        public async Task<ResponseViewModel<ExamWithQuestionsVM>> GetExamWithQuestions(int examId)
        {
            var result = await _examService.GetExamWithQuestionsAsync(examId);
            return _mapper.Map<ResponseViewModel<ExamWithQuestionsVM>>(result);
        }

        [HttpPost("{examId}/auto-generate")]
       // [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<ExamWithQuestionsVM>> AutoGenerateExamQuestions(int examId)
        {
            var result = await _examService.AutoGenerateExamQuestionsAsync(examId);
            return _mapper.Map<ResponseViewModel<ExamWithQuestionsVM>>(result);
        }

        [HttpGet("GetExam/{courseId}")]
        public async Task<ResponseViewModel<List<ExamVM>>> GetExamsByCourse(int courseId)
        {
            var result = await _examService.GetExamsByCourseAsync(courseId);
            return _mapper.Map<ResponseViewModel<List<ExamVM>>>(result);
        }


    }
}
