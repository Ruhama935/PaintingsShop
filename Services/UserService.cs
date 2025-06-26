using Repositories;
using Entities;
using System.Net;
using Zxcvbn;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using DTOs;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IMapper mapper)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public int GetPasswordStrength(string password)
        {
            return Zxcvbn.Core.EvaluatePassword(password).Score;
        }


        public async Task<IEnumerable<UserDTO>> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> getUserByID(int id)//GetUserById
        {
            var user = await _userRepository.getUserByID(id);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> SignUp(UserDTO user)
        {
            if (GetPasswordStrength(user.Password) < 2)
                throw new ArgumentException("the password is too weak😐");
            var newUser = await _userRepository.SignUp(_mapper.Map<User>(user));
            return _mapper.Map<UserDTO>(newUser);
        }

        public async Task<UserDTO> Login(UserDTO user)
        {
            _logger.LogInformation($"Login attempt for user {user.UserName}\n-----------------\n");
            var foundUser = await _userRepository.Login(_mapper.Map < User > (user));
            return _mapper.Map<UserDTO>(foundUser);
        }

        public async Task<UserDTO> update(int id, UserDTO user)//Update
        {
            var updatedUser = await _userRepository.update(id, _mapper.Map < User > (user));
            return _mapper.Map<UserDTO>(updatedUser);
        }

    }
}
