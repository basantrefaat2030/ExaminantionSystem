using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Data;
using System.Linq.Expressions;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    public interface IInstructorRepository :IRepository<Instructor>
    {
        Task<bool>  CheckEnrollmentStatuse(int StudentId , int CourceId);
        Task<IQueryable<Student>> GetAllEnrollStudents(int CourceId);
    }

    public class InstructorRepository : Repository<Instructor> , IInstructorRepository
    {
        public InstructorRepository(ExaminationContext context) : base(context) { }

        public Task<bool> CheckEnrollmentStatuse(int StudentId, int CourceId)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Student>> GetAllEnrollStudents(int CourceId)
        {
            throw new NotImplementedException();
        }
    }
}
