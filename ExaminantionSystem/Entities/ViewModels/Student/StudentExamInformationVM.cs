namespace ExaminantionSystem.Entities.ViewModels.Student
{
    public class StudentExamInformationVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
    }

    public class ExamInfomationVM
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public string CourseName { get; set; }
    }
    public class EvaluateQuestionResultVM
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public double Mark { get; set; }
        public double EarnedMarks { get; set; }
        public bool IsCorrect { get; set; }
        public int? SelectedChoiceId { get; set; }
        public string SelectedChoiceContent { get; set; }
    }
}
