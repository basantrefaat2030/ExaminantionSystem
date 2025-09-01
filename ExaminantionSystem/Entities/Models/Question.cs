using ExaminantionSystem.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Models
{
    public class Question : BaseEntity
    {
        //public int QuestionId {  get; set; }
        public string Content { get; set; }

        public QuestionLevel QuestionLevel { get; set; }

        [ForeignKey("Course")]
        public int CourseId {  get; set; }

        public double Mark { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<Choice> Choices { get; set; }
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; }
    }
}
