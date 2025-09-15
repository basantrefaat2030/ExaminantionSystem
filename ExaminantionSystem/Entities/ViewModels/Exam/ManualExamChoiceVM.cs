namespace ExaminantionSystem.Entities.ViewModels.Exam
{
    public class ManualExamChoiceVM
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
    }
}
