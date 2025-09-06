using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Exam
{
    public class AddManualExamDto
    {
        public int examId { get; set; }

        [Required(ErrorMessage = "Questions are required")]
        [MinLength(5, ErrorMessage = "At least 5 questions are required")]
        [MaxLength(20, ErrorMessage = "Cannot exceed 20 questions")]
        public List<int> questionIds {  get; set; }
    }
}
