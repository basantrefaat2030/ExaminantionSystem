namespace ExaminantionSystem.Dtos.Instructor
{
    public class InstructorDTO
    {
        public int Id { get; set; } 
        public string FullName { get; set; }

        public string? Country { get; set; }
        public string? City { get; set; }
        public string EmailAddress { get; set; }

        public string? Bio { get; set; }
        public DateTime? HireDate { get; set; }
        public int YearOfExperience { get; set; }
        public string? Specialization { get; set; }
    }
}
