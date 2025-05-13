using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Enums;
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
    internal class SeatRepositoryTests
    {
        private ApplicationDbContext _context;
        private ISeatRepository _repository;
        private Plane _plane;
        private Flight _flight;
        private Seat _seat1;
        private Seat _seat2;
        private Seat _seat3;
        private Ticket _ticket;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new SeatRepository(_context);
            SeedData();
        }

        private void SeedData()
        {
            _context.Seats.RemoveRange(_context.Seats);
            _context.Planes.RemoveRange(_context.Planes);
            _context.Flights.RemoveRange(_context.Flights);
            _context.Tickets.RemoveRange(_context.Tickets);
            _context.SaveChanges();

            _plane = new Plane
            {
                Id = Guid.NewGuid(),
                Name = "Test Plane",
                Seats = new List<Seat>(),
                Flights = new List<Flight>()
            };

            _seat1 = new Seat
            {
                Id = Guid.NewGuid(),
                Row = 1,
                SeatLetter = SeatLetter.A,
                PlaneId = _plane.Id,
                Plane = _plane
            };

            _seat2 = new Seat
            {
                Id = Guid.NewGuid(),
                Row = 1,
                SeatLetter = SeatLetter.B,
                PlaneId = _plane.Id,
                Plane = _plane
            };

            _seat3 = new Seat
            {
                Id = Guid.NewGuid(),
                Row = 2,
                SeatLetter = SeatLetter.A,
                PlaneId = _plane.Id,
                Plane = _plane
            };

            _plane.Seats.Add(_seat1);
            _plane.Seats.Add(_seat2);
            _plane.Seats.Add(_seat3);

            _flight = new Flight
            {
                Id = Guid.NewGuid(),
                FlightNumber = "FLTEST",
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(4),
                DepartureAirportId = Guid.NewGuid(),
                ArrivalAirportId = Guid.NewGuid(),
                PlaneId = _plane.Id,
                Plane = _plane,
                TicketPrice = 100m,
                Tickets = new List<Ticket>()
            };

            _ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                SeatId = _seat1.Id,
                Seat = _seat1,
                FlightId = _flight.Id,
                Flight = _flight,
                PersonType = PersonType.Adult,
                UserId = "user1",
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male,
                LuggageSize = LuggageSize.Medium,
                Price = 100m,
                Reserved = true,
                CreatedAt = DateTime.UtcNow
            };

            _seat1.Tickets.Add(_ticket);
            _flight.Tickets.Add(_ticket);

            _context.Planes.Add(_plane);
            _context.Flights.Add(_flight);
            _context.Seats.AddRange(_seat1, _seat2, _seat3);
            _context.Tickets.Add(_ticket);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task InsertAsync_ShouldAddNewSeat()
        {
            var newSeat = new Seat
            {
                Id = Guid.NewGuid(),
                Row = 3,
                SeatLetter = SeatLetter.C,
                PlaneId = _plane.Id,
                Plane = _plane
            };

            var result = await _repository.InsertAsync(newSeat);
            Assert.IsTrue(result);
            Assert.IsTrue(await _context.Seats.AnyAsync(s => s.Id == newSeat.Id));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateExistingSeat()
        {
            var seat = _context.Seats.First(s => s.Id == _seat2.Id);
            seat.Row = 5;
            var result = await _repository.UpdateAsync(seat);
            Assert.IsTrue(result);
            var updated = await _context.Seats.FirstOrDefaultAsync(s => s.Id == seat.Id);
            Assert.AreEqual(5, updated.Row);
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkSeatAsDeleted()
        {
            var seat = _context.Seats.First(s => s.Id == _seat3.Id);
            var result = await _repository.DeleteAsync(seat);
            Assert.IsTrue(result);
            Assert.IsNotNull(seat.DeletedAt);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrueWhenSeatExists()
        {
            bool exists = await _repository.ExistsAsync(s => s.Id == _seat1.Id);
            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalseWhenSeatDoesNotExist()
        {
            bool exists = await _repository.ExistsAsync(s => s.Id == Guid.NewGuid());
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllSeats()
        {
            var seats = await _repository.GetAllAsync(trackChanges: false);
            Assert.AreEqual(3, seats.Count());
        }

        [Test]
        public async Task GetByAsync_ShouldReturnMatchingSeat()
        {
            var seat = await _repository.GetByAsync(s => s.Id == _seat1.Id);
            Assert.IsNotNull(seat);
            Assert.AreEqual(_seat1.Id, seat.Id);
        }

        [Test]
        public async Task GetByAsync_ShouldReturnNullWhenNoMatch()
        {
            var seat = await _repository.GetByAsync(s => s.Id == Guid.NewGuid());
            Assert.IsNull(seat);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnSeatWhenExists()
        {
            var seat = await _repository.GetByIdAsync(_seat2.Id, trackChanges: false);
            Assert.IsNotNull(seat);
            Assert.AreEqual(_seat2.Id, seat.Id);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNullWhenNotExists()
        {
            var seat = await _repository.GetByIdAsync(Guid.NewGuid(), trackChanges: false);
            Assert.IsNull(seat);
        }

        [Test]
        public async Task GetPageCount_ShouldReturnCorrectPageCount()
        {
            int pageCount = await _repository.GetPageCount(2);
            Assert.AreEqual(2, pageCount);
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnPagedSeats()
        {
            int pageSize = 2;
            var page1 = await _repository.GetPagedAsync(trackChanges: false, page: 1, size: pageSize);
            var page2 = await _repository.GetPagedAsync(trackChanges: false, page: 2, size: pageSize);
            Assert.AreEqual(2, page1.Count());
            Assert.AreEqual(1, page2.Count());
            Assert.AreNotEqual(page1.First().Id, page2.First().Id);
        }

        [Test]
        public async Task GenerateSeatsForPlane_ShouldGenerateCorrectNumberOfSeats()
        {
            var initialCount = await _context.Seats.CountAsync(s => s.PlaneId == _plane.Id);
            int availableSeats = 2;
            int seatsPerRow = (int)SeatLetter.F;
            bool result = await _repository.GenerateSeatsForPlane(availableSeats, _plane.Id);
            Assert.IsTrue(result);
            var finalCount = await _context.Seats.CountAsync(s => s.PlaneId == _plane.Id);
            Assert.AreEqual(initialCount + availableSeats * seatsPerRow + 2, finalCount);
        }

        [Test]
        public async Task GetPagedForFlightAsync_ShouldReturnSeatsWithoutTicketsForFlight()
        {
            int pageSize = 5;
            var seats = await _repository.GetPagedForFlightAsync(_flight.Id, trackChanges: false, page: 1, size: pageSize);
            Assert.IsTrue(seats.All(s => !s.Tickets.Any(t => t.FlightId == _flight.Id)));
            Assert.AreEqual(2, seats.Count());
        }

        [Test]
        public async Task GetFreeSeatsForFlightAsync_ShouldReturnFreeSeats()
        {
            int size = 2;
            var seats = await _repository.GetFreeSeatsForFlightAsync(_flight.Id, trackChanges: false, size: size);
            Assert.IsTrue(seats.All(s => !s.Tickets.Any(t => t.FlightId == _flight.Id)));
            Assert.LessOrEqual(seats.Count(), size);
        }
    }
}
