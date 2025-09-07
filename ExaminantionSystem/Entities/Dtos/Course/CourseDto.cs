using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Entities.Dtos.Course
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Hours { get; set; }
        public double Budget { get; set; }
        public int InstructorId { get; set; }

    }
}
