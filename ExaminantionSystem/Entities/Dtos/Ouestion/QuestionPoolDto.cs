using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Shared;

namespace ExaminantionSystem.Entities.Dtos.Ouestion
{
    public class QuestionPoolDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public QuestionLevel Level { get; set; }
        public double Mark { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ChoiceCount { get; set; }
        public List<QuestionChoicesPoolDto> Choices { get; set; }
    }
}
