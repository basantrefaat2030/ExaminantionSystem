

using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.User
{
    public class UserLoginDto
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
