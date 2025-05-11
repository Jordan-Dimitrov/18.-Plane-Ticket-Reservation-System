using NUnit.Framework;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using EasyFly.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using EasyFly.Persistence;

namespace EasyFly.Tests.ServiceTests.Repositories
{
    [TestFixture]
    public class AuditRepositoryTests
    {
        private ApplicationDbContext _context;
        private IAuditRepository _repository;
        private User _user;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new AuditRepository(_context);
            Seed seed = new Seed(_context);
            seed.SeedData();
        }


        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task InsertAsync_ShouldAddNewAudit()
        {
            User user = new User
            {
                Id = "user2",
                UserName = "TestUser"
            };

            var newAudit = new Audit
            {
                Message = "New Audit Message",
                ModifiedAt = DateTime.UtcNow,
                User = user,
                UserId = user.Id
            };

            var result = await _repository.InsertAsync(newAudit);

            Assert.IsTrue(result);
            Assert.IsTrue(await _context.Audits.AnyAsync(a => a.Id == newAudit.Id));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateExistingAudit()
        {
            var audit = _context.Audits.First();
            audit.Message = "Updated Audit Message";

            var result = await _repository.UpdateAsync(audit);

            Assert.IsTrue(result);
            var updated = await _context.Audits.FirstOrDefaultAsync(a => a.Id == audit.Id);
            Assert.AreEqual("Updated Audit Message", updated.Message);
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkAuditAsDeleted()
        {
            var audit = _context.Audits.First();

            var result = await _repository.DeleteAsync(audit);

            Assert.IsTrue(result);
            Assert.IsNotNull(audit.DeletedAt);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrueWhenAuditExists()
        {
            var audit = _context.Audits.First();

            bool exists = await _repository.ExistsAsync(a => a.Id == audit.Id);

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalseWhenAuditDoesNotExist()
        {
            bool exists = await _repository.ExistsAsync(a => a.Id == Guid.NewGuid());

            Assert.IsFalse(exists);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllAudits()
        {
            var audits = await _repository.GetAllAsync(trackChanges: false);

            Assert.AreEqual(3, audits.Count());
        }

        [Test]
        public async Task GetAllByAsync_ShouldReturnMatchingAudits()
        {
            ICollection<Audit>? audits = await _repository.GetAllByAsync(a => a.Message.Contains("1"));

            Assert.IsNotNull(audits);
            Assert.AreEqual(1, audits.Count);
            Assert.IsTrue(audits.All(a => a.Message.Contains("1")));
        }

        [Test]
        public async Task GetByAsync_ShouldReturnMatchingAudit()
        {
            var audit = _context.Audits.First();

            var result = await _repository.GetByAsync(a => a.Id == audit.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(audit.Id, result.Id);
        }

        [Test]
        public async Task GetByAsync_ShouldReturnNullWhenNoMatch()
        {
            var result = await _repository.GetByAsync(a => a.Id == Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnAuditWhenExists()
        {
            var audit = _context.Audits.First();

            var result = await _repository.GetByIdAsync(audit.Id, trackChanges: false);

            Assert.IsNotNull(result);
            Assert.AreEqual(audit.Id, result.Id);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNullWhenNotExists()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid(), trackChanges: false);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetPageCount_ShouldReturnCorrectPageCount()
        {
            int pageCount = await _repository.GetPageCount(2);

            Assert.AreEqual(2, pageCount);
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnPagedAudits()
        {
            int pageSize = 1;

            var page1 = await _repository.GetPagedAsync(trackChanges: false, page: 1, size: pageSize);
            var page2 = await _repository.GetPagedAsync(trackChanges: false, page: 2, size: pageSize);
            var page3 = await _repository.GetPagedAsync(trackChanges: false, page: 3, size: pageSize);

            Assert.AreEqual(1, page1.Count());
            Assert.AreEqual(1, page2.Count());
            Assert.AreEqual(1, page3.Count());
            Assert.AreNotEqual(page1.First().Id, page2.First().Id);
            Assert.AreNotEqual(page2.First().Id, page3.First().Id);
        }
    }
}
