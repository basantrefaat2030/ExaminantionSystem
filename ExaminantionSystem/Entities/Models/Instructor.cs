using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Entities.Models
{
    public class Instructor : BaseEntity
    {
        public string? Bio { get; set; }
        public DateTime? HireDate { get; set; }

        public int? YearOfExperience { get; set; }
        public string? Specialization { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        // Navigation properties
        public virtual User User { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<Exam> Exams { get; set; }
        //public ICollection<Question> Questions { get; set; }
    }
}
