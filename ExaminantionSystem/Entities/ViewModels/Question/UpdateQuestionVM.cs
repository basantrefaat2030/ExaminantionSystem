using ExaminantionSystem.Entities.Shared;
using ExaminantionSystem.Entities.ViewModels.Choice;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.ViewModels.Question
{
    public class UpdateQuestionVM
    {
        [Required] 
        public int QuestionId { get; set; }
        [Required] 
        public string Content { get; set; }
        [Required] 
        public QuestionLevel Level { get; set; }
        [Required]
        [Range(1.0, 10.0)] 
        public double Mark { get; set; }

        [Required(ErrorMessage = "At least one choice is required")]
        [MinLength(2, ErrorMessage = "Question must have at least 2 choices")]
        [MaxLength(4, ErrorMessage = "Question cannot have more than 4 choices")]
        public List<CreateChoiceVM> Choices { get; set; }
    }
}
