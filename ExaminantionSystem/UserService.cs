using AutoMapper;
using ExaminationSystem.DTOs.User;
using ExaminationSystem.Models;
using ExaminationSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExaminationSystem.Services
{
    public class UserService
    {
        private readonly GeneralRepository<User> _generalRepository;
        public UserService(GeneralRepository<User> generalRepository)
        {
            _generalRepository = generalRepository;
        }    

        public async Task<UserLoginDTO> Login(string UserName, String Password)
        {
            var result = await  _generalRepository.Get(u => u.UserName == UserName && u.Password == Password).Select(u => new UserLoginDTO
            {
                ID = u.ID,
                Name = u.FullName,
                Role = u.Role
            }).FirstOrDefaultAsync();

            return result ?? new UserLoginDTO
            {
                ID = 0,
                Role = Models.Enums.Role.None,
            };
        }
    }
}
