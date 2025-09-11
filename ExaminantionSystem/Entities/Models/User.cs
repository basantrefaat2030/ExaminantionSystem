using ExaminantionSystem.Entities.Enums;

namespace ExaminantionSystem.Entities.Models
{
    public class User :BaseEntity
    {
        //public int Id { get; set; }
        
        public string FullName { get; set; }

        public string? Country {  get; set; }
        public string? City { get; set; }
        public string EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; } 
    }
}
