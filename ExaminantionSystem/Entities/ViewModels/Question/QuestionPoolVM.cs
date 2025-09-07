using ExaminantionSystem.Entities.Shared;

namespace ExaminantionSystem.Entities.ViewModels.Question
{
    public class QuestionPoolVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public QuestionLevel Level { get; set; }
        public decimal Mark { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<QuestionChoicesPoolVM> Choices { get; set; }
        public int ChoiceCount { get; set; }
    }

    public class QuestionChoicesPoolVM
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}
