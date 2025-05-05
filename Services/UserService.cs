using Repositories;
using Entities;
using System.Net;
using Zxcvbn;

namespace Services
{
    public class UserService
    {
        UserRepository repository = new UserRepository();
        public int GetPasswordStrength(string password)
        {
            return Zxcvbn.Core.EvaluatePassword(password).Score;
        }


        public IEnumerable<User> GetUsers()
        {
            return repository.GetUsers();
        }

        public User getUserByID(int id)
        {
            return repository.getUserByID(id);
        }

        public User SignUp(User user)
        {
            if(GetPasswordStrength(user.password) < 2)
                throw new ArgumentException("the password is too weak😐")
            return repository.SignUp(user);
        }

        public User Login(User user)
        {
            return repository.Login(user);
        }

        public User update(int id, User user)
        {
            return repository.update(id, user);
        }

    }
}
