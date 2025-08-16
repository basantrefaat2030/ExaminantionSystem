using ExaminantionSystem.Entities.Shared;

namespace ExaminantionSystem.Entities.Dtos.Ouestion
{
    public class OuestionDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public QuestionLevel QuestionLevel { get; set; }

        public decimal Mark { get; set; }

        public int InstructorId { get; set; }

        public int CourseId { get; set; }

    }
}
