using ExaminationSystem.Data;
using ExaminationSystem.Models.Enums;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace ExaminationSystem.Helpers
{
    public class GenerateToken
    {
        public static string Generate(int userID, string Name, Role Role)
        {
            var key = Encoding.ASCII.GetBytes(Constants.SecretKey);
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "",
                Audience = "",
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("ID", userID.ToString()),
                    new System.Security.Claims.Claim(ClaimTypes.Name, Name),
                    new System.Security.Claims.Claim(ClaimTypes.Role, ((int)Role).ToString())
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
