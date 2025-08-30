using ExaminantionSystem.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Entities.Dtos.Exam
{
    public class ExamDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Exam ID is required")]
        public int ExamId { get; set; }

        public DateTime? StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Score must be a positive number")]
        public int Score { get; set; }

        [Range(1, 100, ErrorMessage = "Total questions must be 1-100")]
        public int TotalQuestions { get; set; }

        [Range(0, 100, ErrorMessage = "Percentage must be 0-100")]
        public decimal Percentage { get; set; }
    }
}
