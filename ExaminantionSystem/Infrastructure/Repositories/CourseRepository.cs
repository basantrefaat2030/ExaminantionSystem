using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Data;
using System.Linq.Expressions;

namespace ExaminantionSystem.Infrastructure.Repositories
{

    public interface ICourseRepository : IRepository<Course>
    {
        Task<IQueryable<Course>> GetCoursesByInstructorId(int instructorId);
        Task<IQueryable<Course>> GetCoursesByStudentId(int studentId);
    }

    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(ExaminationContext executionContex) : base(executionContex)
        {
        }

        public Task<IQueryable<Course>> GetCoursesByInstructorId(int instructorId)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Course>> GetCoursesByStudentId(int studentId)
        {
            throw new NotImplementedException();
        }
    }


}
