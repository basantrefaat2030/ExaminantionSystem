using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.ViewModels;
using ExaminantionSystem.Entities.ViewModels.Course;
using ExaminantionSystem.Entities.ViewModels.Question;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExaminantionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : BaseController
    {
        private readonly CourseService _courseService;
        private readonly IMapper _mapper;
        public CourseController(CourseService courseService, IMapper mapper)
        {
            _courseService = courseService;
            _mapper = mapper;
        }

        [HttpPost]
        //[Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<CourseVM>> CreateCourse([FromBody] CreateCourseVM model)
        {
            var currentUserId = GetCurrentUserId();
            var createCourse = _mapper.Map<CreateCourseDto>(model);
            var result = await _courseService.CreateCourseAsync(createCourse, currentUserId);
            return _mapper.Map<ResponseViewModel<CourseVM>>(result);
        }

        [HttpPut("{id}")]
       // [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<CourseVM>> UpdateCourse( [FromBody] UpdateCourseVM model)
        {
            var currentUserId = GetCurrentUserId();
            var updatechoice = _mapper.Map<UpdateCourseDto>(model);
            var result = await _courseService.UpdateCourseAsync(updatechoice, currentUserId);
            return _mapper.Map<ResponseViewModel<CourseVM>>(result);
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<bool>> DeleteCourse(int id)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _courseService.DeleteCourseAsync(id, currentUserId);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpGet]
        public async Task<ResponseViewModel<PagedResponseViewModel<CourseInformationVM>>> GetCourses(
            [FromQuery] int? instructorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _courseService.GetPaginatedCoursesAsync(instructorId, pageNumber, pageSize);
            return _mapper.Map<ResponseViewModel<PagedResponseViewModel<CourseInformationVM>>>(result);
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<CourseDetailsVM>> GetCourseDetails(int id)
        {
            var result = await _courseService.GetCourseDetailsAsync(id);
            return _mapper.Map<ResponseViewModel<CourseDetailsVM>>(result);
        }

        [HttpGet("{courseId}/question-pool")]
        //[Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<List<QuestionPoolVM>>> GetQuestionPool(int courseId)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _courseService.GetQuestionPoolByCourseAsync(courseId, currentUserId);
            return _mapper.Map<ResponseViewModel<List<QuestionPoolVM>>>(result);
        }

        [HttpGet("instructor/my-courses")]
        //[Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<PagedResponseViewModel<CourseInformationVM>>> GetMyCourses(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _courseService.GetPaginatedCoursesAsync(currentUserId, pageNumber, pageSize);
            return _mapper.Map<ResponseViewModel<PagedResponseViewModel<CourseInformationVM>>>(result);
        }

    }


}



