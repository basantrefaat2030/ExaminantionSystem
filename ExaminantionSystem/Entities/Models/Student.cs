namespace ExaminantionSystem.Entities.Models
{
    public class Student :User
    {
        // Navigation properties
        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<StudentExam> StudentExams { get; set; }
    }
}
