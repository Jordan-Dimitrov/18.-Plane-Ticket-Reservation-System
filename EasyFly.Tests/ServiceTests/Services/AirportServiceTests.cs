using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using EasyFly.Infrastructure.Services;
using Moq;
using System.Linq.Expressions;

namespace EasyFly.Tests.ServiceTests.Services
{
    [TestFixture]
    public class AirportServiceTests
    {
        private Mock<IAirportRepository> _airportRepositoryMock;
        private AirportService _airportService;

        [SetUp]
        public void Setup()
        {
            _airportRepositoryMock = new Mock<IAirportRepository>();
            _airportService = new AirportService(_airportRepositoryMock.Object);
        }

        [Test]
        public async Task CreateAirport_ShouldReturnError_WhenAirportAlreadyExists()
        {
            var airportDto = new AirportDto { Name = "Test Airport", Latitude = 10, Longtitude = 20 };
            _airportRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Airport, bool>>>()))
                .ReturnsAsync(true);
            var response = await _airportService.CreateAirport(airportDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.AirportExists, response.ErrorMessage);
        }

        [Test]
        public async Task CreateAirport_ShouldReturnError_WhenInsertFails()
        {
            var airportDto = new AirportDto { Name = "Test Airport", Latitude = 10, Longtitude = 20 };
            _airportRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Airport, bool>>>()))
                .ReturnsAsync(false);
            _airportRepositoryMock
                .Setup(repo => repo.InsertAsync(It.IsAny<Airport>()))
                .ReturnsAsync(false);
            var response = await _airportService.CreateAirport(airportDto);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task CreateAirport_ShouldReturnSuccess_WhenInsertSucceeds()
        {
            var airportDto = new AirportDto { Name = "Test Airport", Latitude = 10, Longtitude = 20 };
            _airportRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Airport, bool>>>()))
                .ReturnsAsync(false);
            _airportRepositoryMock
                .Setup(repo => repo.InsertAsync(It.IsAny<Airport>()))
                .ReturnsAsync(true);
            var response = await _airportService.CreateAirport(airportDto);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task DeleteAirport_ShouldReturnError_WhenAirportNotFound()
        {
            var airportId = Guid.NewGuid();
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(airportId, true)).ReturnsAsync((Airport)null);
            var response = await _airportService.DeleteAirport(airportId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.AirportNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task DeleteAirport_ShouldReturnError_WhenDeleteFails()
        {
            var airportId = Guid.NewGuid();
            var airport = new Airport { Id = airportId, Name = "Test Airport", Latitude = 10, Longtitude = 20 };
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(airportId, true)).ReturnsAsync(airport);
            _airportRepositoryMock.Setup(repo => repo.DeleteAsync(airport)).ReturnsAsync(false);
            var response = await _airportService.DeleteAirport(airportId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task DeleteAirport_ShouldReturnSuccess_WhenDeleteSucceeds()
        {
            var airportId = Guid.NewGuid();
            var airport = new Airport { Id = airportId, Name = "Test Airport", Latitude = 10, Longtitude = 20 };
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(airportId, true)).ReturnsAsync(airport);
            _airportRepositoryMock.Setup(repo => repo.DeleteAsync(airport)).ReturnsAsync(true);
            var response = await _airportService.DeleteAirport(airportId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
        }

        [Test]
        public async Task GetAirport_ShouldReturnError_WhenAirportNotFound()
        {
            var airportId = Guid.NewGuid();
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(airportId, false)).ReturnsAsync((Airport)null);
            var response = await _airportService.GetAirport(airportId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.AirportNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task GetAirport_ShouldReturnAirportViewModel_WhenFound()
        {
            var airportId = Guid.NewGuid();
            var airport = new Airport { Id = airportId, Name = "Test Airport", Latitude = 10, Longtitude = 20 };
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(airportId, false)).ReturnsAsync(airport);
            var response = await _airportService.GetAirport(airportId);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.AreEqual(airport.Id, response.Data.Id);
            Assert.AreEqual(airport.Name, response.Data.Name);
            Assert.AreEqual(airport.Latitude, response.Data.Latitude);
            Assert.AreEqual(airport.Longtitude, response.Data.Longtitude);
        }

        [Test]
        public async Task GetAirportsPaged_ShouldReturnEmpty_WhenNoAirportsFound()
        {
            int page = 1, size = 10;
            _airportRepositoryMock.Setup(repo => repo.GetPagedAsync(false, page, size)).ReturnsAsync(new List<Airport>());
            _airportRepositoryMock.Setup(repo => repo.GetPageCount(size)).ReturnsAsync(0);
            var response = await _airportService.GetAirportsPaged(page, size);
            Assert.IsNotNull(response.Data);
            Assert.IsNull(response.Data.Airports);
            Assert.AreEqual(0, response.Data.TotalPages);
        }

        [Test]
        public async Task GetAirportsPaged_ShouldReturnAirportsAndPageCount_WhenAirportsExist()
        {
            int page = 1, size = 10;
            var airports = new List<Airport>
            {
                new Airport { Id = Guid.NewGuid(), Name = "Airport 1", Latitude = 10, Longtitude = 20 },
                new Airport { Id = Guid.NewGuid(), Name = "Airport 2", Latitude = 30, Longtitude = 40 }
            };
            _airportRepositoryMock.Setup(repo => repo.GetPagedAsync(false, page, size)).ReturnsAsync(airports);
            _airportRepositoryMock.Setup(repo => repo.GetPageCount(size)).ReturnsAsync(5);
            var response = await _airportService.GetAirportsPaged(page, size);
            Assert.NotNull(response.Data);
            Assert.AreEqual(2, response.Data.Airports.Count());
            Assert.AreEqual(5, response.Data.TotalPages);
        }

        [Test]
        public async Task UpdateAirport_ShouldReturnError_WhenAirportNotFound()
        {
            var airportId = Guid.NewGuid();
            var airportDto = new AirportDto { Name = "Updated Airport", Latitude = 15, Longtitude = 25 };
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(airportId, true)).ReturnsAsync((Airport)null);
            var response = await _airportService.UpdateAirport(airportDto, airportId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.AirportNotFound, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateAirport_ShouldReturnError_WhenUpdateFails()
        {
            var airportId = Guid.NewGuid();
            var airportDto = new AirportDto { Name = "Updated Airport", Latitude = 15, Longtitude = 25 };
            var existingAirport = new Airport { Id = airportId, Name = "Test Airport", Latitude = 10, Longtitude = 20 };
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(airportId, true)).ReturnsAsync(existingAirport);
            _airportRepositoryMock.Setup(repo => repo.UpdateAsync(existingAirport)).ReturnsAsync(false);
            var response = await _airportService.UpdateAirport(airportDto, airportId);
            Assert.False(response.Success);
            Assert.AreEqual(ResponseConstants.Unexpected, response.ErrorMessage);
        }

        [Test]
        public async Task UpdateAirport_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            var airportId = Guid.NewGuid();
            var airportDto = new AirportDto { Name = "Updated Airport", Latitude = 15, Longtitude = 25 };
            var existingAirport = new Airport { Id = airportId, Name = "Test Airport", Latitude = 10, Longtitude = 20 };
            _airportRepositoryMock.Setup(repo => repo.GetByIdAsync(airportId, true)).ReturnsAsync(existingAirport);
            _airportRepositoryMock.Setup(repo => repo.UpdateAsync(existingAirport)).ReturnsAsync(true);
            var response = await _airportService.UpdateAirport(airportDto, airportId);
            Assert.True(response.Success);
            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(airportDto.Name, existingAirport.Name);
            Assert.AreEqual(airportDto.Latitude, existingAirport.Latitude);
            Assert.AreEqual(airportDto.Longtitude, existingAirport.Longtitude);
        }
    }
}
