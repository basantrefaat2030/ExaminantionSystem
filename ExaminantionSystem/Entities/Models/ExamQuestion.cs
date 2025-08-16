using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Entities.Models
{
    public class ExamQuestion :AuditEntity
    {
        //composite key
        [ForeignKey("Exam")]
        public int ExamId { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        // Navigation properties
        public virtual Exam Exam { get; set; }
        public virtual Question Question { get; set; }
    }
}
