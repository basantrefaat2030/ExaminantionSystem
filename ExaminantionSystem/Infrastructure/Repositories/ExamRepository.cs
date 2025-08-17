using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Data;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    public interface IExamRepository :IRepository<Exam>
    {
        Task<IQueryable<Student>>GetAllStudentExamed();


    }

    public class ExamRepository : Repository<Exam> , IExamRepository
    {
        public ExamRepository(ExaminationContext context)  : base(context) { }

        public Task<IQueryable<Student>> GetAllStudentExamed()
        {
            throw new NotImplementedException();
        }
    }
}
