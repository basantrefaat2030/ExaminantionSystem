using ExaminantionSystem.Entities.Shared;

namespace ExaminantionSystem.Entities.Dtos.Exam
{
    public class ExamWithQuestionsChoicesDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
    }
}
