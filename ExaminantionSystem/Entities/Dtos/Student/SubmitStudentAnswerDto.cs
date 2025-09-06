namespace ExaminantionSystem.Entities.Dtos.Student
{
    public class SubmitStudentAnswerDto
    {
        public int examId { get; set; }
        public List<StudentAnswerDto> answers { get; set; }
    }
}
