using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Entities.Models
{
    public class StudentAnswer :BaseEntity
    {

        // Foreign keys
        [ForeignKey("Student")]
        public int StudentExamId { get; set; }
        [ForeignKey("Student")]
        public int QuestionId { get; set; }
        [ForeignKey("Choice")]
        public int? ChoiceId { get; set; } //if not choose any answer

        // Navigation properties
        public virtual StudentExam StudentExam { get; set; }
        public virtual Question Question { get; set; }
        public virtual Choice Choice { get; set; }
    }
}
