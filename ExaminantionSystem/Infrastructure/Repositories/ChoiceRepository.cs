using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    public class ChoiceRepository : Repository<Choice>
    {
        private readonly ExaminationContext _context;
        public ChoiceRepository(ExaminationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ChoiceTextExistsAsync(string text, int questionId)
        {
            var query = _context.Choices
                .Where(c => c.Text.ToLower() == text.ToLower() &&
                          c.QuestionId == questionId &&
                          !c.IsDeleted && c.IsActive);

            return await query.AnyAsync();
        }

        public async Task<List<Choice>> GetChoicesForQuestionAsync(int questionId)
        {
            return await _context.Choices
                .Where(c => c.QuestionId == questionId && !c.IsDeleted && c.IsActive)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

    }
}
