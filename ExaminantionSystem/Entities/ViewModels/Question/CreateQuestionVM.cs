using ExaminantionSystem.Entities.Shared;
using ExaminantionSystem.Entities.ViewModels.Choice;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.ViewModels.Question
{
    public class CreateQuestionVM
    {
        [Required] public string Content { get; set; }
        [Required] public QuestionLevel Level { get; set; }
        [Required][Range(0.1, 100)] public decimal Mark { get; set; }
        [Required] public int CourseId { get; set; }


        [Required(ErrorMessage = "At least one choice is required")]
        [MinLength(2, ErrorMessage = "Question must have at least 2 choices")]
        [MaxLength(4, ErrorMessage = "Question cannot have more than 4 choices")]
        public List<CreateChoiceVM> Choices { get; set; }

    }
}
