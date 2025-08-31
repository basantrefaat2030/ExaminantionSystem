using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos
{
    public class UpdateCourseDto
    {
        [Required(ErrorMessage = "Course title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3-100 characters")]
        public string Title { get; set; }
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Range(1, 300, ErrorMessage = "Hours must be between 1-300")]
        [Required(ErrorMessage = "Enter Course Hours")]
        public int Hours { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive value")]
        [Required(ErrorMessage = "Enter Course Budget")]
        public double Budget { get; set; }
        [Required(ErrorMessage = "Instractor id is required")]
        public int InstructorId { get; set; }
    }
}
