using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Choice
{
    public class UpdateChoiceDto
    {
        [Required(ErrorMessage = "Content is required")]
        [StringLength(500, ErrorMessage = "Content cannot exceed 500 characters")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Correct flag is required")]
        public bool IsCorrect { get; set; }
    }
}
