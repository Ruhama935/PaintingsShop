using Repositories;

namespace Services
{
    public class UserService
    {
        UserRepository repository = new UserRepository();
        public List<User> GetUser()
        {
            return repository.GetUser();
        }

        public User getUserByID(int id)
        {
            return repository.getUserByID(id);
        }

        public User SignUp(User user)
        {
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
