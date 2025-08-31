using ExaminantionSystem.Entities.Enums;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExaminantionSystem.Infrastructure.Repositories
{

    //public interface ICourseRepository : IRepository<Course>
    //{
    //    Task<IQueryable<Course>> GetCoursesByInstructorId(int instructorId);
    //    Task<IQueryable<Course>> GetCoursesByStudentId(int studentId);
    //}

    public class CourseRepository : Repository<Course>
    {
        private readonly ExaminationContext _context;
        public CourseRepository(ExaminationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> GetTotalEnrollments(int courseId , bool IsActive = false)
        {
            return await _context.StudentCourses
                        .CountAsync(sc => sc.Id == courseId && !sc.IsDeleted && sc.IsActive == IsActive && sc.Status == RequestStatus.Approved);
        }
        public async Task<bool> CourseTitleExistsAsync(string title, int? instructorId)
        {
            var query = _context.Courses
                .Where(c => c.Title.ToLower() == title.ToLower()
                         && c.InstructorId == instructorId
                         && !c.IsDeleted
                         && c.IsActive);

            return await query.AnyAsync();
        }

        public async Task<bool> CourseHasActiveEnrollmentsAsync(int courseId)
        {
            return await _context.StudentCourses
                .AnyAsync(sc => sc.CourseId == courseId
                             && sc.Status == RequestStatus.Approved
                             && !sc.IsDeleted
                             && sc.IsActive);
        }

        public async Task<bool> IsInstructorCourseOwnerAsync(int courseId, int instructorId)
        {
            return await _context.Courses
                .AnyAsync(c => c.Id == courseId
                            && c.InstructorId == instructorId
                            && !c.IsDeleted
                            && c.IsActive);
        }

        public async Task<bool> CourseHasExamsAsync(int courseId)
        {
            return await _context.Exams
                .AnyAsync(e => e.CourseId == courseId && !e.IsDeleted);
        }

        public async Task<bool> IsCourseActiveAsync(int courseId)
        {
            return await _context.Courses
                .AnyAsync(c => c.Id == courseId
                            && c.IsActive
                            && !c.IsDeleted);
        }

        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await _context.Courses
                .AnyAsync(c => c.Id == courseId && !c.IsDeleted && c.IsActive);
        }

      


    }


}
