using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Exam
{
    public class SubmitExamDto
    {
        [Required(ErrorMessage = "Exam ID is required")]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Answers are required")]
        public Dictionary<int, int> QuestionAnswers { get; set; }
    }
}
