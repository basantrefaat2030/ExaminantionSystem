using ExaminantionSystem.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Course
{
    public class StudentCourseDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? EnrollmentDate { get; set; }
    }

}
