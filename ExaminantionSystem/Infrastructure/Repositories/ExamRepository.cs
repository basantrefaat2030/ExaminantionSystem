using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Shared;
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    //public interface IExamRepository :IRepository<Exam>
    //{
    //    Task<IQueryable<Student>>GetAllStudentExamed();


    //}

    public class ExamRepository : Repository<Exam> 
    {
        private readonly ExaminationContext _context;
        public ExamRepository(ExaminationContext context)  : base(context) { _context = context; }

        public async Task<bool> ExamTitleExistsAsync(string title, int courseId)
        {
            var query = _context.Exams
                .Where(e => e.Title.ToLower() == title.ToLower() &&
                          e.CourseId == courseId 
                          &&!e.IsDeleted
                          && e.IsActive);

            return await query.AnyAsync();
        }

        public async Task<bool> IsQuestionInExamActive(int questionId)
        {
            return await  _context.ExamQuestions.AnyAsync(a => a.QuestionId == questionId 
            && !a.Exam.IsDeleted
            && a.Exam.IsActive 
            && a.Exam.StartDate < DateTime.UtcNow);
        }

        public async Task<bool> ExamHasSubmissionsAsync(int examId)
        {
            return await _context.ExamResult
                .AnyAsync(er => er.ExamId == examId && !er.IsDeleted);
        }



        public async Task<List<Exam>> GetCourseExamsAsync(int courseId, ExamType? examType = null)
        {
            var query = _context.Exams
                .Where(e => e.CourseId == courseId && !e.IsDeleted && e.IsActive);

            if (examType.HasValue)
                query = query.Where(e => e.ExamType == examType.Value);

            return await query.ToListAsync();
        }

        public async Task<bool> IsFinalExamCreatedAsync(int courseId)
        {
            return await _context.Exams
                .AnyAsync(e => e.CourseId == courseId &&
                             e.ExamType == ExamType.Final &&
                             !e.IsDeleted && e.IsActive);
        }

        public async Task<int> GetTotalExam(int courseId)
        {
            return await _context.Exams.CountAsync(e => e.CourseId == courseId && !e.IsDeleted && e.IsActive);
        }
    }
}
