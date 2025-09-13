namespace ExaminantionSystem.Entities.ViewModels.Student
{
    public class StudentExamVM
    {
        public int ExamResultId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime StartedAt { get; set; }
        public string CourseName { get; set; }
        public int? Duration { get; set; }
    }
}
