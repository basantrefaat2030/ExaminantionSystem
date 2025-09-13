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
            return await  _context.Choices
                .AnyAsync(c => c.Text.ToLower() == text.ToLower() &&
                          c.QuestionId == questionId &&
                          !c.IsDeleted && c.IsActive);
        }

        public async Task<bool> IsChoiceRelatedWithActiveExam(int questionId)
        {
            return await _context.ExamQuestions
                   .AnyAsync(eq => eq.QuestionId == questionId && 
                    eq.Question.IsActive && 
                    !eq.Question.IsDeleted &&
                    eq.Exam.StartDate <= DateTime.Now &&
                    eq.Exam.EndDate >= DateTime.Now);
        } 

    }
}
