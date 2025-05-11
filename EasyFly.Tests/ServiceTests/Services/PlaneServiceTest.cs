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
using EasyFly.Domain.Abstractions;

namespace EasyFly.Tests.ServiceTests.Services
{
    [TestFixture]
    public class PlaneServiceTests
    {
        private Mock<IPlaneRepository> _planeRepositoryMock;
        private Mock<ISeatRepository> _seatRepositoryMock;
        private PlaneService _planeService;

        [SetUp]
        public void Setup()
        {
            _planeRepositoryMock = new Mock<IPlaneRepository>();
            _seatRepositoryMock = new Mock<ISeatRepository>();
            _planeService = new PlaneService(_planeRepositoryMock.Object, _seatRepositoryMock.Object);
        }

        [Test]
        public async Task CreatePlane_ShouldReturnError_WhenPlaneExists()
        {
            var planeDto = new PlaneDto { Name = "Test Plane", AvailableSeats = 100 };
            _planeRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Plane, bool>>>()))
                .ReturnsAsync(true);
            var response = await _planeService.CreatePlane(planeDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.PlaneExists, response.ErrorMessage);
        }

        [Test]
        public async Task CreatePlane_ShouldReturnError_WhenInsertFails()
        {
            var planeDto = new PlaneDto { Name = "Test Plane", AvailableSeats = 100 };
            _planeRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Plane, bool>>>()))
                .ReturnsAsync(false);
            _planeRepositoryMock.Setup(repo => repo.InsertAsync(It.IsAny<Plane>()))
                .ReturnsAsync(false);
            var response = await _planeService.CreatePlane(planeDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task CreatePlane_ShouldReturnError_WhenGenerateSeatsFails()
        {
            var planeDto = new PlaneDto { Name = "Test Plane", AvailableSeats = 100 };
            _planeRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Plane, bool>>>()))
                .ReturnsAsync(false);
            _planeRepositoryMock.Setup(repo => repo.InsertAsync(It.IsAny<Plane>()))
                .ReturnsAsync(true);
            _seatRepositoryMock.Setup(repo => repo.GenerateSeatsForPlane(planeDto.AvailableSeats, It.IsAny<Guid>()))
                .ReturnsAsync(false);
            var response = await _planeService.CreatePlane(planeDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task CreatePlane_ShouldReturnSuccess_WhenValidData()
        {
            var planeDto = new PlaneDto { Name = "Test Plane", AvailableSeats = 100 };
            _planeRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Plane, bool>>>()))
                .ReturnsAsync(false);
            _planeRepositoryMock.Setup(repo => repo.InsertAsync(It.IsAny<Plane>()))
                .ReturnsAsync(true);
            _seatRepositoryMock.Setup(repo => repo.GenerateSeatsForPlane(planeDto.AvailableSeats, It.IsAny<Guid>()))
                .ReturnsAsync(true);
            var response = await _planeService.CreatePlane(planeDto);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task DeletePlane_ShouldReturnError_WhenPlaneNotFound()
        {
            var planeId = Guid.NewGuid();
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(planeId, true))
                .ReturnsAsync((Plane)null);
            var response = await _planeService.DeletePlane(planeId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.PlaneNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task DeletePlane_ShouldReturnError_WhenDeleteFails()
        {
            var planeId = Guid.NewGuid();
            var plane = new Plane { Id = planeId, Name = "Test Plane", Seats = new List<Seat>() };
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(planeId, true))
                .ReturnsAsync(plane);
            _planeRepositoryMock.Setup(repo => repo.DeleteAsync(plane))
                .ReturnsAsync(false);
            var response = await _planeService.DeletePlane(planeId);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task DeletePlane_ShouldReturnSuccess_WhenDeleteSucceeds()
        {
            var planeId = Guid.NewGuid();
            var plane = new Plane { Id = planeId, Name = "Test Plane", Seats = new List<Seat>() };
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(planeId, true))
                .ReturnsAsync(plane);
            _planeRepositoryMock.Setup(repo => repo.DeleteAsync(plane))
                .ReturnsAsync(true);
            var response = await _planeService.DeletePlane(planeId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task GetPlane_ShouldReturnError_WhenPlaneNotFound()
        {
            var planeId = Guid.NewGuid();
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(planeId, false))
                .ReturnsAsync((Plane)null);
            var response = await _planeService.GetPlane(planeId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.PlaneNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task GetPlane_ShouldReturnPlaneViewModel_WhenPlaneFound()
        {
            var planeId = Guid.NewGuid();
            var seats = new List<Seat> { new Seat(), new Seat() };
            var plane = new Plane { Id = planeId, Name = "Test Plane", Seats = seats };
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(planeId, false))
                .ReturnsAsync(plane);
            var response = await _planeService.GetPlane(planeId);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.AreEqual(plane.Id, response.Data.Id);
            Assert.AreEqual(plane.Name, response.Data.Name);
            Assert.AreEqual(seats.Count, response.Data.Seats);
        }

        [Test]
        public async Task GetPlanesPaged_ShouldReturnEmpty_WhenNoPlanes()
        {
            int page = 1, size = 10;
            _planeRepositoryMock.Setup(repo => repo.GetPagedAsync(false, page, size))
                .ReturnsAsync(new List<Plane>());
            var response = await _planeService.GetPlanesPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.IsNull(response.Data.Planes);
        }

        [Test]
        public async Task GetPlanesPaged_ShouldReturnPlaneViewModels()
        {
            int page = 1, size = 10;
            var planes = new List<Plane>
            {
                new Plane { Id = Guid.NewGuid(), Name = "Plane1", Seats = new List<Seat> { new Seat(), new Seat() } },
                new Plane { Id = Guid.NewGuid(), Name = "Plane2", Seats = new List<Seat> { new Seat() } }
            };
            _planeRepositoryMock.Setup(repo => repo.GetPagedAsync(false, page, size))
                .ReturnsAsync(planes);
            _planeRepositoryMock.Setup(repo => repo.GetPageCount(size)).ReturnsAsync(3);
            var response = await _planeService.GetPlanesPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.AreEqual(planes.Count, response.Data.Planes.Count());
            Assert.AreEqual(3, response.Data.TotalPages);
        }

        [Test]
        public async Task UpdatePlane_ShouldReturnError_WhenPlaneNotFound()
        {
            var planeId = Guid.NewGuid();
            var planeDto = new PlaneDto { Name = "Updated Plane", AvailableSeats = 100 };
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(planeId, true))
                .ReturnsAsync((Plane)null);
            var response = await _planeService.UpdatePlane(planeDto, planeId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.PlaneNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task UpdatePlane_ShouldReturnError_WhenUpdateFails()
        {
            var planeId = Guid.NewGuid();
            var planeDto = new PlaneDto { Name = "Updated Plane", AvailableSeats = 100 };
            var plane = new Plane { Id = planeId, Name = "Old Plane", Seats = new List<Seat> { new Seat(), new Seat() } };
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(planeId, true))
                .ReturnsAsync(plane);
            _planeRepositoryMock.Setup(repo => repo.UpdateAsync(plane))
                .ReturnsAsync(false);
            var response = await _planeService.UpdatePlane(planeDto, planeId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task UpdatePlane_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            var planeId = Guid.NewGuid();
            var planeDto = new PlaneDto { Name = "Updated Plane", AvailableSeats = 100 };
            var plane = new Plane { Id = planeId, Name = "Old Plane", Seats = new List<Seat> { new Seat(), new Seat() } };
            _planeRepositoryMock.Setup(repo => repo.GetByIdAsync(planeId, true))
                .ReturnsAsync(plane);
            _planeRepositoryMock.Setup(repo => repo.UpdateAsync(plane))
                .ReturnsAsync(true);
            var response = await _planeService.UpdatePlane(planeDto, planeId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(planeDto.Name, plane.Name);
        }
    }
}
