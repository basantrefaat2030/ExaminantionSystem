using ExaminantionSystem.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Models
{
    public class Exam : BaseEntity
    {
        //public int ExamId { get; set; }
        public string Title { get; set; }

        public string? Description { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        public ExamType ExamType { get; set; }

        //[Range(1, 1000)]
        // public int DurationMinutes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int TotalQuestions { get; set; }
        public bool IsAutomatic { get; set; } = false; // auto generate exam
        // Navigation

        public virtual Course Course { get; set; }
        public virtual  ICollection<ExamQuestion> ExamQuestions { get; set; }
        public virtual ICollection<ExamResult> ExamResults { get; set; }
    }
}
