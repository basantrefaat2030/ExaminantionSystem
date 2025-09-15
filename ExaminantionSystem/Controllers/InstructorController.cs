using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.Enums;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.ViewModels;
using ExaminantionSystem.Entities.ViewModels.Exam;
using ExaminantionSystem.Entities.ViewModels.Instructor;
using ExaminantionSystem.Entities.ViewModels.Student;
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

        [HttpPost("ApproveEnrollment/{enrollmentId}")]
        public async Task<ResponseViewModel<ApproveEnrollmentVM>> ApproveEnrollment(int enrollmentId)
        {
            var result = await _instructorService.ApproveEnrollmentAsync(enrollmentId);
            return _mapper.Map<ResponseViewModel<ApproveEnrollmentVM>>(result);
        }

        [HttpPost("RejectEnrollment/{enrollmentId}")]
        public async Task<ResponseViewModel<RejectEnrollmentVM>> RejectEnrollment(int enrollmentId)
        {
            var result = await _instructorService.RejectEnrollmentAsync(enrollmentId);
            return _mapper.Map<ResponseViewModel<RejectEnrollmentVM>>(result);
        }

        [HttpGet("GetEnrollmentRequests")]
        public async Task<ResponseViewModel<PagedResponseViewModel<EnrollmentVM>>> GetEnrollmentRequests( 
            [FromQuery] RequestStatus? status, 
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _instructorService.GetCourseEnrollmentRequestsAsync(GetCurrentUserId() ,status, pageNumber,pageSize);

            return _mapper.Map<ResponseViewModel<PagedResponseViewModel<EnrollmentVM>>>(result);
        }


        [HttpPost("CreateManualExam")]
        public async Task<ResponseViewModel<ManualExamVM>> CreateManualExam([FromBody] AddManualExamVM examModel)
        {
            var manualExamDto = _mapper.Map<AddManualExamDto>(examModel);
            var result = await _instructorService.ManualExamQuestionsAsync(manualExamDto, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<ManualExamVM>>(result);
        }

        [HttpGet("GetStudentResults")]
        public async Task<ResponseViewModel<PagedResponseViewModel<StudentResultVM>>> GetStudentResults([FromQuery] int courseId, [FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 10)
        {
            if (courseId != 0 && courseId != null)
                return ResponseViewModel<PagedResponseViewModel<StudentResultVM>>.Fail(
                    GlobalErrorType.Validation,
                    new ErrorDetailViewModel("", "Route ID and body CourseId must match", "courseId"));

                 var result = await _instructorService.GetAllStudentResultAsync(
                courseId, pageNumber, pageSize);

            return _mapper.Map<ResponseViewModel<PagedResponseViewModel<StudentResultVM>>>(result);
        }
    }
}

