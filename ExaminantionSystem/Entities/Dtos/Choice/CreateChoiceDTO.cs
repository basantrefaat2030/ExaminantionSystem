using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Choice
{
    public class CreateChoiceDto
    {
       
            [Required(ErrorMessage = "Choice text is required")]
            [StringLength(500, ErrorMessage = "Choice text cannot exceed 500 characters")]
            public string Text { get; set; }

             [Required(ErrorMessage = "Correct flag is required")]
              public bool IsCorrect { get; set; }
        
    }
}
