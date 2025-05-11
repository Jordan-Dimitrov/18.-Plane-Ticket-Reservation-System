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
    public class SeatServiceTests
    {
        private Mock<ISeatRepository> _seatRepositoryMock;
        private Mock<IPlaneRepository> _planeRepositoryMock;
        private SeatService _seatService;

        [SetUp]
        public void Setup()
        {
            _seatRepositoryMock = new Mock<ISeatRepository>();
            _planeRepositoryMock = new Mock<IPlaneRepository>();
            _seatService = new SeatService(_seatRepositoryMock.Object, _planeRepositoryMock.Object);
        }

        [Test]
        public async Task CreateSeat_ShouldReturnError_WhenSeatExists()
        {
            var seatDto = new SeatDto { Row = 1, SeatLetter = SeatLetter.A, PlaneId = Guid.NewGuid() };
            _seatRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(true);
            var response = await _seatService.CreateSeat(seatDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.SeatExists, response.ErrorMessage);
        }

        [Test]
        public async Task CreateSeat_ShouldReturnError_WhenPlaneNotFound()
        {
            var seatDto = new SeatDto { Row = 1, SeatLetter = SeatLetter.A, PlaneId = Guid.NewGuid() };
            _seatRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(false);
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(seatDto.PlaneId, false))
                .ReturnsAsync((Plane)null);
            var response = await _seatService.CreateSeat(seatDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.InvalidData, response.ErrorMessage);
        }

        [Test]
        public async Task CreateSeat_ShouldReturnError_WhenInsertFails()
        {
            var seatDto = new SeatDto { Row = 1, SeatLetter = SeatLetter.A, PlaneId = Guid.NewGuid() };
            _seatRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(false);
            var plane = new Plane { Id = seatDto.PlaneId, Name = "Test Plane", Seats = new List<Seat>() };
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(seatDto.PlaneId, false))
                .ReturnsAsync(plane);
            _seatRepositoryMock.Setup(repo => repo.InsertAsync(It.IsAny<Seat>()))
                .ReturnsAsync(false);
            var response = await _seatService.CreateSeat(seatDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task CreateSeat_ShouldReturnSuccess_WhenValidData()
        {
            var seatDto = new SeatDto { Row = 1, SeatLetter = SeatLetter.A, PlaneId = Guid.NewGuid() };
            _seatRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(false);
            var plane = new Plane { Id = seatDto.PlaneId, Name = "Test Plane", Seats = new List<Seat>() };
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(seatDto.PlaneId, false))
                .ReturnsAsync(plane);
            _seatRepositoryMock.Setup(repo => repo.InsertAsync(It.IsAny<Seat>()))
                .ReturnsAsync(true);
            var response = await _seatService.CreateSeat(seatDto);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task DeleteSeat_ShouldReturnError_WhenSeatNotFound()
        {
            var seatId = Guid.NewGuid();
            _seatRepositoryMock.Setup(repo => repo.GetByIdAsync(seatId, true))
                .ReturnsAsync((Seat)null);
            var response = await _seatService.DeleteSeat(seatId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.SeatNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task DeleteSeat_ShouldReturnError_WhenDeleteFails()
        {
            var seatId = Guid.NewGuid();
            var seat = new Seat { Id = seatId, Row = 1, SeatLetter = SeatLetter.A, PlaneId = Guid.NewGuid() };
            _seatRepositoryMock.Setup(repo => repo.GetByIdAsync(seatId, true))
                .ReturnsAsync(seat);
            _seatRepositoryMock.Setup(repo => repo.DeleteAsync(seat))
                .ReturnsAsync(false);
            var response = await _seatService.DeleteSeat(seatId);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task DeleteSeat_ShouldReturnSuccess_WhenDeleteSucceeds()
        {
            var seatId = Guid.NewGuid();
            var seat = new Seat { Id = seatId, Row = 1, SeatLetter = SeatLetter.A, PlaneId = Guid.NewGuid() };
            _seatRepositoryMock.Setup(repo => repo.GetByIdAsync(seatId, true))
                .ReturnsAsync(seat);
            _seatRepositoryMock.Setup(repo => repo.DeleteAsync(seat))
                .ReturnsAsync(true);
            var response = await _seatService.DeleteSeat(seatId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task GetSeat_ShouldReturnError_WhenSeatNotFound()
        {
            var seatId = Guid.NewGuid();
            _seatRepositoryMock.Setup(repo => repo.GetByIdAsync(seatId, false))
                .ReturnsAsync((Seat)null);
            var response = await _seatService.GetSeat(seatId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.SeatNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task GetSeat_ShouldReturnSeatViewModel_WhenSeatFound()
        {
            var seatId = Guid.NewGuid();
            var plane = new Plane { Id = Guid.NewGuid(), Name = "Test Plane", Seats = new List<Seat> { new Seat(), new Seat() } };
            var seat = new Seat { Id = seatId, Row = 1, SeatLetter = SeatLetter.A, PlaneId = plane.Id, Plane = plane };
            _seatRepositoryMock.Setup(repo => repo.GetByIdAsync(seatId, false))
                .ReturnsAsync(seat);
            var response = await _seatService.GetSeat(seatId);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.AreEqual(seat.Id, response.Data.Id);
            Assert.AreEqual(seat.Row, response.Data.Row);
            Assert.AreEqual(seat.SeatLetter, response.Data.SeatLetter);
            Assert.AreEqual(plane.Id, response.Data.Plane.Id);
        }

        [Test]
        public async Task GetSeatsPaged_ShouldReturnEmpty_WhenNoSeats()
        {
            int page = 1, size = 10;
            _seatRepositoryMock.Setup(repo => repo.GetPagedAsync(false, page, size))
                .ReturnsAsync(new List<Seat>());
            var response = await _seatService.GetSeatsPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.IsNull(response.Data.Seats);
        }

        [Test]
        public async Task GetSeatsPaged_ShouldReturnSeatViewModels()
        {
            int page = 1, size = 10;
            var plane = new Plane { Id = Guid.NewGuid(), Name = "Test Plane", Seats = new List<Seat> { new Seat(), new Seat() } };
            var seats = new List<Seat>
            {
                new Seat { Id = Guid.NewGuid(), Row = 1, SeatLetter = SeatLetter.A, PlaneId = plane.Id, Plane = plane },
                new Seat { Id = Guid.NewGuid(), Row = 1, SeatLetter = SeatLetter.B, PlaneId = plane.Id, Plane = plane }
            };
            _seatRepositoryMock.Setup(repo => repo.GetPagedAsync(false, page, size))
                .ReturnsAsync(seats);
            _seatRepositoryMock.Setup(repo => repo.GetPageCount(size)).ReturnsAsync(3);
            var response = await _seatService.GetSeatsPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.AreEqual(seats.Count, response.Data.Seats.Count());
            Assert.AreEqual(3, response.Data.TotalPages);
        }

        [Test]
        public async Task GetSeatsPagedForFlight_ShouldReturnEmpty_WhenNoSeats()
        {
            int page = 1, size = 10;
            var flightId = Guid.NewGuid();
            _seatRepositoryMock.Setup(repo => repo.GetPagedForFlightAsync(flightId, false, page, size))
                .ReturnsAsync(new List<Seat>());
            var response = await _seatService.GetSeatsPagedForFlight(flightId, page, size);
            Assert.NotNull(response.Data);
            Assert.IsNull(response.Data.Seats);
        }

        [Test]
        public async Task GetSeatsPagedForFlight_ShouldReturnSeatViewModels()
        {
            int page = 1, size = 10;
            var flightId = Guid.NewGuid();
            var plane = new Plane { Id = Guid.NewGuid(), Name = "Test Plane", Seats = new List<Seat> { new Seat(), new Seat() } };
            var seats = new List<Seat>
            {
                new Seat { Id = Guid.NewGuid(), Row = 1, SeatLetter = SeatLetter.A, PlaneId = plane.Id, Plane = plane },
                new Seat { Id = Guid.NewGuid(), Row = 2, SeatLetter = SeatLetter.B, PlaneId = plane.Id, Plane = plane }
            };
            _seatRepositoryMock.Setup(repo => repo.GetPagedForFlightAsync(flightId, false, page, size))
                .ReturnsAsync(seats);
            _seatRepositoryMock.Setup(repo => repo.GetPageCount(size)).ReturnsAsync(2);
            var response = await _seatService.GetSeatsPagedForFlight(flightId, page, size);
            Assert.NotNull(response.Data);
            Assert.AreEqual(seats.Count, response.Data.Seats.Count());
            Assert.AreEqual(2, response.Data.TotalPages);
        }

        [Test]
        public async Task UpdateSeat_ShouldReturnError_WhenSeatNotFound()
        {
            var seatId = Guid.NewGuid();
            var seatDto = new SeatDto { Row = 2, SeatLetter = SeatLetter.B, PlaneId = Guid.NewGuid() };
            _seatRepositoryMock.Setup(repo => repo.GetByIdAsync(seatId, true))
                .ReturnsAsync((Seat)null);
            var response = await _seatService.UpdateSeat(seatDto, seatId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.SeatNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateSeat_ShouldReturnError_WhenPlaneNotFound()
        {
            var seatId = Guid.NewGuid();
            var seatDto = new SeatDto { Row = 2, SeatLetter = SeatLetter.B, PlaneId = Guid.NewGuid() };
            var existingSeat = new Seat { Id = seatId, Row = 1, SeatLetter = SeatLetter.A, PlaneId = Guid.NewGuid() };
            _seatRepositoryMock.Setup(repo => repo.GetByIdAsync(seatId, true))
                .ReturnsAsync(existingSeat);
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(seatDto.PlaneId, true))
                .ReturnsAsync((Plane)null);
            var response = await _seatService.UpdateSeat(seatDto, seatId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.InvalidData, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateSeat_ShouldReturnError_WhenUpdateFails()
        {
            var seatId = Guid.NewGuid();
            var seatDto = new SeatDto { Row = 2, SeatLetter = SeatLetter.B, PlaneId = Guid.NewGuid() };
            var existingSeat = new Seat { Id = seatId, Row = 1, SeatLetter = SeatLetter.A, PlaneId = seatDto.PlaneId };
            var plane = new Plane { Id = seatDto.PlaneId, Name = "Test Plane", Seats = new List<Seat> { new Seat(), new Seat() } };
            _seatRepositoryMock.Setup(repo => repo.GetByIdAsync(seatId, true))
                .ReturnsAsync(existingSeat);
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(seatDto.PlaneId, true))
                .ReturnsAsync(plane);
            _seatRepositoryMock.Setup(repo => repo.UpdateAsync(existingSeat))
                .ReturnsAsync(false);
            var response = await _seatService.UpdateSeat(seatDto, seatId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateSeat_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            var seatId = Guid.NewGuid();
            var seatDto = new SeatDto { Row = 2, SeatLetter = SeatLetter.B, PlaneId = Guid.NewGuid() };
            var existingSeat = new Seat { Id = seatId, Row = 1, SeatLetter = SeatLetter.A, PlaneId = seatDto.PlaneId };
            var plane = new Plane { Id = seatDto.PlaneId, Name = "Test Plane", Seats = new List<Seat> { new Seat(), new Seat() } };
            _seatRepositoryMock.Setup(repo => repo.GetByIdAsync(seatId, true))
                .ReturnsAsync(existingSeat);
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(seatDto.PlaneId, true))
                .ReturnsAsync(plane);
            _seatRepositoryMock.Setup(repo => repo.UpdateAsync(existingSeat))
                .ReturnsAsync(true);
            var response = await _seatService.UpdateSeat(seatDto, seatId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(seatDto.Row, existingSeat.Row);
            Assert.AreEqual(seatDto.SeatLetter, existingSeat.SeatLetter);
            Assert.AreEqual(seatDto.PlaneId, existingSeat.PlaneId);
        }
    }
}
