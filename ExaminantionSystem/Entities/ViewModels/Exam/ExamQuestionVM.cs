using ExaminantionSystem.Entities.Shared;

namespace ExaminantionSystem.Entities.ViewModels.Exam
{
    public class ExamQuestionVM
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public QuestionLevel Level { get; set; }
        public decimal Mark { get; set; }
        public List<ExamChoiceVM> Choices { get; set; }
    }
}
