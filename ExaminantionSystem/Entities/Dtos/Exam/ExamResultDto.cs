namespace ExaminantionSystem.Entities.Dtos.Exam
{
    public class ExamResultDto
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public double Score { get; set; } // Changed to double
        public double TotalPoints { get; set; } // Changed to double
        public double Percentage { get; set; } // Changed to double
        public DateTime CompletedAt { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; }
    }
}
