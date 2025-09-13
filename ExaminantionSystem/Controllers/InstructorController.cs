using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.ViewModels;
using ExaminantionSystem.Entities.ViewModels.Instructor;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminantionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : BaseController
    {
        private readonly InstructorService _instructorService;
        private readonly IMapper _mapper;
        public InstructorController(InstructorService instructorService , IMapper mapper) 
        { 
            _instructorService = instructorService;
            _mapper = mapper;
        }

        [HttpPost("enrollments/approve")]
        public async Task<ResponseViewModel<ApproveEnrollmentVM>> ApproveEnrollment(int enrollmentId)
        {
            var result = await _instructorService.ApproveEnrollmentAsync(enrollmentId, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<ApproveEnrollmentVM>>(result);
        }

        [HttpPost("enrollments/reject")]
        public async Task<ResponseViewModel<EnrollmentResponseVM>> RejectEnrollment([FromBody] EnrollmentRejectionVM model)
        {
            var result = await _instructorService.RejectEnrollmentAsync(model.EnrollmentId, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<EnrollmentResponseVM>>(result);
        }

        [HttpGet("enrollments/requests")]
        public async Task<ResponseViewModel<PagedResponseVM<EnrollmentResponseVM>>> GetEnrollmentRequests([FromQuery] EnrollmentRequestsQueryVM model)
        {
            var result = await _instructorService.GetCourseEnrollmentRequestsAsync(
                model.CourseId, GetCurrentUserId(), model.Status, model.PageNumber, model.PageSize);

            return _mapper.Map<ResponseViewModel<PagedResponseVM<EnrollmentResponseVM>>>(result);
        }

        #region Exam Management Endpoints

        [HttpPost("exams/manual")]
        public async Task<ResponseViewModel<ManualExamResponseVM>> CreateManualExam([FromBody] ManualExamCreationVM model)
        {
            var manualExamDto = _mapper.Map<AddManualExamDto>(model);
            var result = await _instructorService.ManualExamQuestionsAsync(manualExamDto, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<ManualExamResponseVM>>(result);
        }

        [HttpGet("courses/{courseId}/results")]
        public async Task<ResponseViewModel<PagedResponseVM<StudentExamResultVM>>> GetStudentResults(int courseId, [FromQuery] StudentResultsQueryVM model)
        {
            if (courseId != model.CourseId)
                return ResponseViewModel<PagedResponseVM<StudentExamResultVM>>.Fail(
                    GlobalErrorType.Validation,
                    new ErrorDetailViewModel("ID_MISMATCH", "ID mismatch", "Route ID and body CourseId must match", "courseId")
                );
            var result = await _instructorService.GetAllStudentResultAsync(
                model.CourseId, GetCurrentUserId(), model.PageNumber, model.PageSize);

            return _mapper.Map<ResponseViewModel<PagedResponseVM<StudentExamResultVM>>>(result);
        }
    }
}
