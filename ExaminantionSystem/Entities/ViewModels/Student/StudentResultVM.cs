namespace ExaminantionSystem.Entities.ViewModels.Student
{
    public class StudentResultVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public string ExamType { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public double Score { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
