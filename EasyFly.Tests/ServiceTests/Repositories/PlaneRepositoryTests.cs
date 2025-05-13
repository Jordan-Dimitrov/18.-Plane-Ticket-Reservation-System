using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using EasyFly.Persistence;
using EasyFly.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyFly.Tests.ServiceTests.Repositories
{
    [TestFixture]
    internal class PlaneRepositoryTests
    {
        private ApplicationDbContext _context;
        private IPlaneRepository _repository;
        private Plane _plane1;
        private Plane _plane2;
        private Airport _dummyAirport;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new PlaneRepository(_context);
            SeedData();
        }

        private void SeedData()
        {
            _context.Planes.RemoveRange(_context.Planes);
            _context.Airports.RemoveRange(_context.Airports);
            _context.SaveChanges();

            _dummyAirport = new Airport { Id = Guid.NewGuid(), Name = "Dummy Airport" };

            var seats1 = new List<Seat>
            {
                new Seat { Id = Guid.NewGuid() },
                new Seat { Id = Guid.NewGuid() }
            };

            _plane1 = new Plane
            {
                Id = Guid.NewGuid(),
                Name = "Plane 1",
                Seats = seats1,
                Flights = new List<Flight>()
            };

            var flight = new Flight
            {
                FlightNumber = "TestFlight",
                DepartureTime = DateTime.UtcNow.AddHours(1),
                ArrivalTime = DateTime.UtcNow.AddHours(3),
                DepartureAirport = _dummyAirport,
                DepartureAirportId = _dummyAirport.Id,
                ArrivalAirport = _dummyAirport,
                ArrivalAirportId = _dummyAirport.Id,
                Plane = _plane1,
                PlaneId = _plane1.Id,
                TicketPrice = 100m,
                Tickets = new List<Ticket>()
            };
            _plane1.Flights.Add(flight);

            var seats2 = new List<Seat>
            {
                new Seat { Id = Guid.NewGuid() }
            };

            _plane2 = new Plane
            {
                Id = Guid.NewGuid(),
                Name = "Plane 2",
                Seats = seats2,
                Flights = new List<Flight>()
            };

            _context.Airports.Add(_dummyAirport);
            _context.Planes.AddRange(_plane1, _plane2);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task InsertAsync_ShouldAddNewPlane()
        {
            var seats = new List<Seat>
            {
                new Seat { Id = Guid.NewGuid() },
                new Seat { Id = Guid.NewGuid() }
            };
            var newPlane = new Plane
            {
                Id = Guid.NewGuid(),
                Name = "New Plane",
                Seats = seats,
                Flights = new List<Flight>()
            };

            var result = await _repository.InsertAsync(newPlane);

            Assert.IsTrue(result);
            Assert.IsTrue(await _context.Planes.AnyAsync(p => p.Id == newPlane.Id));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateExistingPlane()
        {
            var plane = _context.Planes.First();
            plane.Name = "Updated Plane Name";

            var result = await _repository.UpdateAsync(plane);

            Assert.IsTrue(result);
            var updated = await _context.Planes.FirstOrDefaultAsync(p => p.Id == plane.Id);
            Assert.AreEqual("Updated Plane Name", updated.Name);
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkPlaneSeatsAndFlightsAsDeleted()
        {
            var plane = await _context.Planes.Include(p => p.Seats).Include(p => p.Flights)
                .FirstOrDefaultAsync(p => p.Id == _plane1.Id);
            var result = await _repository.DeleteAsync(plane);

            Assert.IsTrue(result);
            Assert.IsNotNull(plane.DeletedAt);
            foreach (var seat in plane.Seats)
            {
                Assert.IsNotNull(seat.DeletedAt);
            }
            foreach (var flight in plane.Flights)
            {
                Assert.IsNotNull(flight.DeletedAt);
            }
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrueWhenPlaneExists()
        {
            bool exists = await _repository.ExistsAsync(p => p.Id == _plane1.Id);
            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalseWhenPlaneDoesNotExist()
        {
            bool exists = await _repository.ExistsAsync(p => p.Id == Guid.NewGuid());
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllPlanes()
        {
            var planes = await _repository.GetAllAsync(trackChanges: false);
            Assert.AreEqual(2, planes.Count());
        }

        [Test]
        public async Task GetByAsync_ShouldReturnMatchingPlane()
        {
            var plane = await _repository.GetByAsync(p => p.Id == _plane1.Id);
            Assert.IsNotNull(plane);
            Assert.AreEqual(_plane1.Id, plane.Id);
        }

        [Test]
        public async Task GetByAsync_ShouldReturnNullWhenNoMatch()
        {
            var plane = await _repository.GetByAsync(p => p.Id == Guid.NewGuid());
            Assert.IsNull(plane);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnPlaneWhenExists()
        {
            var plane = await _repository.GetByIdAsync(_plane1.Id, trackChanges: false);
            Assert.IsNotNull(plane);
            Assert.AreEqual(_plane1.Id, plane.Id);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNullWhenNotExists()
        {
            var plane = await _repository.GetByIdAsync(Guid.NewGuid(), trackChanges: false);
            Assert.IsNull(plane);
        }

        [Test]
        public async Task GetPageCount_ShouldReturnCorrectPageCount()
        {
            int pageCount = await _repository.GetPageCount(1);
            Assert.AreEqual(2, pageCount);
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnPagedPlanes()
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
