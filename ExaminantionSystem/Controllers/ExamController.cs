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

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<ExamVM>> UpdateExam([FromBody] UpdateExamVM model)
        {
            if (model.ExamId != 0 && model.ExamId != null)
            
                return ResponseViewModel<ExamVM>.Fail(
                GlobalErrorType.Validation,
                    new ErrorDetailViewModel("ID_MISMATCH", "ID mismatch", "Route ID and body ExamId must match", "id")
                );
            

            var updateExamInfo = _mapper.Map<UpdateExamDto>(model);
            var result = await _examService.UpdateExamAsync(updateExamInfo, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<ExamVM>>(result);
        }

        [HttpDelete("{examId}")]
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

        [HttpGet("{id}/details")]
        public async Task<ResponseViewModel<ExamWithQuestionsVM>> GetExamWithQuestions(int id)
        {
            var result = await _examService.GetExamWithQuestionsAsync(id);
            return _mapper.Map<ResponseViewModel<ExamWithQuestionsVM>>(result);
        }

        [HttpPost("{examId}/auto-generate")]
       // [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<ExamWithQuestionsVM>> AutoGenerateExamQuestions(int examId)
        {
            var result = await _examService.AutoGenerateExamQuestionsAsync(examId);
            return _mapper.Map<ResponseViewModel<ExamWithQuestionsVM>>(result);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ResponseViewModel<List<ExamVM>>> GetExamsByCourse(int courseId)
        {
            var result = await _examService.GetExamsByCourseAsync(courseId , GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<List<ExamVM>>>(result);
        }


    }
}
