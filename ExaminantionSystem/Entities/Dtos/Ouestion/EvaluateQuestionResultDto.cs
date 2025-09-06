namespace ExaminantionSystem.Entities.Dtos.Ouestion
{
    public class EvaluateQuestionResultDto
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public double Mark { get; set; }
        public double EarnedMarks { get; set; }
        public bool IsCorrect { get; set; }
        public int? SelectedChoiceId { get; set; }
        public string SelectedChoiceContent { get; set; }
    }
}
