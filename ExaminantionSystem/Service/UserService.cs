using ExaminantionSystem.Entities.Dtos.User;
using ExaminantionSystem.Entities.Enums;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Helper;
using ExaminantionSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExaminantionSystem.Service
{
    public class UserService
    {
        private readonly Repository<User> _userRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher _passwordHasher;

        public UserService(Repository<User> userRepository, IConfiguration configuration , PasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }


        public async Task<Response<UserLoginInformationDto>> LoginAsync(UserLoginDto loginRequest)
        {

            // Find user by email
            var user = await _userRepository.GetAll(u => u.EmailAddress == loginRequest.emailAddress).FirstOrDefaultAsync();

            if (user == null)
                return Response<UserLoginInformationDto>.Fail(ErrorType.USER_NOT_FOUND,
                    new ErrorDetail("Invalid email or password"));

            // Verify password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user.PasswordHash, loginRequest.password);
            if (!passwordVerificationResult)
                return Response<UserLoginInformationDto>.Fail(ErrorType.INVALID_PASSWORD_CREDENTIALS,
                    new ErrorDetail("Invalid email or password"));

            // Generate JWT token

            var token = GenerateToken(user);

            var response = new UserLoginInformationDto
            {
                Token = token,
                Expiry = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryInMinutes")),
                Role = user.Role.ToString(),
                FullName = user.FullName,
                UserId = user.Id
            };

            return Response<UserLoginInformationDto>.Success(response);
        }


        private string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.EmailAddress),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            
        };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("ExpiryInMinutes")),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
