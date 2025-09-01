using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Shared;
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    public class QuestionRepository : Repository<Question>
    {
        private readonly ExaminationContext _context;


        public QuestionRepository(ExaminationContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<bool> QuestionTextExistsAsync(string text, int courseId)
        //{
        //    var query = _context.Questions
        //        .Where(q => q.Content.ToLower() == text.ToLower() &&
        //                  q.InstructorId == instructorId &&
        //                  !q.IsDeleted && q.IsActive);

        //    return await query.AnyAsync();
        //}


        public async Task<List<Question>> GetQuestionsByLevelAsync(int courseId, QuestionLevel level)
        {
            return await _context.Questions
                .Where(q => q.CourseId == courseId &&
                          q.QuestionLevel == level &&
                          !q.IsDeleted && q.IsActive)
                .Include(q => q.Choices.Where(c => !c.IsDeleted))
                .ToListAsync();
        }


    }
}
