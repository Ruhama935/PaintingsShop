using AutoMapper;
using DTOs;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Repositories;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tests
{
    public class UserServiceUnitTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _userService;

        public UserServiceUnitTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockMapper = new Mock<IMapper>();
            _userService = new UserService(_mockUserRepository.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetPasswordStrength_WithWeakPassword_ReturnsLowScore()
        {
            // Arrange
            string weakPassword = "1234567";

            // Act
            int score = _userService.GetPasswordStrength(weakPassword);

            // Assert
            Assert.True(score < 2);
        }

        [Fact]
        public void GetPasswordStrength_WithStrongPassword_ReturnsHighScore()
        {
            // Arrange
            string strongPassword = "G00dPassw0rd!@#";

            // Act
            int score = _userService.GetPasswordStrength(strongPassword);

            // Assert
            Assert.True(score >= 3);
        }

        [Fact]
        public async Task Login_LogsInformationMessage()
        {
            // Arrange
            var userDto = new UserDTO { UserName = "tester@example.com", Password = "password123" };
            var userEntity = new User { UserName = "tester@example.com", Password = "password123" };

            _mockMapper.Setup(m => m.Map<User>(userDto)).Returns(userEntity);
            _mockMapper.Setup(m => m.Map<UserDTO>(userEntity)).Returns(userDto);
            _mockUserRepository.Setup(r => r.Login(It.IsAny<User>())).ReturnsAsync(userEntity);

            // Act
            var result = await _userService.Login(userDto);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Login attempt for user")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SignUp_WithWeakPassword_ThrowsArgumentException()
        {
            // Arrange
            var weakUserDto = new UserDTO { UserName = "weak@example.com", Password = "123" };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _userService.SignUp(weakUserDto));
            Assert.Equal("the password is too weak😐", ex.Message);
        }

        [Fact]
        public async Task SignUp_WithStrongPassword_ReturnsUser()
        {
            // Arrange
            var userDto = new UserDTO { UserName = "strong@example.com", Password = "Str0ngP@ssword" };
            var userEntity = new User { UserName = "strong@example.com", Password = "Str0ngP@ssword" };

            _mockMapper.Setup(m => m.Map<User>(userDto)).Returns(userEntity);
            _mockMapper.Setup(m => m.Map<UserDTO>(userEntity)).Returns(userDto);
            _mockUserRepository.Setup(r => r.SignUp(It.IsAny<User>())).ReturnsAsync(userEntity);

            // Act
            var result = await _userService.SignUp(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.UserName, result.UserName);
        }
    }
}
