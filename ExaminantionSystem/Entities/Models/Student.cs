namespace ExaminantionSystem.Entities.Models
{
    public class Student :User
    {
        // Navigation properties
        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<ExamResult> ExamResults { get; set; }
    }
}
