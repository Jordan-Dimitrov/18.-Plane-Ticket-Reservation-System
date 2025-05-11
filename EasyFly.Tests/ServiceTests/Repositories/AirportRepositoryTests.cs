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
    public class AirportRepositoryTests
    {
        private ApplicationDbContext _context;
        private IAirportRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new AirportRepository(_context);
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
        public async Task InsertAsync_ShouldAddNewAirport()
        {
            var newAirport = new Airport
            {
                Id = Guid.NewGuid(),
                Name = "New Airport",
                Flights = new List<Flight>()
            };

            var result = await _repository.InsertAsync(newAirport);

            Assert.IsTrue(result);
            Assert.IsTrue(await _context.Airports.AnyAsync(a => a.Id == newAirport.Id));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateExistingAirport()
        {
            var airport = _context.Airports.First();
            airport.Name = "Updated Airport Name";

            var result = await _repository.UpdateAsync(airport);

            Assert.IsTrue(result);
            var updated = await _context.Airports.FirstOrDefaultAsync(a => a.Id == airport.Id);
            Assert.AreEqual("Updated Airport Name", updated.Name);
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkAirportAndFlightsAsDeleted()
        {
            var airport = _context.Airports.Include(a => a.Flights).First();

            var result = await _repository.DeleteAsync(airport);

            Assert.IsTrue(result);
            Assert.IsNotNull(airport.DeletedAt);
            foreach (var flight in airport.Flights)
            {
                Assert.IsNotNull(flight.DeletedAt);
            }
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrueWhenAirportExists()
        {
            var airport = _context.Airports.First();

            bool exists = await _repository.ExistsAsync(a => a.Id == airport.Id);

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalseWhenAirportDoesNotExist()
        {
            bool exists = await _repository.ExistsAsync(a => a.Id == Guid.NewGuid());

            Assert.IsFalse(exists);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllAirports()
        {
            var airports = await _repository.GetAllAsync(trackChanges: false);

            Assert.AreEqual(2, airports.Count());
        }

        [Test]
        public async Task GetByAsync_ShouldReturnMatchingAirport()
        {
            var airport = _context.Airports.First();

            var result = await _repository.GetByAsync(a => a.Id == airport.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(airport.Id, result.Id);
        }

        [Test]
        public async Task GetByAsync_ShouldReturnNullWhenNoMatch()
        {
            var result = await _repository.GetByAsync(a => a.Id == Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnAirportWhenExists()
        {
            var airport = _context.Airports.First();

            var result = await _repository.GetByIdAsync(airport.Id, trackChanges: false);

            Assert.IsNotNull(result);
            Assert.AreEqual(airport.Id, result.Id);
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
            int pageCount = await _repository.GetPageCount(1);

            Assert.AreEqual(2, pageCount);
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnPagedAirports()
        {
            int pageSize = 1;

            var page1 = await _repository.GetPagedAsync(trackChanges: false, page: 1, size: pageSize);
            var page2 = await _repository.GetPagedAsync(trackChanges: false, page: 2, size: pageSize);

            Assert.AreEqual(1, page1.Count());
            Assert.AreEqual(1, page2.Count());
            Assert.AreNotEqual(page1.First().Id, page2.First().Id);
        }
    }
}
