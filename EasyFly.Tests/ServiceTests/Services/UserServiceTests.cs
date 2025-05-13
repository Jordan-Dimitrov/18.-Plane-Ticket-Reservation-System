using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using EasyFly.Infrastructure.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFly.Tests.ServiceTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Test]
        public async Task DeleteUser_ShouldReturnError_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, true))
                .ReturnsAsync((User)null);
            var response = await _userService.DeleteUser(userId);
            Assert.False(response.Success);
            Assert.AreEqual("No such user", response.ErrorMessage);
        }

        [Test]
        public async Task DeleteUser_ShouldReturnError_WhenDeleteFails()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId.ToString(), UserName = "TestUser" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, true))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.DeleteAsync(user))
                .ReturnsAsync(false);
            var response = await _userService.DeleteUser(userId);
            Assert.False(response.Success);
            Assert.AreEqual("Unexpected error", response.ErrorMessage);
        }

        [Test]
        public async Task DeleteUser_ShouldReturnSuccess_WhenDeleteSucceeds()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId.ToString(), UserName = "TestUser" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, true))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.DeleteAsync(user))
                .ReturnsAsync(true);
            var response = await _userService.DeleteUser(userId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task GetUser_ShouldReturnError_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, true))
                .ReturnsAsync((User)null);
            var response = await _userService.GetUser(userId);
            Assert.False(response.Success);
            Assert.AreEqual("No such user", response.ErrorMessage);
        }

        [Test]
        public async Task GetUser_ShouldReturnUserDto_WhenUserFound()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId.ToString(), UserName = "TestUser" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, true))
                .ReturnsAsync(user);
            var response = await _userService.GetUser(userId);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.AreEqual(user.UserName, response.Data.Username);
        }

        [Test]
        public async Task GetUserCount_ShouldReturnCorrectCount()
        {
            _userRepositoryMock.Setup(r => r.Count()).ReturnsAsync(10);
            var response = await _userService.GetUserCount();
            Assert.AreEqual(10, response.Data);
        }

        [Test]
        public async Task GetUsersPaged_ShouldReturnEmpty_WhenNoUsers()
        {
            int page = 1, size = 10;
            _userRepositoryMock.Setup(r => r.GetPagedAsync(false, page, size))
                .ReturnsAsync(new List<User>());
            var response = await _userService.GetUsersPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.IsNull(response.Data.Users);
        }

        [Test]
        public async Task GetUsersPaged_ShouldReturnUserViewModels_WhenUsersExist()
        {
            int page = 1, size = 10;
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid().ToString(), UserName = "User1", Email = "user1@test.com", PhoneNumber = "12345" },
                new User { Id = Guid.NewGuid().ToString(), UserName = "User2", Email = "user2@test.com", PhoneNumber = null }
            };
            _userRepositoryMock.Setup(r => r.GetPagedAsync(false, page, size))
                .ReturnsAsync(users);
            _userRepositoryMock.Setup(r => r.GetPageCount(page)).ReturnsAsync(2);
            var response = await _userService.GetUsersPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.AreEqual(users.Count, response.Data.Users.Count());
            Assert.AreEqual(2, response.Data.TotalPages);
            var user2Dto = response.Data.Users.FirstOrDefault(u => u.Username == "User2");
            Assert.NotNull(user2Dto);
            Assert.AreEqual("No phone provided", user2Dto.PhoneNumber);
        }

        [Test]
        public async Task UpdateUser_ShouldReturnError_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var userDto = new UserDto { Username = "UpdatedUser", Email = "updated@test.com", PhoneNumber = "98765" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, true))
                .ReturnsAsync((User)null);
            var response = await _userService.UpdateUser(userDto, userId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.UserNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateUser_ShouldReturnError_WhenUpdateFails()
        {
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId.ToString(), UserName = "OldUser", Email = "old@test.com", PhoneNumber = "11111" };
            var userDto = new UserDto { Username = "UpdatedUser", Email = "updated@test.com", PhoneNumber = "98765" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, true))
                .ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.UpdateAsync(existingUser)).ReturnsAsync(false);
            var response = await _userService.UpdateUser(userDto, userId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateUser_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId.ToString(), UserName = "OldUser", Email = "old@test.com", PhoneNumber = "11111" };
            var userDto = new UserDto { Username = "UpdatedUser", Email = "updated@test.com", PhoneNumber = "98765" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, true))
                .ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.UpdateAsync(existingUser)).ReturnsAsync(true);
            var response = await _userService.UpdateUser(userDto, userId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(userDto.Username, existingUser.UserName);
            Assert.AreEqual(userDto.Email, existingUser.Email);
            Assert.AreEqual(userDto.PhoneNumber, existingUser.PhoneNumber);
        }
    }
}
