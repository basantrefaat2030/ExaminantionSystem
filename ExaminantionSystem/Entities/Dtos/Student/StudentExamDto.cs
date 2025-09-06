namespace ExaminantionSystem.Entities.Dtos.Student
{
    public class StudentExamDto
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
       
        public int ExamResultId {  get; set; }
        public DateTime StartedAt {  get; set; }
        public int Duration {  get; set; }
        public string CourseName {  get; set; }
        //public double Score { get; set; } // Changed to double
        //public double TotalPoints { get; set; } // Changed to double
        //public double Percentage { get; set; } // Changed to double
        // public DateTime CompletedAt { get; set; }
    }
}
