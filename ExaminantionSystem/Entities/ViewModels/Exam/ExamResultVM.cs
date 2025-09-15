using ExaminantionSystem.Entities.Dtos.Exam;
using ExaminantionSystem.Entities.ViewModels.Student;

namespace ExaminantionSystem.Entities.ViewModels.Exam
{
    public class ExamResultVM
    {
        public StudentExamInformationVM studentExamInformation { get; set; }

        public ExamInfomationVM examInfomationDto { get; set; }
        //public double Percentage { get; set; } 
        public string CourseName { get; set; }

        public double TotalScore { get; set; }
        public DateTime ExamStartAt { get; set; }
        public DateTime ExamCompletedAt { get; set; }
        public List<EvaluateQuestionResultVM> QuestionResults { get; set; }
    }
}
