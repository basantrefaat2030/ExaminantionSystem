using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Ouestion
{
    public class CreateQuestionDto
    {
        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Question level is required")]
        public QuestionLevel Level { get; set; }

        [Required(ErrorMessage = "Mark are required")]
        [Range(1.0, 10.0, ErrorMessage = "Mark must be between 1.0 and 10.0")]
        public double Mark { get; set; }

        [Required(ErrorMessage = "At least one choice is required")]
        [MinLength(2, ErrorMessage = "Question must have at least 2 choices")]
        [MaxLength(4, ErrorMessage = "Question cannot have more than 4 choices")]
        public List<CreateChoiceDto> Choices { get; set; }

        [Required]
        public  int CourseId { get; set; }
    }
}
