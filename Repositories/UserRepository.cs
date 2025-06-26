using Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PaintingsShopContext dbContext;//_dbContext;
        public UserRepository(PaintingsShopContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<User> getUserByID(int id)//GetUserById
        {
            return await dbContext.Users.FindAsync(id);
        }

        public async Task<User> SignUp(User user)
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> Login(User user)
        {
            User foundUser = await dbContext.Users.FirstAsync(u => u.UserName == user.UserName
            && u.Password == user.Password);
            if (foundUser == null)
            {
                return null;
            }
            return foundUser;
            //foundUser == null ? return null : return foundUser;
        }

        public async Task<User> update(int id, User user)//Update
        {
            // User prevUser = await dbContext.Users.FindAsync(id);
            // if (prevUser != null)
            // {
            //     prevUser.UserName = user.UserName;
            //     prevUser.Password = user.Password;
            //     prevUser.FirstName = user.FirstName;
            //     prevUser.LastName = user.LastName;
            //     dbContext.SaveChanges();
            //     return prevUser;
            // }
            // return null;
            dbContext.update(user);
            await dbContext.SaveChanges();
            return user;
        }
    }
}
