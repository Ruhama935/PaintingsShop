using Repositories;
using Entities;
using System.Net;
using Zxcvbn;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public int GetPasswordStrength(string password)
        {
            return Zxcvbn.Core.EvaluatePassword(password).Score;
        }


        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userRepository.GetUsers();
        }

        public async Task<User> getUserByID(int id)
        {
            return await _userRepository.getUserByID(id);
        }

        public async Task<User> SignUp(User user)
        {
            if (GetPasswordStrength(user.Password) < 2)
                throw new ArgumentException("the password is too weak😐");
            return await _userRepository.SignUp(user);
        }

        public async Task<User> Login(User user)
        {
            return await _userRepository.Login(user);
        }

        public async Task<User> update(int id, User user)
        {
            return await _userRepository.update(id, user);
        }

    }
}
