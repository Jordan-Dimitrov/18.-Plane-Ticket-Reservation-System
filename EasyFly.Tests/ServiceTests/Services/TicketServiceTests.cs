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
using EasyFly.Domain.Enums;
using EasyFly.Domain.Abstractions;

namespace EasyFly.Tests.ServiceTests.Services
{
    [TestFixture]
    public class TicketServiceTests
    {
        private Mock<ITicketRepository> _ticketRepositoryMock;
        private Mock<ISeatRepository> _seatRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IFlightRepository> _flightRepositoryMock;
        private TicketService _ticketService;

        [SetUp]
        public void Setup()
        {
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _seatRepositoryMock = new Mock<ISeatRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _flightRepositoryMock = new Mock<IFlightRepository>();
            _ticketService = new TicketService(
                _ticketRepositoryMock.Object,
                _seatRepositoryMock.Object,
                _userRepositoryMock.Object,
                _flightRepositoryMock.Object);
        }

        [Test]
        public async Task CreateTicket_ShouldReturnError_WhenInvalidData()
        {
            var ticketDto = new TicketDto
            {
                SeatId = Guid.NewGuid(),
                UserId = "user1",
                FlightId = Guid.NewGuid(),
                LuggageSize = LuggageSize.Large,
                Price = 100,
                PersonType = PersonType.Adult,
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male
            };
            _seatRepositoryMock.Setup(r => r.GetByIdAsync(ticketDto.SeatId, true))
                .ReturnsAsync((Seat)null);
            var response = await _ticketService.CreateTicket(ticketDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.InvalidData, response.ErrorMessage);
        }

