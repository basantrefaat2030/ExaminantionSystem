using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Dtos.Courcse
{
    public class CourseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Hours { get; set; }
        public double Budget { get; set; }

    }
}
