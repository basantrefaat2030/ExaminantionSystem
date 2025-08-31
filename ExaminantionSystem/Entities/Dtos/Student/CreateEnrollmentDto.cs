using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Student
{
    public class CreateEnrollmentDto
    {
        [Required(ErrorMessage = "Student ID is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }
    }
}
