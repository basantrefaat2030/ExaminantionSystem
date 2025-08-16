using ExaminantionSystem.Entities.Models;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<IQueryable<Course>> GetEnrollmentCource(int StudentId);
        Task<bool> IsStudentEnrolledAsync(int courseId, int studentId);
        Task EnrollCource(Student student);
    }
}
