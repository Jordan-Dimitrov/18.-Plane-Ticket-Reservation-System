using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using EasyFly.Persistence;
using EasyFly.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EasyFly.Tests.ServiceTests.Repositories
{
    [TestFixture]
    internal class UserRepositoryTests
    {
        private ApplicationDbContext _context;
        private IUserRepository _repository;
        private User _user1;
        private User _user2;
        private Audit _audit1;
        private Ticket _ticket1;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new UserRepository(_context);
            SeedData();
        }

        private void SeedData()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.Audits.RemoveRange(_context.Audits);
            _context.Tickets.RemoveRange(_context.Tickets);
            _context.SaveChanges();

            var userId1 = Guid.NewGuid().ToString();
            var userId2 = Guid.NewGuid().ToString();

            _user1 = new User
            {
                Id = userId1,
                UserName = "user1",
                Email = "user1@test.com"
            };

            _user2 = new User
            {
                Id = userId2,
                UserName = "user2",
                Email = "user2@test.com"
            };

            _context.Users.AddRange(_user1, _user2);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Count_ShouldReturnCorrectUserCount()
        {
            int count = await _repository.Count();
            Assert.AreEqual(2, count);
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkUserAndRelatedEntitiesAsDeleted()
        {
            var result = await _repository.DeleteAsync(_user1);
            Assert.IsTrue(result);
            Assert.IsNotNull(_user1.DeletedAt);
            Assert.IsTrue(_user1.Audits.All(a => a.DeletedAt != null));
            Assert.IsTrue(_user1.Ticket.All(t => t.DeletedAt != null));
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrueWhenUserExists()
        {
            bool exists = await _repository.ExistsAsync(u => u.Id == _user2.Id);
            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalseWhenUserDoesNotExist()
        {
            bool exists = await _repository.ExistsAsync(u => u.Id == Guid.NewGuid().ToString());
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            var users = await _repository.GetAllAsync(false);
            Assert.AreEqual(2, users.Count());
        }

        [Test]
        public async Task GetByAsync_ShouldReturnMatchingUser()
        {
            var user = await _repository.GetByAsync(u => u.UserName == "user2");
            Assert.IsNotNull(user);
            Assert.AreEqual(_user2.Id, user.Id);
        }

        [Test]
        public async Task GetByAsync_ShouldReturnNullWhenNoMatch()
        {
            var user = await _repository.GetByAsync(u => u.UserName == "nonexistent");
            Assert.IsNull(user);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnUserWhenExists()
        {
            var user = await _repository.GetByIdAsync(Guid.Parse(_user1.Id), false);
            Assert.IsNotNull(user);
            Assert.AreEqual(_user1.Id, user.Id);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNullWhenNotExists()
        {
            var user = await _repository.GetByIdAsync(Guid.NewGuid(), false);
            Assert.IsNull(user);
        }

        [Test]
        public async Task GetPageCount_ShouldReturnCorrectPageCount()
        {
            int pageCount = await _repository.GetPageCount(1);
            Assert.AreEqual(2, pageCount);
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnPagedUsers()
        {
            var page1 = await _repository.GetPagedAsync(false, 1, 1);
            var page2 = await _repository.GetPagedAsync(false, 2, 1);
            Assert.AreEqual(1, page1.Count());
            Assert.AreEqual(1, page2.Count());
            Assert.AreNotEqual(page1.First().Id, page2.First().Id);
        }

        [Test]
        public async Task InsertAsync_ShouldAddNewUser()
        {
            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "user3",
                Email = "user3@test.com"
            };
            var result = await _repository.InsertAsync(newUser);
            Assert.IsTrue(result);
            Assert.IsTrue(await _context.Users.AnyAsync(u => u.Id == newUser.Id));
        }

        [Test]
        public async Task UpdateAsync_ShouldModifyUser()
        {
            _user2.Email = "updated@test.com";
            var result = await _repository.UpdateAsync(_user2);
            Assert.IsTrue(result);
            var updated = await _context.Users.FirstOrDefaultAsync(u => u.Id == _user2.Id);
            Assert.AreEqual("updated@test.com", updated.Email);
        }
    }
}
