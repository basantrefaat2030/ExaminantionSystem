using ExaminantionSystem.Entities.Models;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    public interface IInstructorRepository :IRepository<Instructor>
    {
        Task<bool>  CheckEnrollmentStatuse(int StudentId , int CourceId);
        Task<IQueryable<Student>> GetAllEnrollStudents(int CourceId);
    }
}
