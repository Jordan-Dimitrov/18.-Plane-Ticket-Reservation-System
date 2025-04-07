using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EasyFly.Infrastructure.Services;
using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Models;
using EasyFly.Domain.Abstractions;

namespace EasyFly.Tests.ServiceTests.Services
{
    [TestFixture]
    public class AuditServiceTests
    {
        private Mock<IAuditRepository> _auditRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private AuditService _auditService;

        [SetUp]
        public void Setup()
        {
            _auditRepositoryMock = new Mock<IAuditRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _auditService = new AuditService(_auditRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Test]
        public async Task CreateAudit_ShouldReturnError_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var auditDto = new AuditDto { ModifiedAt = DateTime.UtcNow, Message = "Test Message" };
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, true)).ReturnsAsync((User)null);
            var response = await _auditService.CreateAudit(userId, auditDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.UserNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task CreateAudit_ShouldReturnError_WhenInsertFails()
        {
            var userId = Guid.NewGuid();
            var auditDto = new AuditDto { ModifiedAt = DateTime.UtcNow, Message = "Test Message" };
            var user = new User { Id = userId.ToString(), UserName = "testuser", Email = "test@test.com", PhoneNumber = "123456" };
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, true)).ReturnsAsync(user);
            _auditRepositoryMock.Setup(repo => repo.InsertAsync(It.IsAny<Audit>())).ReturnsAsync(false);
            var response = await _auditService.CreateAudit(userId, auditDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task CreateAudit_ShouldReturnSuccess_WhenInsertSucceeds()
        {
            var userId = Guid.NewGuid();
            var auditDto = new AuditDto { ModifiedAt = DateTime.UtcNow, Message = "Test Message" };
            var user = new User { Id = userId.ToString(), UserName = "testuser", Email = "test@test.com", PhoneNumber = "123456" };
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, true)).ReturnsAsync(user);
            _auditRepositoryMock.Setup(repo => repo.InsertAsync(It.IsAny<Audit>())).ReturnsAsync(true);
            var response = await _auditService.CreateAudit(userId, auditDto);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task DeleteAudit_ShouldReturnError_WhenAuditNotFound()
        {
            var auditId = Guid.NewGuid();
            _auditRepositoryMock.Setup(repo => repo.GetByIdAsync(auditId, true)).ReturnsAsync((Audit)null);
            var response = await _auditService.DeleteAudit(auditId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.AuditNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task DeleteAudit_ShouldReturnError_WhenDeleteFails()
        {
            var auditId = Guid.NewGuid();
            var audit = new Audit { Id = auditId, ModifiedAt = DateTime.UtcNow, Message = "Test Message", UserId = Guid.NewGuid().ToString() };
            _auditRepositoryMock.Setup(repo => repo.GetByIdAsync(auditId, true)).ReturnsAsync(audit);
            _auditRepositoryMock.Setup(repo => repo.DeleteAsync(audit)).ReturnsAsync(false);
            var response = await _auditService.DeleteAudit(auditId);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task DeleteAudit_ShouldReturnSuccess_WhenDeleteSucceeds()
        {
            var auditId = Guid.NewGuid();
            var audit = new Audit { Id = auditId, ModifiedAt = DateTime.UtcNow, Message = "Test Message", UserId = Guid.NewGuid().ToString() };
            _auditRepositoryMock.Setup(repo => repo.GetByIdAsync(auditId, true)).ReturnsAsync(audit);
            _auditRepositoryMock.Setup(repo => repo.DeleteAsync(audit)).ReturnsAsync(true);
            var response = await _auditService.DeleteAudit(auditId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task DeleteOldestAudits_ShouldReturnSuccess_WhenAllAuditsDeleted()
        {
            var audits = new List<Audit>
            {
                new Audit { Id = Guid.NewGuid(), ModifiedAt = DateTime.UtcNow.AddDays(-10), Message = "Audit1" },
                new Audit { Id = Guid.NewGuid(), ModifiedAt = DateTime.UtcNow.AddDays(-5), Message = "Audit2" }
            };
            _auditRepositoryMock.Setup(repo => repo.GetAllByAsync(It.IsAny<Expression<Func<Audit, bool>>>()))
                .ReturnsAsync(audits);
            _auditRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Audit>())).ReturnsAsync(true);
            var response = await _auditService.DeleteOldestAudits(DateTime.UtcNow);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task DeleteOldestAudits_ShouldReturnError_WhenDeletionFails()
        {
            var audits = new List<Audit>
            {
                new Audit { Id = Guid.NewGuid(), ModifiedAt = DateTime.UtcNow.AddDays(-10), Message = "Audit1" },
                new Audit { Id = Guid.NewGuid(), ModifiedAt = DateTime.UtcNow.AddDays(-5), Message = "Audit2" }
            };
            _auditRepositoryMock.Setup(repo => repo.GetAllByAsync(It.IsAny<Expression<Func<Audit, bool>>>()))
                .ReturnsAsync(audits);
            _auditRepositoryMock.SetupSequence(repo => repo.DeleteAsync(It.IsAny<Audit>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            var response = await _auditService.DeleteOldestAudits(DateTime.UtcNow);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task GetAudit_ShouldReturnError_WhenAuditNotFound()
        {
            var auditId = Guid.NewGuid();
            _auditRepositoryMock.Setup(repo => repo.GetByIdAsync(auditId, false)).ReturnsAsync((Audit)null);
            var response = await _auditService.GetAudit(auditId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.AuditNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task GetAudit_ShouldReturnAuditViewModel_WhenAuditFound()
        {
            var auditId = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var audit = new Audit { Id = auditId, ModifiedAt = DateTime.UtcNow, Message = "Test Message", UserId = userId };
            var user = new User { Id = userId, UserName = "testuser", Email = "test@test.com", PhoneNumber = "123456" };
            _auditRepositoryMock.Setup(repo => repo.GetByIdAsync(auditId, false)).ReturnsAsync(audit);
            _userRepositoryMock.Setup(repo => repo.GetByAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);
            var response = await _auditService.GetAudit(auditId);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.AreEqual(audit.Id, response.Data.Id);
            Assert.AreEqual(audit.Message, response.Data.Message);
            Assert.AreEqual(user.Id, response.Data.User.Id);
            Assert.AreEqual(user.UserName, response.Data.User.Username);
            Assert.AreEqual(user.Email, response.Data.User.Email);
            Assert.AreEqual(user.PhoneNumber, response.Data.User.PhoneNumber);
        }

        [Test]
        public async Task GetAuditsPaged_ShouldReturnEmpty_WhenNoAuditsFound()
        {
            int page = 1, size = 10;
            _auditRepositoryMock.Setup(repo => repo.GetPagedAsync(false, page, size)).ReturnsAsync(new List<Audit>());
            _auditRepositoryMock.Setup(repo => repo.GetPageCount(size)).ReturnsAsync(0);
            var response = await _auditService.GetAuditsPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.IsEmpty(response.Data.AuditViewModels);
            Assert.AreEqual(0, response.Data.TotalPages);
        }

        [Test]
        public async Task GetAuditsPaged_ShouldReturnAuditsAndTotalPages_WhenAuditsExist()
        {
            int page = 1, size = 10;
            var audits = new List<Audit>
            {
                new Audit
                {
                    Id = Guid.NewGuid(),
                    ModifiedAt = DateTime.UtcNow,
                    Message = "Audit1",
                    User = new User { Id = Guid.NewGuid().ToString(), UserName = "user1", Email = "user1@test.com", PhoneNumber = "111" }
                },
                new Audit
                {
                    Id = Guid.NewGuid(),
                    ModifiedAt = DateTime.UtcNow,
                    Message = "Audit2",
                    User = new User { Id = Guid.NewGuid().ToString(), UserName = "user2", Email = "user2@test.com", PhoneNumber = null }
                }
            };
            _auditRepositoryMock.Setup(repo => repo.GetPagedAsync(false, page, size)).ReturnsAsync(audits);
            _auditRepositoryMock.Setup(repo => repo.GetPageCount(size)).ReturnsAsync(5);
            var response = await _auditService.GetAuditsPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.AreEqual(audits.Count, response.Data.AuditViewModels.Count());
            Assert.AreEqual(5, response.Data.TotalPages);
        }
    }
}
