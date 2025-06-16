using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Repositories;

namespace Tests
{
    public class UserRepositoryUnitTesting
    {
        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            // Arrange
            var user1 = new User { Id = 1, FirstName = "Miriam", LastName = "Cohen", UserName = "miriam123", Password = "password"};
            var user2 = new User { Id = 2, FirstName = "David", LastName = "Levi", UserName = "david456", Password = "123456" };

            var mockContext = new Mock<PaintingsShopContext>();
            var users = new List<User>() { user1, user2 };
            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = await userRepository.GetUsers();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(user1.FirstName, result.First().FirstName);
        }

        [Fact]
        public async Task GetUserByID_ValidId_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "Miriam", LastName = "Cohen", UserName = "miriam123", Password = "password" };

            var mockContext = new Mock<PaintingsShopContext>();
            var users = new List<User>() { user };
            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = await userRepository.getUserByID(1);

            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task GetUserByID_InvalidId_ReturnsNull()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "Miriam", LastName = "Cohen", UserName = "miriam123", Password = "password" };

            var mockContext = new Mock<PaintingsShopContext>();
            var users = new List<User>() { user };
            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = await userRepository.getUserByID(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SignUp_ValidUser_ReturnsCreatedUser()
        {
            // Arrange
            var user = new User { FirstName = "John", LastName = "Doe", UserName = "john123", Password = "newpass"};

            var mockContext = new Mock<PaintingsShopContext>();
            var users = new List<User>();
            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = await userRepository.SignUp(user);

            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "Miriam", LastName = "Cohen", UserName = "miriam123", Password = "password" };
            var loginUser = new User { UserName = "miriam123", Password = "password" };

            var mockContext = new Mock<PaintingsShopContext>();
            var users = new List<User>() { user };
            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = await userRepository.Login(loginUser);

            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ThrowsException()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "Miriam", LastName = "Cohen", UserName = "miriam123", Password = "password" };
            var loginUser = new User { UserName = "wronguser", Password = "wrongpass" };

            var mockContext = new Mock<PaintingsShopContext>();
            var users = new List<User>() { user };
            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var userRepository = new UserRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => userRepository.Login(loginUser));
        }

        [Fact]
        public async Task Update_ValidIdAndUser_ReturnsUpdatedUser()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "Miriam", LastName = "Cohen", UserName = "miriam123", Password = "password" };
            var updateData = new User { FirstName = "UpdatedMiriam", LastName = "UpdatedCohen", UserName = "updatedmiriam", Password = "newpassword" };

            var mockContext = new Mock<PaintingsShopContext>();
            var users = new List<User>() { user };
            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = await userRepository.update(1, updateData);

            // Assert
            Assert.Equal(updateData.FirstName, result.FirstName);
            Assert.Equal(updateData.LastName, result.LastName);
            Assert.Equal(updateData.UserName, result.UserName);
            Assert.Equal(updateData.Password, result.Password);
        }

        [Fact]
        public async Task Update_InvalidId_ReturnsNull()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "Miriam", LastName = "Cohen", UserName = "miriam123", Password = "password" };
            var updateData = new User { FirstName = "UpdatedName", LastName = "UpdatedLast", UserName = "updated", Password = "newpass" };

            var mockContext = new Mock<PaintingsShopContext>();
            var users = new List<User>() { user };
            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = await userRepository.update(999, updateData);

            // Assert
            Assert.Null(result);
        }
    }
 }
