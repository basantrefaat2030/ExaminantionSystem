using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Ouestion
{
        public class QuestionDto
        {
            public int Id { get; set; }
            public string Content {  get; set; }
            public string QuestionContent { get; set; }
            public QuestionLevel Level { get; set; }
            public int InstructorId { get; set; }
            public double Mark { get; set; }
            public string CourseName { get; set; }
            public string InstructorName { get; set; }
            public string CourseTitle { get; set; }     

           // public List<ChoiceDto> Choices { get; set; }

    }
}
