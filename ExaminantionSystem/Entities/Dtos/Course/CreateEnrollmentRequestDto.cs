using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Course
{
    public class CreateEnrollmentRequestDto
    {
        [Required(ErrorMessage = "Course ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Course ID")]
        public int CourseId { get; set; }
    }
}
