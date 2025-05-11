using Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        //string filePath = "./userFile.txt";
        PaintingsShopContext dbContext;
        public UserRepository(PaintingsShopContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            List<User> users = await dbContext.Users.ToListAsync();
            return users;
        }

        public async Task<User> getUserByID(int id)
        {
            User user = await dbContext.Users.FindAsync(id);
            return user;
        }

        public async Task<User> SignUp(User user)
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> Login(User user)
        {
            User foundUser = await dbContext.Users.FirstAsync(u =>  u.UserName == user.UserName 
            && u.Password == user.Password);
            if (foundUser == null)
            {
                return null;
            }
            return foundUser;
        }

        public async Task<User> update(int id, User user)
        {
            User prevUser = await dbContext.Users.FindAsync(id);
            if (prevUser != null)
            {
                prevUser.UserName = user.UserName;
                prevUser.Password = user.Password;
                prevUser.FirstName = user.FirstName;
                prevUser.LastName = user.LastName;
                dbContext.SaveChanges();
                return prevUser;
            }
            //dbContext.Users.First<User>();
            //string textToReplace = string.Empty;
            //using (StreamReader reader = System.IO.File.OpenText(filePath))
            //{
            //    string currentUserInFile;
            //    while ((currentUserInFile = reader.ReadLine()) != null)
            //    {

            //        User Cuser = JsonSerializer.Deserialize<User>(currentUserInFile);
            //        if (Cuser.Id == id)
            //            textToReplace = currentUserInFile;
            //    }
            //}

            //if (textToReplace != string.Empty)
            //{
            //    string text = System.IO.File.ReadAllText(filePath);
            //    text = text.Replace(textToReplace, JsonSerializer.Serialize(user));
            //    System.IO.File.WriteAllText(filePath, text);
            //    return user;
            //}
            return null;
        }
    }
}
