using ExaminantionSystem.Entities.Dtos.Ouestion;

namespace ExaminantionSystem.Entities.Dtos.Exam
{
    public class ExamResultDto
    {

        public StudentExamInformationDto studentExamInformation { get; set; }

        public ExamInfomationDto examInfomationDto { get; set; }
        //public double Percentage { get; set; } 
        public string CourseName { get; set; }

        public double TotalScore { get; set; }
        public DateTime ExamStartAt { get; set; }
        public DateTime ExamCompletedAt { get; set; }
        public List<EvaluateQuestionResultDto> QuestionResults { get; set; }
    }

    public class StudentExamInformationDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
    }
    public class ExamInfomationDto
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }

        public string CourseName { get; set; }
    }


}
