using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{

    using Microsoft.EntityFrameworkCore;
    using Xunit;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;
    using System;
    using Repositories;

    public class UserRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly PaintingsShopContext _dbContext;
        private readonly UserRepository _userRepository;

        public UserRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _userRepository = new UserRepository(_dbContext);
        }

        #region GetUsers Tests - Happy & Unhappy Paths

        [Fact]
        public async Task GetUsers_WithMultipleUsers_ReturnsAllUsers()
        {
            // Arrange
            var user1 = new User { UserName = "testuser1@example.com", Password = "password1@", FirstName = "John", LastName = "Doe" };
            var user2 = new User { UserName = "testuser2@example.com", Password = "password2@", FirstName = "Jane", LastName = "Smith" };
            var user3 = new User { UserName = "testuser3@example.com", Password = "password3@", FirstName = "Bob", LastName = "Johnson" };

            await _dbContext.Users.AddRangeAsync(user1, user2, user3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUsers();

            // Assert
            Assert.NotNull(result);
            var usersList = result.ToList();
            Assert.True(usersList.Count >= 3);
            Assert.Contains(usersList, u => u.UserName == "testuser1@example.com");
            Assert.Contains(usersList, u => u.UserName == "testuser2@example.com");
            Assert.Contains(usersList, u => u.UserName == "testuser3@example.com");

            // Cleanup
            _dbContext.Users.RemoveRange(user1, user2, user3);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetUsers_EmptyDatabase_ReturnsEmptyCollection()
        {
            // Arrange - ensure database is clean
            var existingUsers = await _dbContext.Users.ToListAsync();
            _dbContext.Users.RemoveRange(existingUsers);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUsers_WithSingleUser_ReturnsSingleUser()
        {
            // Arrange
            var existingUsers = await _dbContext.Users.ToListAsync();
            _dbContext.Users.RemoveRange(existingUsers);
            await _dbContext.SaveChangesAsync();

            var user = new User { UserName = "singleuser@example.com", Password = "password1@", FirstName = "Single", LastName = "User" };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUsers();

            // Assert
            Assert.NotNull(result);
            var usersList = result.ToList();
            Assert.Single(usersList);
            Assert.Equal("singleuser", usersList.First().UserName);

            // Cleanup
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region GetUserByID Tests - Happy & Unhappy Paths

        [Fact]
        public async Task GetUserByID_ValidId_ReturnsCorrectUser()
        {
            // Arrange
            var user = new User { UserName = "testuser@example.com", Password = "password22@", FirstName = "Test", LastName = "User" };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.getUserByID(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal("testuser", result.UserName);
            Assert.Equal("password22@", result.Password);
            Assert.Equal("Test", result.FirstName);
            Assert.Equal("User", result.LastName);

            // Cleanup
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetUserByID_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _userRepository.getUserByID(99999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByID_NegativeId_ReturnsNull()
        {
            // Act
            var result = await _userRepository.getUserByID(-1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByID_ZeroId_ReturnsNull()
        {
            // Act
            var result = await _userRepository.getUserByID(0);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region SignUp Tests - Happy & Unhappy Paths

        [Fact]
        public async Task SignUp_ValidUser_CreatesAndReturnsUser()
        {
            // Arrange
            var newUser = new User
            {
                UserName = "newuser@example.com",
                Password = "newpassword12!",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var result = await _userRepository.SignUp(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser", result.UserName);
            Assert.Equal("newpassword12!", result.Password);
            Assert.Equal("New", result.FirstName);
            Assert.Equal("User", result.LastName);
            Assert.True(result.Id > 0);

            // Verify user was saved to database
            var savedUser = await _dbContext.Users.FindAsync(result.Id);
            Assert.NotNull(savedUser);
            Assert.Equal("newuser", savedUser.UserName);

            // Cleanup
            _dbContext.Users.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task SignUp_UserWithMinimalData_CreatesUser()
        {
            // Arrange
            var newUser = new User
            {
                UserName = "minimaluser@example.com",
                Password = "pass14242@"
            };

            // Act
            var result = await _userRepository.SignUp(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("minimaluser", result.UserName);
            Assert.Equal("pass14242@", result.Password);
            Assert.True(result.Id > 0);

            // Cleanup
            _dbContext.Users.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task SignUp_NullUser_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _userRepository.SignUp(null)
            );
        }

        [Fact]
        public async Task SignUp_DuplicateUsername_ThrowsException()
        {
            // Arrange
            var existingUser = new User
            {
                UserName = "duplicateuser@example.com",
                Password = "password1!",
                FirstName = "First",
                LastName = "User"
            };
            await _dbContext.Users.AddAsync(existingUser);
            await _dbContext.SaveChangesAsync();

            var duplicateUser = new User
            {
                UserName = "duplicateuser@example.com",
                Password = "password#2",
                FirstName = "Second",
                LastName = "User"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userRepository.SignUp(duplicateUser)
            );

            // Cleanup
            _dbContext.Users.Remove(existingUser);
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Login Tests - Happy & Unhappy Paths

        [Fact]
        public async Task Login_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var user = new User
            {
                UserName = "loginuser@example.com",
                Password = "loginpassword22^",
                FirstName = "Login",
                LastName = "Test"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var loginAttempt = new User
            {
                UserName = "loginuser@example.com",
                Password = "loginpassword22^"
            };

            // Act
            var result = await _userRepository.Login(loginAttempt);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal("loginuser", result.UserName);
            Assert.Equal("loginpassword22^", result.Password);
            Assert.Equal("Login", result.FirstName);
            Assert.Equal("Test", result.LastName);

            // Cleanup
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task Login_InvalidUsername_ThrowsException()
        {
            // Arrange
            var loginAttempt = new User
            {
                UserName = "nonexistentuser@example.com",
                Password = "password123@"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userRepository.Login(loginAttempt)
            );
        }

        [Fact]
        public async Task Login_InvalidPassword_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                UserName = "testuser@example.com",
                Password = "correctpassword1@",
                FirstName = "Test",
                LastName = "User"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var loginAttempt = new User
            {
                UserName = "testuser@example.com",
                Password = "wrongpassword"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userRepository.Login(loginAttempt)
            );

            // Cleanup
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task Login_NullUser_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _userRepository.Login(null)
            );
        }

        [Fact]
        public async Task Login_EmptyUsername_ThrowsException()
        {
            // Arrange
            var loginAttempt = new User
            {
                UserName = "",
                Password = "password22@"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userRepository.Login(loginAttempt)
            );
        }

        [Fact]
        public async Task Login_EmptyPassword_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                UserName = "testuser",
                Password = "correctpassword2#",
                FirstName = "Test",
                LastName = "User"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var loginAttempt = new User
            {
                UserName = "testuser",
                Password = ""
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userRepository.Login(loginAttempt)
            );

            // Cleanup
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Update Tests - Happy & Unhappy Paths

        [Fact]
        public async Task Update_ValidIdAndUser_UpdatesAndReturnsUser()
        {
            // Arrange
            var originalUser = new User
            {
                UserName = "originaluser",
                Password = "originalpassword1@",
                FirstName = "Original",
                LastName = "User"
            };
            await _dbContext.Users.AddAsync(originalUser);
            await _dbContext.SaveChangesAsync();

            var updatedUserData = new User
            {
                UserName = "updateduser",
                Password = "updatedpassword1@",
                FirstName = "Updated",
                LastName = "User"
            };

            // Act
            var result = await _userRepository.update(originalUser.Id, updatedUserData);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(originalUser.Id, result.Id);
            Assert.Equal("updateduser", result.UserName);
            Assert.Equal("updatedpassword1@", result.Password);
            Assert.Equal("Updated", result.FirstName);
            Assert.Equal("User", result.LastName);

            // Verify changes were saved
            var dbUser = await _dbContext.Users.FindAsync(originalUser.Id);
            Assert.NotNull(dbUser);
            Assert.Equal("updateduser", dbUser.UserName);
            Assert.Equal("Updated", dbUser.FirstName);

            // Cleanup
            _dbContext.Users.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task Update_PartialUpdate_UpdatesOnlyProvidedFields()
        {
            // Arrange
            var originalUser = new User
            {
                UserName = "originaluser",
                Password = "originalpassword@2",
                FirstName = "Original",
                LastName = "User"
            };
            await _dbContext.Users.AddAsync(originalUser);
            await _dbContext.SaveChangesAsync();

            var partialUpdate = new User
            {
                UserName = "newusername",
                Password = "newpassword@2",
                FirstName = "NewFirst",
                LastName = "NewLast"
            };

            // Act
            var result = await _userRepository.update(originalUser.Id, partialUpdate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newusername", result.UserName);
            Assert.Equal("newpassword@2", result.Password);
            Assert.Equal("NewFirst", result.FirstName);
            Assert.Equal("NewLast", result.LastName);

            // Cleanup
            _dbContext.Users.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task Update_InvalidId_ReturnsNull()
        {
            // Arrange
            var updatedUserData = new User
            {
                UserName = "updateduser",
                Password = "updatedpassword2!",
                FirstName = "Updated",
                LastName = "User"
            };

            // Act
            var result = await _userRepository.update(99999, updatedUserData);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Update_NegativeId_ReturnsNull()
        {
            // Arrange
            var updatedUserData = new User
            {
                UserName = "updateduser",
                Password = "updatedpassword1@",
                FirstName = "Updated",
                LastName = "User"
            };

            // Act
            var result = await _userRepository.update(-1, updatedUserData);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Update_ZeroId_ReturnsNull()
        {
            // Arrange
            var updatedUserData = new User
            {
                UserName = "updateduser",
                Password = "updatedpassword2@",
                FirstName = "Updated",
                LastName = "User"
            };

            // Act
            var result = await _userRepository.update(0, updatedUserData);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Update_NullUser_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                UserName = "testuser",
                Password = "password22@",
                FirstName = "Test",
                LastName = "User"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(
                () => _userRepository.update(user.Id, null)
            );

            // Cleanup
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
