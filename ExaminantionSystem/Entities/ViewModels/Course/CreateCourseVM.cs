using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.ViewModels.Course
{
    public class CreateCourseVM
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public int InstructorId { get; set; }


        [Range(80, 300, ErrorMessage = "Hours must be between 80-300")]
        [Required(ErrorMessage = "Enter Course Hours")]
        public int Hours { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive value")]
        [Required(ErrorMessage = "Enter Course Budget")]
        public double Budget { get; set; }
    }
}
