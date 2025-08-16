using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Models
{
    public class ExamResult :AuditEntity
    {
        //composite key
        [ForeignKey("Exam")]
        public int ExamId { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        public decimal Score { get; set; }          // Total marks obtained
        public decimal TotalMarks { get; set; }     // Max possible marks
        public virtual Exam Exam { get; set; }
        public virtual Student Student { get; set; }

    }
}
