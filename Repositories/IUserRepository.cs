using Entities;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<User> getUserByID(int id);
        Task<IEnumerable<User>> GetUsers();
        Task<User> Login(User user);
        Task<User> SignUp(User user);
        Task<User> update(int id, User user);
    }
}