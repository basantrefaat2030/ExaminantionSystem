using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    //public interface IInstructorRepository :IRepository<Instructor>
    //{
    //    Task<bool>  CheckEnrollmentStatuse(int StudentId , int CourceId);
    //    Task<IQueryable<Student>> GetAllEnrollStudents(int CourceId);
    //}

    public class InstructorRepository : Repository<Instructor> 
    {
        private readonly ExaminationContext _context;
        public InstructorRepository(ExaminationContext context) : base(context) { _context = context; }

        public async Task<bool> InstructorHasCoursesAsync(int instructorId)
        {
            return await _context.Courses
                .AnyAsync(c => c.InstructorId == instructorId && !c.IsDeleted);
        }

        public async Task<int> GetInstructorCourseCountAsync(int instructorId)
        {
            return await _context.Courses
                .CountAsync(c => c.InstructorId == instructorId && !c.IsDeleted && c.IsActive);
        }

        public async Task<int> GetInstructorActiveCourseCountAsync(int instructorId)
        {
            return await _context.Courses
                .CountAsync(c => c.InstructorId == instructorId && c.IsActive && !c.IsDeleted);
        }

    }
}
