using Entities;

namespace Services
{
    public interface IUserService
    {
        int GetPasswordStrength(string password);
        Task<User> getUserByID(int id);
        Task<IEnumerable<User>> GetUsers();
        Task<User> Login(User user);
        Task<User> SignUp(User user);
        Task<User> update(int id, User user);
    }
}