using ExaminantionSystem.Entities.Shared;

namespace ExaminantionSystem.Entities.ViewModels.Exam
{
    public class ManualExamQuestionsVM
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public QuestionLevel Level { get; set; }
        public double Mark { get; set; }

        public List<ManualExamChoiceVM> Choices { get; set; }
    }
}
