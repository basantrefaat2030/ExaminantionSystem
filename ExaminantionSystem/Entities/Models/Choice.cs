using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Entities.Models
{
    public class Choice :BaseEntity
    {
        //public int ChoiceId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        // Navigation property
        public virtual Question Question { get; set; }
    }
}
