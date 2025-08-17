using ExaminantionSystem.Entities.Dtos.Courcse;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure;
using ExaminantionSystem.Infrastructure.Repositories;

namespace ExaminantionSystem.Service
{
    public interface ICourseService
    {
        Task<PagedResponse<CourseDTO>> GetPaginatedCoursesAsync(int pageNumber, int pageSize);
        Task<Response<CourseDTO>> GetCourseByIdAsync(int id);
        Task<Response<CourseDTO>> CreateCourseAsync(CourseDTO entity, int instructorId);
        Task<Response<CourseDTO>> UpdateCourseAsync(int id, CourseDTO entity);
        Task<Response<bool>> DeleteCourseAsync(int id);
        Task<Response<IEnumerable<CourseDTO>>> GetInstructorCoursesAsync(int instructorId);

    }
    public class CourseService :ICourseService 
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Instructor> _instructorRepository;


        //dependacy Injection
        public CourseService(
                 IRepository<Course> courseRepository,IRepository<Instructor> instructorRepository)
        {
            _courseRepository = courseRepository;
            _instructorRepository = instructorRepository;
        }

        public Task<Response<CourseDTO>> CreateCourseAsync(CourseDTO entity, int instructorId)
        {
            throw new NotImplementedException();
        }

        public Task<Response<bool>> DeleteCourseAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<CourseDTO>> GetCourseByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<IEnumerable<CourseDTO>>> GetInstructorCoursesAsync(int instructorId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponse<CourseDTO>> GetPaginatedCoursesAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Response<CourseDTO>> UpdateCourseAsync(int id, CourseDTO entity)
        {
            throw new NotImplementedException();
        }
    }
    
}
