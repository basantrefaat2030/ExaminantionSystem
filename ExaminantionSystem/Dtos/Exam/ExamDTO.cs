using ExaminantionSystem.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Dtos.Exam
{
    public class ExamDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ExamType ExamType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int TotalQuestions { get; set; }
        public bool IsAutomatic { get; set; } = false; // auto generate exam
    }
}
