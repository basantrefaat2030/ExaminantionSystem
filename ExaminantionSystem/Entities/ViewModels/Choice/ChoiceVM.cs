namespace ExaminantionSystem.Entities.ViewModels.Choice
{
    public class ChoiceVM
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
    }
}
