using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Enums;
using EasyFly.Domain.Models;
using EasyFly.Persistence;
using EasyFly.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EasyFly.Tests.ServiceTests.Repositories
{
    [TestFixture]
    internal class TicketRepositoryTests
    {
        private ApplicationDbContext _context;
        private ITicketRepository _repository;
        private Flight _flight;
        private Seat _seat;
        private Ticket _ticket1;
        private Ticket _ticket2;
        private Ticket _ticket3;
        private Plane _plane;
        private Airport _departureAirport;
        private Airport _arrivalAirport;
        private User _user1;
        private User _user2;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new TicketRepository(_context);
            SeedData();
        }

        private void SeedData()
        {
            _context.Tickets.RemoveRange(_context.Tickets);
            _context.Seats.RemoveRange(_context.Seats);
            _context.Flights.RemoveRange(_context.Flights);
            _context.Planes.RemoveRange(_context.Planes);
            _context.Airports.RemoveRange(_context.Airports);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            _plane = new Plane
            {
                Id = Guid.NewGuid(),
                Name = "Test Plane"
            };

            _departureAirport = new Airport
            {
                Id = Guid.NewGuid(),
                Name = "Departure Airport"
            };

            _arrivalAirport = new Airport
            {
                Id = Guid.NewGuid(),
                Name = "Arrival Airport"
            };

            _user1 = new User
            {
                Id = "user1",
                UserName = "John Doe"
            };

            _user2 = new User
            {
                Id = "user2",
                UserName = "Jane Smith"
            };

            _seat = new Seat
            {
                Id = Guid.NewGuid(),
                Row = 1,
                SeatLetter = SeatLetter.A,
                PlaneId = _plane.Id,
                Plane = _plane
            };

            _flight = new Flight
            {
                Id = Guid.NewGuid(),
                FlightNumber = "FLTEST",
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(4),
                DepartureAirportId = _departureAirport.Id,
                ArrivalAirportId = _arrivalAirport.Id,
                DepartureAirport = _departureAirport,
                ArrivalAirport = _arrivalAirport,
                PlaneId = _plane.Id,
                Plane = _plane,
                TicketPrice = 100m
            };

            _ticket1 = new Ticket
            {
                Id = Guid.NewGuid(),
                SeatId = _seat.Id,
                Seat = _seat,
                FlightId = _flight.Id,
                Flight = _flight,
                PersonType = PersonType.Adult,
                UserId = _user1.Id,
                User = _user1,
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male,
                LuggageSize = LuggageSize.Medium,
                Price = 100m,
                Reserved = true,
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            };

            _ticket2 = new Ticket
            {
                Id = Guid.NewGuid(),
                SeatId = _seat.Id,
                Seat = _seat,
                FlightId = _flight.Id,
                Flight = _flight,
                PersonType = PersonType.Kid,
                UserId = _user2.Id,
                User = _user2,
                PersonFirstName = "Jane",
                PersonLastName = "Smith",
                Gender = Gender.Female,
                LuggageSize = LuggageSize.Small,
                Price = 80m,
                Reserved = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-20)
            };

            _ticket3 = new Ticket
            {
                Id = Guid.NewGuid(),
                SeatId = _seat.Id,
                Seat = _seat,
                FlightId = _flight.Id,
                Flight = _flight,
                PersonType = PersonType.Adult,
                UserId = _user1.Id,
                User = _user1,
                PersonFirstName = "Alice",
                PersonLastName = "Johnson",
                Gender = Gender.Female,
                LuggageSize = LuggageSize.Large,
                Price = 120m,
                Reserved = true,
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            };

            _context.Planes.Add(_plane);
            _context.Airports.AddRange(_departureAirport, _arrivalAirport);
            _context.Users.AddRange(_user1, _user2);
            _context.Seats.Add(_seat);
            _context.Flights.Add(_flight);
            _context.Tickets.AddRange(_ticket1, _ticket2, _ticket3);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Count_ShouldReturnReservedTicketCount()
        {
            int count = await _repository.Count();
            Assert.AreEqual(2, count);
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkTicketAsDeleted()
        {
            var result = await _repository.DeleteAsync(_ticket1);
            Assert.IsTrue(result);
            Assert.IsNotNull(_ticket1.DeletedAt);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrueWhenTicketExists()
        {
            bool exists = await _repository.ExistsAsync(t => t.Id == _ticket2.Id);
            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalseWhenTicketDoesNotExist()
        {
            bool exists = await _repository.ExistsAsync(t => t.Id == Guid.NewGuid());
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllTickets()
        {
            var tickets = await _repository.GetAllAsync(false);
            Assert.AreEqual(3, tickets.Count());
            Assert.IsTrue(tickets.First().CreatedAt >= tickets.Last().CreatedAt);
        }

        [Test]
        public async Task GetByAsync_ShouldReturnMatchingTicket()
        {
            var ticket = await _repository.GetByAsync(t => t.UserId == _user2.Id);
            Assert.IsNotNull(ticket);
            Assert.AreEqual(_ticket2.Id, ticket.Id);
        }

        [Test]
        public async Task GetByAsync_ShouldReturnNullWhenNoMatch()
        {
            var ticket = await _repository.GetByAsync(t => t.UserId == "nonexistent");
            Assert.IsNull(ticket);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnTicketWhenExists()
        {
            var ticket = await _repository.GetByIdAsync(_ticket3.Id, false);
            Assert.IsNotNull(ticket);
            Assert.AreEqual(_ticket3.Id, ticket.Id);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNullWhenNotExists()
        {
            var ticket = await _repository.GetByIdAsync(Guid.NewGuid(), false);
            Assert.IsNull(ticket);
        }

        [Test]
        public async Task GetPageCount_ShouldReturnCorrectPageCount()
        {
            int pageCount = await _repository.GetPageCount(2);
            Assert.AreEqual(2, pageCount);
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnPagedTickets()
        {
            int pageSize = 2;
            var page1 = await _repository.GetPagedAsync(false, 1, pageSize);
            var page2 = await _repository.GetPagedAsync(false, 2, pageSize);
            Assert.AreEqual(2, page1.Count());
            Assert.AreEqual(1, page2.Count());
            Assert.AreNotEqual(page1.First().Id, page2.First().Id);
        }

        [Test]
        public async Task GetPagedByFlightIdAsync_ShouldReturnTicketsForFlight()
        {
            int pageSize = 2;
            var tickets = await _repository.GetPagedByFlightIdAsync(_flight.Id, false, 1, pageSize, null, null, null);
            Assert.AreEqual(2, tickets.Count());
            Assert.IsTrue(tickets.All(t => t.FlightId == _flight.Id));
        }

        [Test]
        public async Task GetPagedByUserIdAsync_ShouldReturnTicketsForUser()
        {
            int pageSize = 2;
            var tickets = await _repository.GetPagedByUserIdAsync(_user1.Id, false, 1, pageSize, null, null, null);
            Assert.AreEqual(2, tickets.Count());
            Assert.IsTrue(tickets.All(t => t.UserId == _user1.Id));
        }

        [Test]
        public async Task InsertAsync_ShouldAddNewTicket()
        {
            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                SeatId = _seat.Id,
                Seat = _seat,
                FlightId = _flight.Id,
                Flight = _flight,
                PersonType = PersonType.Adult,
                UserId = "user3",
                User = new User { Id = "user3", UserName = "Bob Brown" },
                PersonFirstName = "Bob",
                PersonLastName = "Brown",
                Gender = Gender.Male,
                LuggageSize = LuggageSize.Small,
                Price = 90m,
                Reserved = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _repository.InsertAsync(newTicket);
            Assert.IsTrue(result);
            Assert.IsTrue(await _context.Tickets.AnyAsync(t => t.Id == newTicket.Id));
        }

        [Test]
        public async Task InsertBulkAsync_ShouldAddMultipleTickets()
        {
            var ticketList = new List<Ticket>
            {
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    SeatId = _seat.Id,
                    Seat = _seat,
                    FlightId = _flight.Id,
                    Flight = _flight,
                    PersonType = PersonType.Kid,
                    UserId = "user4",
                    User = new User { Id = "user4", UserName = "Sam Green" },
                    PersonFirstName = "Sam",
                    PersonLastName = "Green",
                    Gender = Gender.Male,
                    LuggageSize = LuggageSize.Medium,
                    Price = 70m,
                    Reserved = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    SeatId = _seat.Id,
                    Seat = _seat,
                    FlightId = _flight.Id,
                    Flight = _flight,
                    PersonType = PersonType.Adult,
                    UserId = "user5",
                    User = new User { Id = "user5", UserName = "Lisa White" },
                    PersonFirstName = "Lisa",
                    PersonLastName = "White",
                    Gender = Gender.Female,
                    LuggageSize = LuggageSize.Large,
                    Price = 110m,
                    Reserved = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
            var result = await _repository.InsertBulkAsync(ticketList);
            Assert.IsTrue(result);
            Assert.AreEqual(5, await _context.Tickets.CountAsync());
        }

        [Test]
        public async Task RemoveUnreservedTickets_ShouldRemoveTicketsThatAreUnreservedAndRecent()
        {
            var unreservedTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                SeatId = _seat.Id,
                Seat = _seat,
                FlightId = _flight.Id,
                Flight = _flight,
                PersonType = PersonType.Adult,
                UserId = "user6",
                User = new User { Id = "user6", UserName = "Tom Hardy" },
                PersonFirstName = "Tom",
                PersonLastName = "Hardy",
                Gender = Gender.Male,
                LuggageSize = LuggageSize.Medium,
                Price = 95m,
                Reserved = false,
                CreatedAt = DateTime.UtcNow
            };
            await _repository.InsertAsync(unreservedTicket);
            int beforeCount = await _context.Tickets.CountAsync();
            var result = await _repository.RemoveUnreservedTickets();
            int afterCount = await _context.Tickets.CountAsync();
            Assert.IsTrue(result);
            Assert.Less(afterCount, beforeCount);
            Assert.IsFalse(await _context.Tickets.AnyAsync(t => t.Id == unreservedTicket.Id));
        }

        [Test]
        public async Task UpdateAsync_ShouldModifyTicket()
        {
            _ticket2.Price = 85m;
            var result = await _repository.UpdateAsync(_ticket2);
            Assert.IsTrue(result);
            var updated = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == _ticket2.Id);
            Assert.AreEqual(85m, updated.Price);
        }

        [Test]
        public async Task UpdateTicketStatus_ShouldMarkTicketsAsReserved()
        {
            var ticketIds = new List<Guid> { _ticket2.Id };
            var result = await _repository.UpdateTicketStatus(ticketIds);
            Assert.IsTrue(result);
            var updatedTicket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == _ticket2.Id);
            Assert.IsTrue(updatedTicket.Reserved);
        }
    }
}
