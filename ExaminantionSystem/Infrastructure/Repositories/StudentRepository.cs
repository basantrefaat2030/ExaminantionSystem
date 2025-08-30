using ExaminantionSystem.Entities.Enums;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Shared;
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    //public interface IStudentRepository : IRepository<Student>
    //{
    //    Task<IQueryable<Course>> GetEnrollmentCource(int StudentId);
    //    Task<bool> IsStudentEnrolledAsync(int courseId, int studentId);
    //    Task EnrollCource(Student student);
    //}

    public class StudentRepository : Repository<Student>
    {
        private readonly ExaminationContext _context;
        public StudentRepository(ExaminationContext context) : base(context){ _context = context; }

        public async Task<bool> StudentEmailExistsAsync(string email)
        {
            var query = _context.Students
                .Where(s => s.EmailAddress.ToLower() == email.ToLower() && !s.IsDeleted && s.IsActive);


            return await query.AnyAsync();
        }

        public async Task<bool> StudentIsEnrolledInCourseAsync(int studentId, int courseId)
        {
            return await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == studentId &&
                              sc.CourseId == courseId &&
                              sc.Status == RequestStatus.Approved &&
                              !sc.IsDeleted && sc.IsActive);
        }

        public async Task<int> GetStudentEnrollmentCountAsync(int studentId)
        {
            return await _context.StudentCourses
                .CountAsync(sc => sc.StudentId == studentId && !sc.IsDeleted && sc.IsActive);
        }


        public async Task<bool> StudentHasTakenFinalExamAsync(int studentId, int courseId)
        {
            return await _context.ExamResult
                .AnyAsync(er => er.StudentId == studentId &&
                              er.Exam.CourseId == courseId &&
                              er.Exam.ExamType == ExamType.Final &&
                              !er.IsDeleted && 
                              er.IsActive);
        }
    }
}
