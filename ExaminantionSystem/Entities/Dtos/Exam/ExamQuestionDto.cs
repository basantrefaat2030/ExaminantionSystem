using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Shared;

namespace ExaminantionSystem.Entities.Dtos.Exam
{
    public class ExamQuestionDto
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public QuestionLevel Level { get; set; }
        public double Mark { get; set; }
        public List<ExamWithQuestionsChoicesDto> Choices { get; set; }
    }
}
