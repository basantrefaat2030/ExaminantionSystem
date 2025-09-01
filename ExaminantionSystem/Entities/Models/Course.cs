using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Models
{
    public class Course :BaseEntity
    {
        //public int CourceId {  get; set; }
        public string Title { get; set; }

        public string? Description { get; set; }
        public int? Hours {  get; set; }

        public double Budget {  get; set; }


        [ForeignKey("Instructor")]
        public int InstructorId { get; set; }
       // public User Instructor { get; set; }

        // Navigation
        public virtual Instructor Instructor { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<StudentCourse> Enrollments { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
