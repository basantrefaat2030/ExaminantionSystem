using AutoMapper;
using ExaminantionSystem.Entities.Dtos.Courcse;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.ViewModels;
using ExaminantionSystem.Entities.ViewModels.Course;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminantionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly IMapper _mapper;
        public CourseController(CourseService courseService , IMapper mapper) 
        { 
            _courseService = courseService;
            _mapper = mapper;
        }

        [HttpPost]
        //[Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<CourseVM>> CreateCourse([FromBody] CreateCourseVM viewModel)
        {
            var currentUserId = _currentUserService.UserId;

            var createCourseDto = _mapper.Map<CreateCourseDto>(viewModel);
            var result = await _courseService.CreateCourseAsync(createCourseDto, currentUserId);

            return _mapper.Map<ResponseViewModel<CourseVM>>(result);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<CourseVM>> UpdateCourse(int id, [FromBody] UpdateCourseVM viewModel)
        {
            if (id != viewModel.CourseId)
            {
                return ResponseViewModel<CourseVM>.Fail( new ErrorResponseViewModel
                {
                        Type = "Validation",
                        StatusCode = 400,
                        Errors = new List<ErrorDetailViewModel>
                        {
                            new ErrorDetailViewModel(
                                "ID_MISMATCH",
                                "ID mismatch",
                                "Route ID and body CourseId must match",
                                "id"
                            )
                        }
                    },
                    "ID mismatch"
                );
            }

            var currentUserId = _currentUserService.UserId;
            var updateCourseDto = _mapper.Map<UpdateCourseDto>(viewModel);
            var result = await _courseService.UpdateCourseAsync(updateCourseDto, currentUserId);

            return _mapper.Map<ResponseViewModel<CourseVM>>(result);
        }

        [HttpDelete("{courseId}")]
        //[Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<bool>> DeleteCourse(int courseId)
        {
            var currentUserId = _currentUserService.UserId;
            var result = await _courseService.DeleteCourseAsync(id, currentUserId);

            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpGet]
        public async Task<ResponseViewModel<PagedResponseViewModel<CourseInformationViewModel>>> GetCourses(
            [FromQuery] int? instructorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _courseService.GetPaginatedCoursesAsync(instructorId, pageNumber, pageSize);

            return _mapper.Map<ResponseViewModel<PagedResponseViewModel<CourseInformationViewModel>>>(result);
        }

        [HttpGet("{courseId}")]
        public async Task<ResponseViewModel<CourseDetailsViewModel>> GetCourseDetails(int courseId)
        {
            var result = await _courseService.GetCourseDetailsAsync(courseId);

            return _mapper.Map<ResponseViewModel<CourseDetailsViewModel>>(result);
        }

        [HttpGet("{courseId}/question-pool")]
        //[Authorize(Roles = "Instructor")]
        public async Task<ResponseViewModel<List<QuestionPoolViewModel>>> GetQuestionPool(int courseId)
        {
            var currentUserId = _currentUserService.UserId;
            var result = await _courseService.GetQuestionPoolByCourseAsync(courseId, currentUserId);

            return _mapper.Map<ResponseViewModel<List<QuestionPoolViewModel>>>(result);
        }
    }


}
