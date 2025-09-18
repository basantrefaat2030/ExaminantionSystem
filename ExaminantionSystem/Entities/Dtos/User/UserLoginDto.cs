

using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.User
{
    public class UserLoginDto
    {
        //public int userId { get; set; } 
        [Required]
        public string emailAddress { get; set; }
        [Required]
        public string password { get; set; }

    }
}
