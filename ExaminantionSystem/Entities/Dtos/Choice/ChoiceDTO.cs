using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Choice
{
    public class ChoiceDto
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }


       // public int QuestionId { get; set; }
    }

}
