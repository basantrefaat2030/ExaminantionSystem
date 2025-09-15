using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.ViewModels.Student
{
    public class StudentAnswerVM
    {
        [Required(ErrorMessage = "Question ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Question ID")]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Choice ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Choice ID")]
        public int ChoiceId { get; set; }
    }
}
