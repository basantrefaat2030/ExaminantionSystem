namespace ExaminantionSystem.Entities.Models
{
    public class User :BaseEntity
    {
        //public int Id { get; set; }
        
        public string FullName { get; set; }

        public string? Country {  get; set; }
        public string? City { get; set; }
        public string EmailAddress { get; set; }
        public string? PoheNumber { get; set; }
        public string PasswordHash { get; set; }

    }
}
