using ExaminantionSystem.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminantionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;
        public CourseController(CourseService courseService) 
        { 
            _courseService = courseService;
        }

        //public IActionResult AddCourse()
        //{

        //}

        //public IActionResult EditCourse()
        //{

        //}

        //public IActionResult DeleteCourse()
        //{

        //}

    }
}
