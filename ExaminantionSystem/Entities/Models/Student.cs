namespace ExaminantionSystem.Entities.Models
{
    public class Student :BaseEntity
    {
        // Navigation properties
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<ExamResult> ExamResults { get; set; }
    }
}
