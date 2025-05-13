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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyFly.Tests.ServiceTests.Services
{
    [TestFixture]
    public class FlightServiceTests
    {
        private Mock<IFlightRepository> _flightRepositoryMock;
        private Mock<IAirportRepository> _airportRepositoryMock;
        private Mock<IPlaneRepository> _planeRepositoryMock;
        private Mock<ISeatRepository> _seatRepositoryMock;
        private FlightService _flightService;

        [SetUp]
        public void Setup()
        {
            _flightRepositoryMock = new Mock<IFlightRepository>();
            _airportRepositoryMock = new Mock<IAirportRepository>();
            _planeRepositoryMock = new Mock<IPlaneRepository>();
            _seatRepositoryMock = new Mock<ISeatRepository>();
            _flightService = new FlightService(_flightRepositoryMock.Object, _airportRepositoryMock.Object, _planeRepositoryMock.Object, _seatRepositoryMock.Object);
        }

        [Test]
        public async Task CreateFlight_ShouldReturnError_WhenFlightAlreadyExists()
        {
            var flightDto = new FlightDto
            {
                FlightNumber = "AB123",
                DepartureAirportId = Guid.NewGuid(),
                ArrivalAirportId = Guid.NewGuid(),
                PlaneId = Guid.NewGuid(),
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(4),
                TicketPrice = 100
            };
            _flightRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(true);
            var response = await _flightService.CreateFlight(flightDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.FlightExists, response.ErrorMessage);
        }

        [Test]
        public async Task CreateFlight_ShouldReturnError_WhenInvalidData()
        {
            var flightDto = new FlightDto
            {
                FlightNumber = "AB123",
                DepartureAirportId = Guid.NewGuid(),
                ArrivalAirportId = Guid.NewGuid(),
                PlaneId = Guid.NewGuid(),
                DepartureTime = DateTime.UtcNow.AddHours(4),
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                TicketPrice = 100
            };
            _flightRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(false);
            _airportRepositoryMock
                .Setup(repo => repo.GetByIdAsync(flightDto.DepartureAirportId, true))
                .ReturnsAsync(new Airport { Id = flightDto.DepartureAirportId, Name = "Dep", Latitude = 0, Longtitude = 0 });
            _airportRepositoryMock
                .Setup(repo => repo.GetByIdAsync(flightDto.ArrivalAirportId, true))
                .ReturnsAsync(new Airport { Id = flightDto.ArrivalAirportId, Name = "Arr", Latitude = 0, Longtitude = 0 });
            _planeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(flightDto.PlaneId, true))
                .ReturnsAsync(new Plane { Id = flightDto.PlaneId, Name = "Plane", Seats = new List<Seat>() });
            var response = await _flightService.CreateFlight(flightDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.InvalidData, response.ErrorMessage);
        }

        [Test]
        public async Task CreateFlight_ShouldReturnError_WhenInsertFails()
        {
            var flightDto = new FlightDto
            {
                FlightNumber = "AB123",
                DepartureAirportId = Guid.NewGuid(),
                ArrivalAirportId = Guid.NewGuid(),
                PlaneId = Guid.NewGuid(),
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(4),
                TicketPrice = 100
            };
            _flightRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(false);
            var depAirport = new Airport { Id = flightDto.DepartureAirportId, Name = "Dep", Latitude = 0, Longtitude = 0 };
            var arrAirport = new Airport { Id = flightDto.ArrivalAirportId, Name = "Arr", Latitude = 0, Longtitude = 0 };
            var plane = new Plane { Id = flightDto.PlaneId, Name = "Plane", Seats = new List<Seat>() };
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.DepartureAirportId, true)).ReturnsAsync(depAirport);
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.ArrivalAirportId, true)).ReturnsAsync(arrAirport);
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.PlaneId, true)).ReturnsAsync(plane);
            _flightRepositoryMock.Setup(repo => repo.InsertAsync(It.IsAny<Flight>())).ReturnsAsync(false);
            var response = await _flightService.CreateFlight(flightDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task CreateFlight_ShouldReturnSuccess_WhenValidData()
        {
            var flightDto = new FlightDto
            {
                FlightNumber = "AB123",
                DepartureAirportId = Guid.NewGuid(),
                ArrivalAirportId = Guid.NewGuid(),
                PlaneId = Guid.NewGuid(),
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(4),
                TicketPrice = 100
            };
            _flightRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(false);
            var depAirport = new Airport { Id = flightDto.DepartureAirportId, Name = "Dep", Latitude = 0, Longtitude = 0 };
            var arrAirport = new Airport { Id = flightDto.ArrivalAirportId, Name = "Arr", Latitude = 0, Longtitude = 0 };
            var plane = new Plane { Id = flightDto.PlaneId, Name = "Plane", Seats = new List<Seat>() };
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.DepartureAirportId, true)).ReturnsAsync(depAirport);
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.ArrivalAirportId, true)).ReturnsAsync(arrAirport);
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.PlaneId, true)).ReturnsAsync(plane);
            _flightRepositoryMock.Setup(repo => repo.InsertAsync(It.IsAny<Flight>())).ReturnsAsync(true);
            var response = await _flightService.CreateFlight(flightDto);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task DeleteFlight_ShouldReturnError_WhenFlightNotFound()
        {
            var flightId = Guid.NewGuid();
            _flightRepositoryMock.Setup(repo => repo.GetByIdAsync(flightId, true)).ReturnsAsync((Flight)null);
            var response = await _flightService.DeleteFlight(flightId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.FlightNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task DeleteFlight_ShouldReturnError_WhenDeleteFails()
        {
            var flightId = Guid.NewGuid();
            var flight = new Flight
            {
                Id = flightId,
                FlightNumber = "AB123",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                DepartureAirport = new Airport { Id = Guid.NewGuid(), Name = "Dep", Latitude = 0, Longtitude = 0 },
                ArrivalAirport = new Airport { Id = Guid.NewGuid(), Name = "Arr", Latitude = 0, Longtitude = 0 },
                Plane = new Plane { Id = Guid.NewGuid(), Name = "Plane", Seats = new List<Seat>() }
            };
            _flightRepositoryMock.Setup(repo => repo.GetByIdAsync(flightId, true)).ReturnsAsync(flight);
            _flightRepositoryMock.Setup(repo => repo.DeleteAsync(flight)).ReturnsAsync(false);
            var response = await _flightService.DeleteFlight(flightId);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task DeleteFlight_ShouldReturnSuccess_WhenDeleteSucceeds()
        {
            var flightId = Guid.NewGuid();
            var flight = new Flight
            {
                Id = flightId,
                FlightNumber = "AB123",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                DepartureAirport = new Airport { Id = Guid.NewGuid(), Name = "Dep", Latitude = 0, Longtitude = 0 },
                ArrivalAirport = new Airport { Id = Guid.NewGuid(), Name = "Arr", Latitude = 0, Longtitude = 0 },
                Plane = new Plane { Id = Guid.NewGuid(), Name = "Plane", Seats = new List<Seat>() }
            };
            _flightRepositoryMock.Setup(repo => repo.GetByIdAsync(flightId, true)).ReturnsAsync(flight);
            _flightRepositoryMock.Setup(repo => repo.DeleteAsync(flight)).ReturnsAsync(true);
            var response = await _flightService.DeleteFlight(flightId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task GetFlight_ShouldReturnError_WhenFlightNotFound()
        {
            var flightId = Guid.NewGuid();
            _flightRepositoryMock.Setup(repo => repo.GetByIdAsync(flightId, false)).ReturnsAsync((Flight)null);
            var response = await _flightService.GetFlight(flightId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.FlightNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task GetFlight_ShouldReturnFlightViewModel_WhenFlightFound()
        {
            var flightId = Guid.NewGuid();
            var depAirport = new Airport { Id = Guid.NewGuid(), Name = "Dep", Latitude = 0, Longtitude = 0 };
            var arrAirport = new Airport { Id = Guid.NewGuid(), Name = "Arr", Latitude = 0, Longtitude = 0 };
            var plane = new Plane { Id = Guid.NewGuid(), Name = "Plane", Seats = new List<Seat> { new Seat(), new Seat() } };
            var flight = new Flight
            {
                Id = flightId,
                FlightNumber = "AB123",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                DepartureAirport = depAirport,
                ArrivalAirport = arrAirport,
                Plane = plane,
                DepartureAirportId = depAirport.Id,
                ArrivalAirportId = arrAirport.Id,
                PlaneId = plane.Id,
                TicketPrice = 100
            };
            _flightRepositoryMock.Setup(repo => repo.GetByIdAsync(flightId, false)).ReturnsAsync(flight);
            _seatRepositoryMock.Setup(repo => repo.GetFreeSeatsForFlightAsync(flightId, false, int.MaxValue))
                .ReturnsAsync(new List<Seat> { new Seat(), new Seat(), new Seat() });
            var response = await _flightService.GetFlight(flightId);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.AreEqual(flight.Id, response.Data.Id);
            Assert.AreEqual(flight.TicketPrice, response.Data.TicketPrice);
            Assert.AreEqual(3, response.Data.FreeSeatCount);
        }

        [Test]
        public async Task GetFlightsPaged_ShouldReturnEmpty_WhenNoFlights()
        {
            int page = 1, size = 10;
            _flightRepositoryMock.Setup(repo => repo.GetPagedAsync(false, page, size)).ReturnsAsync(new List<Flight>());
            var response = await _flightService.GetFlightsPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.IsNull(response.Data.Flights);
        }

        [Test]
        public async Task GetFlightsPaged_ShouldReturnFlightViewModels()
        {
            int page = 1, size = 10;
            var flightList = new List<Flight>
            {
                new Flight
                {
                    Id = Guid.NewGuid(),
                    FlightNumber = "AB123",
                    DepartureTime = DateTime.UtcNow,
                    ArrivalTime = DateTime.UtcNow.AddHours(2),
                    DepartureAirport = new Airport { Id = Guid.NewGuid(), Name = "Dep", Latitude = 0, Longtitude = 0 },
                    ArrivalAirport = new Airport { Id = Guid.NewGuid(), Name = "Arr", Latitude = 0, Longtitude = 0 },
                    Plane = new Plane { Id = Guid.NewGuid(), Name = "Plane", Seats = new List<Seat> { new Seat(), new Seat() } },
                    DepartureAirportId = Guid.NewGuid(),
                    ArrivalAirportId = Guid.NewGuid(),
                    PlaneId = Guid.NewGuid(),
                    TicketPrice = 100
                }
            };
            _flightRepositoryMock.Setup(repo => repo.GetPagedAsync(false, page, size)).ReturnsAsync(flightList);
            _seatRepositoryMock.Setup(repo => repo.GetFreeSeatsForFlightAsync(It.IsAny<Guid>(), false, int.MaxValue))
                .ReturnsAsync(new List<Seat> { new Seat(), new Seat() });
            _flightRepositoryMock.Setup(repo => repo.GetPageCount(size)).ReturnsAsync(5);
            var response = await _flightService.GetFlightsPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.IsNotEmpty(response.Data.Flights);
            Assert.AreEqual(5, response.Data.TotalPages);
        }

        [Test]
        public async Task UpdateFlight_ShouldReturnError_WhenFlightNotFound()
        {
            var flightId = Guid.NewGuid();
            var flightDto = new FlightDto
            {
                FlightNumber = "AB123",
                DepartureAirportId = Guid.NewGuid(),
                ArrivalAirportId = Guid.NewGuid(),
                PlaneId = Guid.NewGuid(),
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(4),
                TicketPrice = 100
            };
            _flightRepositoryMock.Setup(repo => repo.GetByIdAsync(flightId, true)).ReturnsAsync((Flight)null);
            var response = await _flightService.UpdateFlight(flightDto, flightId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.FlightNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateFlight_ShouldReturnError_WhenInvalidData()
        {
            var flightId = Guid.NewGuid();
            var flightDto = new FlightDto
            {
                FlightNumber = "AB123",
                DepartureAirportId = Guid.NewGuid(),
                ArrivalAirportId = Guid.NewGuid(),
                PlaneId = Guid.NewGuid(),
                DepartureTime = DateTime.UtcNow.AddHours(4),
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                TicketPrice = 100
            };
            var flight = new Flight
            {
                Id = flightId,
                FlightNumber = "Old",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                DepartureAirportId = flightDto.DepartureAirportId,
                ArrivalAirportId = flightDto.ArrivalAirportId,
                PlaneId = flightDto.PlaneId
            };
            _flightRepositoryMock.Setup(repo => repo.GetByIdAsync(flightId, true)).ReturnsAsync(flight);
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.DepartureAirportId, true))
                .ReturnsAsync(new Airport { Id = flightDto.DepartureAirportId, Name = "Dep", Latitude = 0, Longtitude = 0 });
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.ArrivalAirportId, true))
                .ReturnsAsync(new Airport { Id = flightDto.ArrivalAirportId, Name = "Arr", Latitude = 0, Longtitude = 0 });
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(flight.PlaneId, true))
                .ReturnsAsync(new Plane { Id = flightDto.PlaneId, Name = "Plane", Seats = new List<Seat>() });
            var response = await _flightService.UpdateFlight(flightDto, flightId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.InvalidData, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateFlight_ShouldReturnError_WhenUpdateFails()
        {
            var flightId = Guid.NewGuid();
            var flightDto = new FlightDto
            {
                FlightNumber = "AB123",
                DepartureAirportId = Guid.NewGuid(),
                ArrivalAirportId = Guid.NewGuid(),
                PlaneId = Guid.NewGuid(),
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(4),
                TicketPrice = 100
            };
            var flight = new Flight
            {
                Id = flightId,
                FlightNumber = "Old",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                DepartureAirportId = flightDto.DepartureAirportId,
                ArrivalAirportId = flightDto.ArrivalAirportId,
                PlaneId = flightDto.PlaneId
            };
            _flightRepositoryMock.Setup(repo => repo.GetByIdAsync(flightId, true)).ReturnsAsync(flight);
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.DepartureAirportId, true))
                .ReturnsAsync(new Airport { Id = flightDto.DepartureAirportId, Name = "Dep", Latitude = 0, Longtitude = 0 });
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.ArrivalAirportId, true))
                .ReturnsAsync(new Airport { Id = flightDto.ArrivalAirportId, Name = "Arr", Latitude = 0, Longtitude = 0 });
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(flight.PlaneId, true))
                .ReturnsAsync(new Plane { Id = flightDto.PlaneId, Name = "Plane", Seats = new List<Seat>() });
            _flightRepositoryMock.Setup(repo => repo.UpdateAsync(flight)).ReturnsAsync(false);
            var response = await _flightService.UpdateFlight(flightDto, flightId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateFlight_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            var flightId = Guid.NewGuid();
            var flightDto = new FlightDto
            {
                FlightNumber = "AB123",
                DepartureAirportId = Guid.NewGuid(),
                ArrivalAirportId = Guid.NewGuid(),
                PlaneId = Guid.NewGuid(),
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(4),
                TicketPrice = 100
            };
            var flight = new Flight
            {
                Id = flightId,
                FlightNumber = "Old",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                DepartureAirportId = flightDto.DepartureAirportId,
                ArrivalAirportId = flightDto.ArrivalAirportId,
                PlaneId = flightDto.PlaneId
            };
            _flightRepositoryMock.Setup(repo => repo.GetByIdAsync(flightId, true)).ReturnsAsync(flight);
            var depAirport = new Airport { Id = flightDto.DepartureAirportId, Name = "Dep", Latitude = 0, Longtitude = 0 };
            var arrAirport = new Airport { Id = flightDto.ArrivalAirportId, Name = "Arr", Latitude = 0, Longtitude = 0 };
            var plane = new Plane { Id = flightDto.PlaneId, Name = "Plane", Seats = new List<Seat> { new Seat(), new Seat() } };
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.DepartureAirportId, true)).ReturnsAsync(depAirport);
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(flightDto.ArrivalAirportId, true)).ReturnsAsync(arrAirport);
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(flight.PlaneId, true)).ReturnsAsync(plane);
            _flightRepositoryMock.Setup(repo => repo.UpdateAsync(flight)).ReturnsAsync(true);
            var response = await _flightService.UpdateFlight(flightDto, flightId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }
    }
}
