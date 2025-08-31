using ExaminantionSystem.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Ouestion
{
   
        public class UpdateQuestionDto
        {
            [Required(ErrorMessage = "Content is required")]
            [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
            public string Content { get; set; }

            [Required(ErrorMessage = "Question level is required")]
            public QuestionLevel Level { get; set; }

            [Required(ErrorMessage = "Mark is required")]
            [Range(0.1, 10.0, ErrorMessage = "Mark must be between 0.1 and 10.0")]
            public double Mark { get; set; }
        }
    
}
