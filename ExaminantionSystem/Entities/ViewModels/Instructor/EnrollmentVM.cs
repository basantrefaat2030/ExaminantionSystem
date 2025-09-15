using ExaminantionSystem.Entities.Enums;

namespace ExaminantionSystem.Entities.ViewModels.Instructor
{
    public class EnrollmentVM
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
