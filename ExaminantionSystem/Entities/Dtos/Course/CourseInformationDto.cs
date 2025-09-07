namespace ExaminantionSystem.Entities.Dtos.Course
{
    public class CourseInformationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Hours { get; set; }
        public double Budget { get; set; }
        public int InstructorId { get; set; }

        public string InstructorName { get; set; }
        public bool IsActive { get; set; }
    }
}
