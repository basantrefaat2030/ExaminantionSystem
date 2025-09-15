using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Student;
using ExaminantionSystem.Entities.Enums;
using ExaminantionSystem.Entities.ViewModels.Exam;
using ExaminantionSystem.Entities.ViewModels.Student;
using ExaminantionSystem.Entities.ViewModels;
using ExaminantionSystem.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminantionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : BaseController
    {
        private readonly StudentService _studentService;
        private readonly IMapper _mapper;

        public StudentController(StudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpPost("CourseEnroll/{courseId}")]
        public async Task<ResponseViewModel<StudentEnrollmentVM>> CourseEnroll(int courseId)
        {
            var result = await _studentService.RequestEnrollmentAsync(courseId, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<StudentEnrollmentVM>>(result);
        }

        [HttpDelete("CancelEnrollment/{enrollmentId}")]
        public async Task<ResponseViewModel<bool>> CancelEnrollment(int enrollmentId)
        {
            var result = await _studentService.CancelEnrollmentRequestAsync(enrollmentId, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpGet("enrollments")]
        public async Task<ResponseViewModel<PagedResponseViewModel<StudentEnrollmentVM>>> GetEnrollments(
            [FromQuery] RequestStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var result = await _studentService.GetStudentEnrollmentsAsync(GetCurrentUserId(), status, page, size);
            return _mapper.Map<ResponseViewModel<PagedResponseViewModel<StudentEnrollmentVM>>>(result);
        }

        [HttpPost("StartExam/{examId}")]
        public async Task<ResponseViewModel<StudentExamVM>> StartExam(int examId)
        {
            var result = await _studentService.StartExamAsync(examId, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<StudentExamVM>>(result);
        }

        [HttpPost("SubmitExam/{examId}")]
        public async Task<ResponseViewModel<ExamResultVM>> SubmitExam(int examId, [FromBody] SubmitStudentAnswerVM answers)
        {
            var answerDto = _mapper.Map<SubmitStudentAnswerDto>(answers);
            answerDto.examId = examId;

            var result = await _studentService.SubmitExamAnswersAsync(answerDto, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<ExamResultVM>>(result);
        }
    }
}
