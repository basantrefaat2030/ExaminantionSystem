using ExaminantionSystem.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Models
{
    public class Question : BaseEntity
    {
        //public int QuestionId {  get; set; }
        public string Text { get; set; }

        public QuestionLevel QuestionLevel { get; set; }

        [ForeignKey("Instructor")]
        public int InstructorId {  get; set; }

        public double Mark { get; set; }

        public virtual Instructor Instructor { get; set; }
        public virtual ICollection<Choice> Choices { get; set; }
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; }
    }
}
