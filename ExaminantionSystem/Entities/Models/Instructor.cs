namespace ExaminantionSystem.Entities.Models
{
    public class Instructor : User
    {
        public string? Bio { get; set; }
        public DateTime? HireDate { get; set; }

        public int? YearOfExperience { get; set; }
        public string? Specialization { get; set; }
        // Navigation properties
        public ICollection<Course> Courses { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
