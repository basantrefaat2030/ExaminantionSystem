namespace ExaminantionSystem.Entities.ViewModels.Course
{
    public class CourseVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int InstructorId { get; set; }
        public int Hours { get; set; }
        public decimal Budget { get; set; }
        public bool IsActive { get; set; }
    }
}
