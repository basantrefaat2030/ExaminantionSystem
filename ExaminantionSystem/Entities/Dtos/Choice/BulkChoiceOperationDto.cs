using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Choice
{
    public class BulkChoiceOperationDto
    {
        [Required(ErrorMessage = "Question ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Question ID")]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Choices are required")]
        [MinLength(4, ErrorMessage = "Choose a four choice per question")]
        public List<CreateChoiceDto> Choices { get; set; }
    }
}
