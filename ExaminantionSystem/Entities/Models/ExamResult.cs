using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Entities.Models
{
    public class ExamResult :BaseEntity
    {
        // Composite key
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        [ForeignKey("Exam")]
        public int ExamId { get; set; }

        // Properties
        public DateTime? StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public double Score { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Exam Exam { get; set; }
        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; }
    }
}
