using ExaminantionSystem.Entities.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace ExaminantionSystem.Helper
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string Password);
    }


    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        public bool VerifyHashedPassword(string hashedPassword, string Password)
        {
            if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(Password))
                return false;

            var providedPasswordHash = HashPassword(Password);
            return hashedPassword == providedPasswordHash;
        }
    }

}
