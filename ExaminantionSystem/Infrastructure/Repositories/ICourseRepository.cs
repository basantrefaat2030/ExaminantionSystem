using ExaminantionSystem.Entities.Models;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IQueryable<Course>> GetCoursesByInstructorId(int instructorId);
        Task<IQueryable<Course>> GetCoursesByStudentId(int studentId);
    }
}
