using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Choice
{
    public class ChoiceDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Choice text is required")]
        [StringLength(500, ErrorMessage = "Choice text cannot exceed 500 characters")]
        public string Text { get; set; }

        public bool IsCorrect { get; set; }
       // public int QuestionId { get; set; }
    }

}
