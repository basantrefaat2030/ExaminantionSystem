using ExaminantionSystem.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Courcse
{
    public class StudentCourceDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Instructor ID is required")]
        public int InstructorId { get; set; }

        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? EnrollmentDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public RequestStatus Status { get; set; }
    }

}
