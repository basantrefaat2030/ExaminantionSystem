using ExaminantionSystem.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Ouestion
{
        public class QuestionDto
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Question text is required")]
            [StringLength(1000, MinimumLength = 10, ErrorMessage = "Question must be 10-1000 characters")]
            public string Text { get; set; }

            [Required(ErrorMessage = "Question level is required")]
            public QuestionLevel Level { get; set; }
            public int InstructorId { get; set; }

            [Range(1.0, 10.0, ErrorMessage = "Mark must be between 1.0-10.0")]
            public double Mark { get; set; }
        }
}
