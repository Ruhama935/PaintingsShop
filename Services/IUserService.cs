using Entities;
using DTOs;

namespace Services
{
    public interface IUserService
    {
        int GetPasswordStrength(string password);
        Task<UserDTO> getUserByID(int id);
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<UserDTO> Login(UserDTO user);
        Task<UserDTO> SignUp(UserDTO user);
        Task<UserDTO> update(int id, UserDTO user);
    }
}