using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.ViewModels;
using ExaminantionSystem.Entities.ViewModels.Choice;
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
            var createCourse = _mapper.Map<CreateCourseDto>(model);
            var result = await _courseService.CreateCourseAsync(createCourse);
            return _mapper.Map<ResponseViewModel<CourseVM>>(result);
        }

        [HttpPut("{courseId}")]
       // [Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<CourseVM>> UpdateCourse(int courseId , [FromBody] UpdateCourseVM model)
        {
            if (courseId != 0 && courseId != null)

                return ResponseViewModel<CourseVM>.Fail(
                GlobalErrorType.Validation,
                    new ErrorDetailViewModel("ID_VALIDATION", "choiceId must has a value !", "courseId")
                );

            var updatechoice = _mapper.Map<UpdateCourseDto>(model);
            updatechoice.courseId = courseId;
            var result = await _courseService.UpdateCourseAsync(updatechoice, GetCurrentUserId());
            return _mapper.Map<ResponseViewModel<CourseVM>>(result);
        }

        [HttpDelete("{courseId}")]
        //[Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<bool>> DeleteCourse(int courseId)
        {
            if (courseId != 0 && courseId != null)

                return ResponseViewModel<bool>.Fail(
                GlobalErrorType.Validation,
                    new ErrorDetailViewModel("ID_VALIDATION", "choiceId must has a value !", "courseId")
                );
            var result = await _courseService.DeleteCourseAsync(courseId, GetCurrentUserId());
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

        [HttpGet("{courseId}")]
        public async Task<ResponseViewModel<CourseDetailsVM>> GetCourseDetails(int courseId)
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
            var result = await _courseService.GetPaginatedCoursesAsync(GetCurrentUserId(), pageNumber, pageSize);
            return _mapper.Map<ResponseViewModel<PagedResponseViewModel<CourseInformationVM>>>(result);
        }

    }


}



