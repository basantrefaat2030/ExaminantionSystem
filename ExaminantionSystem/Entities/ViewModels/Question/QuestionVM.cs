using ExaminantionSystem.Entities.Shared;
using ExaminantionSystem.Entities.ViewModels.Choice;

namespace ExaminantionSystem.Entities.ViewModels.Question
{
    public class QuestionVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public QuestionLevel Level { get; set; }
        public decimal Mark { get; set; }
        public string CourseTitle { get; set; }
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
        public List<ChoiceVM> Choices { get; set; }
    }
}
