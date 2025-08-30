using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Shared;
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    public class QuestionRepository : Repository<Exam>
    {
        private readonly ExaminationContext _context;


        public QuestionRepository(ExaminationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> QuestionTextExistsAsync(string text, int instructorId)
        {
            var query = _context.Questions
                .Where(q => q.Text.ToLower() == text.ToLower() &&
                          q.InstructorId == instructorId &&
                          !q.IsDeleted && q.IsActive);

            return await query.AnyAsync();
        }


        public async Task<List<Question>> GetQuestionsByLevelAsync(int instructorId, QuestionLevel level)
        {
            return await _context.Questions
                .Where(q => q.InstructorId == instructorId &&
                          q.QuestionLevel == level &&
                          !q.IsDeleted && q.IsActive)
                .Include(q => q.Choices.Where(c => !c.IsDeleted))
                .ToListAsync();
        }


    }
}