        [Test]
        public async Task CreateTicket_ShouldReturnError_WhenInsertFails()
        {
            var ticketDto = new TicketDto
            {
                SeatId = Guid.NewGuid(),
                UserId = "user1",
                FlightId = Guid.NewGuid(),
                LuggageSize = LuggageSize.Large,
                Price = 100,
                PersonType = PersonType.Adult,
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male
            };
            var seat = new Seat { Id = ticketDto.SeatId };
            var user = new User { Id = ticketDto.UserId, UserName = "user1", Email = "user1@test.com" };
            var flight = new Flight { Id = ticketDto.FlightId, TicketPrice = 100 };
            _seatRepositoryMock.Setup(r => r.GetByIdAsync(ticketDto.SeatId, true))
                .ReturnsAsync(seat);
            _userRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);
            _flightRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(flight);
            _ticketRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Ticket>()))
                .ReturnsAsync(false);
            var response = await _ticketService.CreateTicket(ticketDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task CreateTicket_ShouldReturnSuccess_WhenValidData()
        {
            var ticketDto = new TicketDto
            {
                SeatId = Guid.NewGuid(),
                UserId = "user1",
                FlightId = Guid.NewGuid(),
                LuggageSize = LuggageSize.Large,
                Price = 100,
                PersonType = PersonType.Adult,
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male
            };
            var seat = new Seat { Id = ticketDto.SeatId };
            var user = new User { Id = ticketDto.UserId, UserName = "user1", Email = "user1@test.com" };
            var flight = new Flight { Id = ticketDto.FlightId, TicketPrice = 100 };
            _seatRepositoryMock.Setup(r => r.GetByIdAsync(ticketDto.SeatId, true))
                .ReturnsAsync(seat);
            _userRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);
            _flightRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(flight);
            _ticketRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Ticket>()))
                .ReturnsAsync(true);
            var response = await _ticketService.CreateTicket(ticketDto);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task CreateTickets_ShouldReturnError_WhenNotEnoughSeats()
        {
            var reserveDtos = new List<ReserveTicketDto>
            {
                new ReserveTicketDto
                {
                    FlightId = Guid.NewGuid(),
                    UserId = "user1",
                    LuggageSize = LuggageSize.Large,
                    PersonType = PersonType.Adult,
                    PersonFirstName = "John",
                    PersonLastName = "Doe",
                    Gender = Gender.Male
                },
                new ReserveTicketDto
                {
                    FlightId = Guid.NewGuid(),
                    UserId = "user1",
                    LuggageSize = LuggageSize.Large,
                    PersonType = PersonType.Adult,
                    PersonFirstName = "Jane",
                    PersonLastName = "Doe",
                    Gender = Gender.Female
                }
            };
            var flight = new Flight { Id = reserveDtos[0].FlightId, TicketPrice = 100 };
            var user = new User { Id = "user1", UserName = "user1", Email = "user1@test.com" };
            _flightRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(flight);
            _userRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);
            _seatRepositoryMock.Setup(r => r.GetFreeSeatsForFlightAsync(flight.Id, true, reserveDtos.Count))
                .ReturnsAsync(new List<Seat> { new Seat() });
            var response = await _ticketService.CreateTickets(reserveDtos);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.InvalidData, response.ErrorMessage);
        }

        [Test]
        public async Task CreateTickets_ShouldReturnSuccess_WhenValidData()
        {
            var reserveDtos = new List<ReserveTicketDto>
            {
                new ReserveTicketDto
                {
                    FlightId = Guid.NewGuid(),
                    UserId = "user1",
                    LuggageSize = LuggageSize.Large,
                    PersonType = PersonType.Adult,
                    PersonFirstName = "John",
                    PersonLastName = "Doe",
                    Gender = Gender.Male
                },
                new ReserveTicketDto
                {
                    FlightId = Guid.NewGuid(),
                    UserId = "user1",
                    LuggageSize = LuggageSize.Large,
                    PersonType = PersonType.Adult,
                    PersonFirstName = "Jane",
                    PersonLastName = "Doe",
                    Gender = Gender.Female
                }
            };
            var flight = new Flight { Id = reserveDtos[0].FlightId, TicketPrice = 100, ArrivalAirport = new Airport { Name = "Arr" } };
            var user = new User { Id = "user1", UserName = "user1", Email = "user1@test.com" };
            _flightRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(flight);
            _userRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);
            _seatRepositoryMock.Setup(r => r.GetFreeSeatsForFlightAsync(flight.Id, true, reserveDtos.Count))
                .ReturnsAsync(new List<Seat> { new Seat(), new Seat() });
            _ticketRepositoryMock.Setup(r => r.InsertBulkAsync(It.IsAny<List<Ticket>>()))
                .ReturnsAsync(true);
            var response = await _ticketService.CreateTickets(reserveDtos);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.IsNotEmpty(response.Data.Tickets);
        }

        [Test]
        public async Task DeleteTicket_ShouldReturnError_WhenTicketNotFound()
        {
            var ticketId = Guid.NewGuid();
            _ticketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId, true))
                .ReturnsAsync((Ticket)null);
            var response = await _ticketService.DeleteTicket(ticketId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.TicketNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task DeleteTicket_ShouldReturnError_WhenDeleteFails()
        {
            var ticketId = Guid.NewGuid();
            var ticket = new Ticket { Id = ticketId };
            _ticketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId, true))
                .ReturnsAsync(ticket);
            _ticketRepositoryMock.Setup(r => r.DeleteAsync(ticket))
                .ReturnsAsync(false);
            var response = await _ticketService.DeleteTicket(ticketId);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task DeleteTicket_ShouldReturnSuccess_WhenDeleteSucceeds()
        {
            var ticketId = Guid.NewGuid();
            var ticket = new Ticket { Id = ticketId };
            _ticketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId, true))
                .ReturnsAsync(ticket);
            _ticketRepositoryMock.Setup(r => r.DeleteAsync(ticket))
                .ReturnsAsync(true);
            var response = await _ticketService.DeleteTicket(ticketId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task GetTicket_ShouldReturnError_WhenTicketNotFound()
        {
            var ticketId = Guid.NewGuid();
            _ticketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId, false))
                .ReturnsAsync((Ticket)null);
            var response = await _ticketService.GetTicket(ticketId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.TicketNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task GetTicket_ShouldReturnTicketViewModel_WhenTicketFound()
        {
            var ticketId = Guid.NewGuid();
            var seat = new Seat
            {
                Id = Guid.NewGuid(),
                Row = 1,
                SeatLetter = SeatLetter.A,
                Plane = new Plane { Id = Guid.NewGuid(), Name = "Test Plane", Seats = new List<Seat> { new Seat(), new Seat() } }
            };
            var user = new User { Id = "user1", UserName = "user1", Email = "user1@test.com", PhoneNumber = "123456" };
            var flight = new Flight
            {
                Id = Guid.NewGuid(),
                FlightNumber = "FL123",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                ArrivalAirport = new Airport { Name = "Arr" },
                DepartureAirport = new Airport { Name = "Dep" }
            };
            var ticket = new Ticket
            {
                Id = ticketId,
                Seat = seat,
                PersonType = PersonType.Adult,
                User = user,
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male,
                Price = 100,
                LuggageSize = LuggageSize.Large,
                Flight = flight,
                FlightId = flight.Id,
                CreatedAt = DateTime.UtcNow
            };
            _ticketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId, false))
                .ReturnsAsync(ticket);
            var response = await _ticketService.GetTicket(ticketId);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.AreEqual(ticket.Id, response.Data.Id);
            Assert.AreEqual(seat.Id, response.Data.Seat.Id);
        }

        [Test]
        public async Task GetTicketCount_ShouldReturnCount()
        {
            _ticketRepositoryMock.Setup(r => r.Count()).ReturnsAsync(5);
            var response = await _ticketService.GetTicketCount();
            Assert.AreEqual(5, response.Data);
        }

        [Test]
        public async Task GetTicketsPaged_ShouldReturnEmpty_WhenNoTickets()
        {
            int page = 1, size = 10;
            _ticketRepositoryMock.Setup(r => r.GetPagedAsync(false, page, size))
                .ReturnsAsync(new List<Ticket>());
            var response = await _ticketService.GetTicketsPaged(page, size, null, null, null);
            Assert.NotNull(response.Data);
            Assert.IsNull(response.Data.Tickets);
        }

        [Test]
        public async Task GetTicketsPagedByFlightId_ShouldReturnEmpty_WhenNoTickets()
        {
            int page = 1, size = 10;
            var flightId = Guid.NewGuid();
            _ticketRepositoryMock.Setup(r => r.GetPagedByFlightIdAsync(flightId, false, page, size, null, null, null))
                .ReturnsAsync(new List<Ticket>());
            var response = await _ticketService.GetTicketsPagedByFlightId(flightId, page, size, null, null, null);
            Assert.NotNull(response.Data);
            Assert.IsNull(response.Data.Tickets);
        }

        [Test]
        public async Task GetTicketsPagedByFlightId_ShouldReturnTicketViewModels()
        {
            int page = 1, size = 10;
            var flightId = Guid.NewGuid();
            var seat = new Seat
            {
                Id = Guid.NewGuid(),
                Row = 1,
                SeatLetter = SeatLetter.A,
                Plane = new Plane { Id = Guid.NewGuid(), Name = "Test Plane", Seats = new List<Seat> { new Seat(), new Seat() } }
            };
            var user = new User { Id = "user1", UserName = "user1", Email = "user1@test.com", PhoneNumber = "123456" };
            var flight = new Flight
            {
                Id = flightId,
                FlightNumber = "FL123",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                ArrivalAirport = new Airport { Name = "Arr" },
                DepartureAirport = new Airport { Name = "Dep" }
            };
            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Seat = seat,
                    PersonType = PersonType.Adult,
                    User = user,
                    PersonFirstName = "John",
                    PersonLastName = "Doe",
                    Gender = Gender.Male,
                    Price = 100,
                    LuggageSize = LuggageSize.Large,
                    Flight = flight,
                    FlightId = flightId,
                    CreatedAt = DateTime.UtcNow
                }
            };
            _ticketRepositoryMock.Setup(r => r.GetPagedByFlightIdAsync(flightId, false, page, size, null, null, null))
                .ReturnsAsync(tickets);
            _ticketRepositoryMock.Setup(r => r.GetPageCount(size)).ReturnsAsync(2);
            var response = await _ticketService.GetTicketsPagedByFlightId(flightId, page, size, null, null, null);
            Assert.NotNull(response.Data);
            Assert.AreEqual(tickets.Count, response.Data.Tickets.Count());
            Assert.AreEqual(0, response.Data.TotalPages);
        }

        [Test]
        public async Task GetTicketsPagedByUserId_ShouldReturnEmpty_WhenNoTickets()
        {
            int page = 1, size = 10;
            string userId = "user1";
            _ticketRepositoryMock.Setup(r => r.GetPagedByUserIdAsync(userId, false, page, size, null, null, null))
                .ReturnsAsync(new List<Ticket>());
            var response = await _ticketService.GetTicketsPagedByUserId(userId, page, size, null, null, null);
            Assert.NotNull(response.Data);
            Assert.IsNull(response.Data.Tickets);
        }

        [Test]
        public async Task GetTicketsPagedByUserId_ShouldReturnTicketViewModels()
        {
            int page = 1, size = 10;
            string userId = "user1";
            var seat = new Seat
            {
                Id = Guid.NewGuid(),
                Row = 1,
                SeatLetter = SeatLetter.A,
                Plane = new Plane { Id = Guid.NewGuid(), Name = "Test Plane", Seats = new List<Seat> { new Seat(), new Seat() } }
            };
            var user = new User { Id = userId, UserName = "user1", Email = "user1@test.com", PhoneNumber = "123456" };
            var flight = new Flight
            {
                FlightNumber = "FL123",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                ArrivalAirport = new Airport { Name = "Arr" },
                DepartureAirport = new Airport { Name = "Dep" }
            };
            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Seat = seat,
                    PersonType = PersonType.Adult,
                    User = user,
                    PersonFirstName = "John",
                    PersonLastName = "Doe",
                    Gender = Gender.Male,
                    Price = 100,
                    LuggageSize = LuggageSize.Large,
                    Flight = flight,
                    FlightId = flight.Id,
                    CreatedAt = DateTime.UtcNow
                }
            };
            _ticketRepositoryMock.Setup(r => r.GetPagedByUserIdAsync(userId, false, page, size, null, null, null))
                .ReturnsAsync(tickets);
            _ticketRepositoryMock.Setup(r => r.GetPageCount(size)).ReturnsAsync(1);
            var response = await _ticketService.GetTicketsPagedByUserId(userId, page, size, null, null, null);
            Assert.NotNull(response.Data);
            Assert.AreEqual(tickets.Count, response.Data.Tickets.Count());
            Assert.AreEqual(0, response.Data.TotalPages);
        }

        [Test]
        public async Task UpdateTicket_ShouldReturnError_WhenTicketNotFound()
        {
            var ticketId = Guid.NewGuid();
            var ticketDto = new TicketDto
            {
                SeatId = Guid.NewGuid(),
                UserId = "user1",
                FlightId = Guid.NewGuid(),
                LuggageSize = LuggageSize.Large,
                Price = 100,
                PersonType = PersonType.Adult,
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male
            };
            _ticketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId, true))
                .ReturnsAsync((Ticket)null);
            var response = await _ticketService.UpdateTicket(ticketDto, ticketId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.TicketNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateTicket_ShouldReturnError_WhenInvalidData()
        {
            var ticketId = Guid.NewGuid();
            var ticketDto = new TicketDto
            {
                SeatId = Guid.NewGuid(),
                UserId = "user1",
                FlightId = Guid.NewGuid(),
                LuggageSize = LuggageSize.Large,
                Price = 100,
                PersonType = PersonType.Adult,
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male
            };
            var existingTicket = new Ticket { Id = ticketId };
            _ticketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId, true))
                .ReturnsAsync(existingTicket);
            _seatRepositoryMock.Setup(r => r.GetByIdAsync(ticketDto.SeatId, true))
                .ReturnsAsync((Seat)null);
            var response = await _ticketService.UpdateTicket(ticketDto, ticketId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.InvalidData, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateTicket_ShouldReturnError_WhenUpdateFails()
        {
            var ticketId = Guid.NewGuid();
            var ticketDto = new TicketDto
            {
                SeatId = Guid.NewGuid(),
                UserId = "user1",
                FlightId = Guid.NewGuid(),
                LuggageSize = LuggageSize.Large,
                Price = 100,
                PersonType = PersonType.Adult,
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male
            };
            var existingTicket = new Ticket { Id = ticketId };
            var seat = new Seat { Id = ticketDto.SeatId };
            var user = new User { Id = "user1", UserName = "user1", Email = "user1@test.com" };
            var flight = new Flight { Id = ticketDto.FlightId, TicketPrice = 100 };
            _ticketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId, true))
                .ReturnsAsync(existingTicket);
            _seatRepositoryMock.Setup(r => r.GetByIdAsync(ticketDto.SeatId, true))
                .ReturnsAsync(seat);
            _userRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);
            _flightRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(flight);
            _ticketRepositoryMock.Setup(r => r.UpdateAsync(existingTicket))
                .ReturnsAsync(false);
            var response = await _ticketService.UpdateTicket(ticketDto, ticketId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateTicket_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            var ticketId = Guid.NewGuid();
            var ticketDto = new TicketDto
            {
                SeatId = Guid.NewGuid(),
                UserId = "user1",
                FlightId = Guid.NewGuid(),
                LuggageSize = LuggageSize.Large,
                Price = 100,
                PersonType = PersonType.Adult,
                PersonFirstName = "John",
                PersonLastName = "Doe",
                Gender = Gender.Male
            };
            var existingTicket = new Ticket { Id = ticketId };
            var seat = new Seat { Id = ticketDto.SeatId };
            var user = new User { Id = "user1", UserName = "user1", Email = "user1@test.com" };
            var flight = new Flight { Id = ticketDto.FlightId, TicketPrice = 100 };
            _ticketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId, true))
                .ReturnsAsync(existingTicket);
            _seatRepositoryMock.Setup(r => r.GetByIdAsync(ticketDto.SeatId, true))
                .ReturnsAsync(seat);
            _userRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);
            _flightRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(flight);
            _ticketRepositoryMock.Setup(r => r.UpdateAsync(existingTicket))
                .ReturnsAsync(true);
            var response = await _ticketService.UpdateTicket(ticketDto, ticketId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task UpdateTicketStatus_ShouldReturnError_WhenUpdateFails()
        {
            var ticketIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            _ticketRepositoryMock.Setup(r => r.UpdateTicketStatus(ticketIds))
                .ReturnsAsync(false);
            var response = await _ticketService.UpdateTicketStatus(ticketIds);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateTicketStatus_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            var ticketIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            _ticketRepositoryMock.Setup(r => r.UpdateTicketStatus(ticketIds))
                .ReturnsAsync(true);
            var response = await _ticketService.UpdateTicketStatus(ticketIds);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }
    }
}
