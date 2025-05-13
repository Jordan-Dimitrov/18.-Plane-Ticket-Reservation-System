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
    internal class FlightRepositoryTests
    {
        private ApplicationDbContext _context;
        private IFlightRepository _repository;
        private Airport _departureAirport;
        private Airport _arrivalAirport;
        private Plane _plane;
        private Flight _flight1;
        private Flight _flight2;
        private Flight _flight3;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new FlightRepository(_context);
            SeedData();
        }

        private void SeedData()
        {
            _context.Flights.RemoveRange(_context.Flights);
            _context.Airports.RemoveRange(_context.Airports);
            _context.Planes.RemoveRange(_context.Planes);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            _departureAirport = new Airport { Id = Guid.NewGuid(), Name = "Departure Airport" };
            _arrivalAirport = new Airport { Id = Guid.NewGuid(), Name = "Arrival Airport" };

            var seats = new List<Seat>();
            for (int i = 0; i < 10; i++)
            {
                seats.Add(new Seat { Id = Guid.NewGuid() });
            }
            _plane = new Plane { Id = Guid.NewGuid(), Name = "Boeing 737", Seats = seats };

            var user = new User { Id = "user1", UserName = "TestUser" };

            _flight1 = new Flight
            {
                FlightNumber = "FL100",
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(4),
                DepartureAirport = _departureAirport,
                DepartureAirportId = _departureAirport.Id,
                ArrivalAirport = _arrivalAirport,
                ArrivalAirportId = _arrivalAirport.Id,
                Plane = _plane,
                PlaneId = _plane.Id,
                TicketPrice = 150.00m,
                Tickets = new List<Ticket>()
            };

            _flight2 = new Flight
            {
                FlightNumber = "FL200",
                DepartureTime = DateTime.UtcNow.AddHours(3),
                ArrivalTime = DateTime.UtcNow.AddHours(5),
                DepartureAirport = _departureAirport,
                DepartureAirportId = _departureAirport.Id,
                ArrivalAirport = _arrivalAirport,
                ArrivalAirportId = _arrivalAirport.Id,
                Plane = _plane,
                PlaneId = _plane.Id,
                TicketPrice = 200.00m,
                Tickets = new List<Ticket>()
            };

            _flight3 = new Flight
            {
                FlightNumber = "FL300",
                DepartureTime = DateTime.UtcNow.AddHours(1),
                ArrivalTime = DateTime.UtcNow.AddHours(3),
                DepartureAirport = _departureAirport,
                DepartureAirportId = _departureAirport.Id,
                ArrivalAirport = _arrivalAirport,
                ArrivalAirportId = _arrivalAirport.Id,
                Plane = _plane,
                PlaneId = _plane.Id,
                TicketPrice = 180.00m,
                Tickets = new List<Ticket>()
            };

            var seat1 = _plane.Seats.First();
            var seat2 = _plane.Seats.Skip(1).First();

            var ticket1 = new Ticket
            {
                Seat = seat1,
                SeatId = seat1.Id,
                Flight = _flight2,
                FlightId = _flight2.Id,
                PersonType = PersonType.Adult,
                User = user,
                UserId = user.Id,
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male,
                LuggageSize = LuggageSize.Medium,
                Price = 200.00m,
                Reserved = true,
                CreatedAt = DateTime.UtcNow
            };

            var ticket2 = new Ticket
            {
                Seat = seat2,
                SeatId = seat2.Id,
                Flight = _flight2,
                FlightId = _flight2.Id,
                PersonType = PersonType.Kid,
                User = user,
                UserId = user.Id,
                PersonFirstName = "Jane",
                PersonLastName = "Doe",
                Gender = Gender.Female,
                LuggageSize = LuggageSize.Small,
                Price = 150.00m,
                Reserved = true,
                CreatedAt = DateTime.UtcNow
            };

            _flight2.Tickets.Add(ticket1);
            _flight2.Tickets.Add(ticket2);

            _context.Users.Add(user);
            _context.Airports.AddRange(_departureAirport, _arrivalAirport);
            _context.Planes.Add(_plane);
            _context.Flights.AddRange(_flight1, _flight2, _flight3);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task InsertAsync_ShouldAddNewFlight()
        {
            var newFlight = new Flight
            {
                FlightNumber = "FL400",
                DepartureTime = DateTime.UtcNow.AddHours(4),
                ArrivalTime = DateTime.UtcNow.AddHours(6),
                DepartureAirport = _departureAirport,
                DepartureAirportId = _departureAirport.Id,
                ArrivalAirport = _arrivalAirport,
                ArrivalAirportId = _arrivalAirport.Id,
                Plane = _plane,
                PlaneId = _plane.Id,
                TicketPrice = 220.00m,
                Tickets = new List<Ticket>()
            };

            var result = await _repository.InsertAsync(newFlight);

            Assert.IsTrue(result);
            Assert.IsTrue(await _context.Flights.AnyAsync(f => f.Id == newFlight.Id));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateExistingFlight()
        {
            var flight = _context.Flights.First();
            flight.FlightNumber = "FL100-Updated";

            var result = await _repository.UpdateAsync(flight);

            Assert.IsTrue(result);
            var updated = await _context.Flights.FirstOrDefaultAsync(f => f.Id == flight.Id);
            Assert.AreEqual("FL100-Updated", updated.FlightNumber);
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkFlightAndTicketsAsDeleted()
        {
            var flight = await _context.Flights.Include(f => f.Tickets).FirstOrDefaultAsync(f => f.Id == _flight2.Id);
            var result = await _repository.DeleteAsync(flight);

            Assert.IsTrue(result);
            Assert.IsNotNull(flight.DeletedAt);
            foreach (var ticket in flight.Tickets)
            {
                Assert.IsNotNull(ticket.DeletedAt);
            }
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrueWhenFlightExists()
        {
            bool exists = await _repository.ExistsAsync(f => f.Id == _flight1.Id);
            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalseWhenFlightDoesNotExist()
        {
            bool exists = await _repository.ExistsAsync(f => f.Id == Guid.NewGuid());
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllFlights()
        {
            var flights = await _repository.GetAllAsync(trackChanges: false);
            Assert.AreEqual(3, flights.Count());
        }

        [Test]
        public async Task GetByAsync_ShouldReturnMatchingFlight()
        {
            var flight = await _repository.GetByAsync(f => f.Id == _flight1.Id);
            Assert.IsNotNull(flight);
            Assert.AreEqual(_flight1.Id, flight.Id);
        }

        [Test]
        public async Task GetByAsync_ShouldReturnNullWhenNoMatch()
        {
            var flight = await _repository.GetByAsync(f => f.Id == Guid.NewGuid());
            Assert.IsNull(flight);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnFlightWhenExists()
        {
            var flight = await _repository.GetByIdAsync(_flight1.Id, trackChanges: false);
            Assert.IsNotNull(flight);
            Assert.AreEqual(_flight1.Id, flight.Id);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNullWhenNotExists()
        {
            var flight = await _repository.GetByIdAsync(Guid.NewGuid(), trackChanges: false);
            Assert.IsNull(flight);
        }

        [Test]
        public async Task GetPageCount_ShouldReturnCorrectPageCount()
        {
            int pageCount = await _repository.GetPageCount(1);
            Assert.AreEqual(3, pageCount);
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnPagedFlights()
        {
            int pageSize = 2;
            var page1 = await _repository.GetPagedAsync(trackChanges: false, page: 1, size: pageSize);
            var page2 = await _repository.GetPagedAsync(trackChanges: false, page: 2, size: pageSize);
            Assert.AreEqual(2, page1.Count());
            Assert.AreEqual(1, page2.Count());
            Assert.AreNotEqual(page1.First().Id, page2.First().Id);
        }

        [Test]
        public async Task GetPagedByArrivalAirportIdAsync_ShouldReturnFlightsForGivenArrivalAirport()
        {
            int pageSize = 2;
            var flights = await _repository.GetPagedByArrivalAirportIdAsync(_arrivalAirport.Id, trackChanges: false, page: 1, size: pageSize);
            Assert.IsTrue(flights.All(f => f.ArrivalAirportId == _arrivalAirport.Id));
        }

        [Test]
        public async Task GetPagedByDepartingAirportIdAsync_ShouldReturnFlightsForGivenDepartureAirport()
        {
            int pageSize = 2;
            var flights = await _repository.GetPagedByDepartingAirportIdAsync(_departureAirport.Id, trackChanges: false, page: 1, size: pageSize);
            Assert.IsTrue(flights.All(f => f.DepartureAirportId == _departureAirport.Id));
        }

        [Test]
        public async Task GetPagedByPlaneIdAsync_ShouldReturnFlightsForGivenPlane()
        {
            int pageSize = 2;
            var flights = await _repository.GetPagedByPlaneIdAsync(_plane.Id, trackChanges: false, page: 1, size: pageSize);
            Assert.IsTrue(flights.All(f => f.PlaneId == _plane.Id));
        }

        [Test]
        public async Task GetPagedByArrivalAndDepartureAirportsAsync_ShouldReturnFlightsWithSufficientSeats()
        {
            int pageSize = 2;
            int requiredSeats = 5;
            var flights = await _repository.GetPagedByArrivalAndDepartureAirportsAsync(_departureAirport.Id, _arrivalAirport.Id, trackChanges: false, requiredSeats, page: 1, size: pageSize);
            Assert.IsTrue(flights.All(f => (f.Plane.Seats.Count - f.Tickets.Count) >= requiredSeats));
        }

        [Test]
        public async Task GetPagedByArrivalAndDepartureAsync_ShouldReturnFlightsOnSpecifiedDate()
        {
            int pageSize = 2;
            int requiredSeats = 5;
            DateTime departureDate = _flight1.DepartureTime.Date;
            var flights = await _repository.GetPagedByArrivalAndDepartureAsync(_departureAirport.Id, _arrivalAirport.Id, departureDate, trackChanges: false, requiredSeats, page: 1, size: pageSize);
            Assert.IsTrue(flights.All(f => f.DepartureTime.Date == departureDate));
        }

        [Test]
        public async Task GetPagedByArrivalAndDepartureWithoutConcreteDateAsync_ShouldReturnFlightsFromSpecifiedDateOnwards()
        {
            int pageSize = 2;
            int requiredSeats = 5;
            DateTime departureDate = _flight3.DepartureTime.Date.AddDays(-1);
            var flights = await _repository.GetPagedByArrivalAndDepartureWithoutConcreteDateAsync(_departureAirport.Id, _arrivalAirport.Id, departureDate, trackChanges: false, requiredSeats, page: 1, size: pageSize);
            Assert.IsTrue(flights.All(f => f.DepartureTime.Date >= departureDate));
        }
    }
}
