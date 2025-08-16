using ExaminantionSystem.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Models
{
    public class Question :AuditEntity
    {
        //public int QuestionId {  get; set; }
        public string Text { get; set; }

        public QuestionLevel QuestionLevel { get; set; }

        public decimal Mark { get; set; }

        [ForeignKey("Instructor")]
        public int InstructorId { get; set; }


        [ForeignKey("Course")]
        public int CourseId { get; set; }


        // Navigation
        public virtual User Instructor { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Choice> Choices { get; set; }
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; }
    }
}
