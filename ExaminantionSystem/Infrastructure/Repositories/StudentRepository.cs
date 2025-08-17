using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Data;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<IQueryable<Course>> GetEnrollmentCource(int StudentId);
        Task<bool> IsStudentEnrolledAsync(int courseId, int studentId);
        Task EnrollCource(Student student);
    }

    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(ExaminationContext executionContex) : base(executionContex){}

        public Task EnrollCource(Student student)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Course>> GetEnrollmentCource(int StudentId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsStudentEnrolledAsync(int courseId, int studentId)
        {
            throw new NotImplementedException();
        }
    }
}
