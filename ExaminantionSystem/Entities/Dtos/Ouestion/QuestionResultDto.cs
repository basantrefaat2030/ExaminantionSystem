namespace ExaminantionSystem.Entities.Dtos.Ouestion
{
    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public double Points { get; set; } // Changed to double
        public double EarnedPoints { get; set; } // Changed to double
        public bool IsCorrect { get; set; }
        public int? SelectedChoiceId { get; set; }
        public string SelectedChoiceContent { get; set; }
    }
}
