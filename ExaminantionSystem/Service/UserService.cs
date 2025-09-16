using ExaminantionSystem.Entities.Dtos.User;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace ExaminantionSystem.Service
{
    public class UserService
    {
        private readonly Repository<User> _userRepository;
        public UserService(Repository<User> userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<Response<UserLoginInformationDto>> LoginAsync(UserLoginDto loginRequest)
        {
        }
    }
}
