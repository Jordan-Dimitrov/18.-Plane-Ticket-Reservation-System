using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;

namespace EasyFly.Infrastructure.Services
{
    public class AirportService : IAirportService
    {
        private readonly IAirportRepository _airportRepository;

        public AirportService(IAirportRepository airportRepository)
        {
            _airportRepository = airportRepository;
        }

        public async Task<Response> CreateAirport(AirportDto airport)
        {
            Response response = new Response();

            if (await _airportRepository.ExistsAsync(x => x.Name == airport.Name))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.AirportExists;
                return response;
            }

            Airport newAirport = new Airport()
            {
                Name = airport.Name,
                Latitude = airport.Latitude,
                Longtitude = airport.Longtitude
            };

            if (!await _airportRepository.InsertAsync(newAirport))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<Response> DeleteAirport(Guid id)
        {
            Response response = new Response();

            Airport? airport = await _airportRepository.GetByIdAsync(id, true);

            if (airport is null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.AirportNotFound;
                return response;
            }

            if (!await _airportRepository.DeleteAsync(airport))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<DataResponse<AirportViewModel>> GetAirport(Guid id)
        {
            DataResponse<AirportViewModel> response = new DataResponse<AirportViewModel>();

            Airport? airport = await _airportRepository.GetByIdAsync(id, false);

            if (airport is null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.AirportNotFound;
                return response;
            }

            response.Data = new AirportViewModel()
            {
                Id = airport.Id,
                Name = airport.Name,
                Latitude = airport.Latitude,
                Longtitude = airport.Longtitude,
            };

            return response;
        }

        public async Task<DataResponse<AirportPagedViewModel>> GetAirportsPaged(int page, int size)
        {
            DataResponse<AirportPagedViewModel> response = new DataResponse<AirportPagedViewModel>();
            response.Data = new AirportPagedViewModel();

            var airports = await _airportRepository.GetPagedAsync(false, page, size);

            if (!airports.Any())
            {
                return response;
            }

            response.Data.Airports = airports
                .Select(x => new AirportViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Latitude = x.Latitude,
                    Longtitude = x.Longtitude,
                });

            response.Data.TotalPages = await _airportRepository.GetPageCount(size);

            return response;
        }

        public async Task<Response> UpdateAirport(AirportDto airport, Guid id)
        {
            Response response = new Response();

            Airport? existingAirport = await _airportRepository.GetByIdAsync(id, true);

            if (existingAirport is null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.AirportNotFound;
                return response;
            }

            existingAirport.Name = airport.Name;
            existingAirport.Latitude = airport.Latitude;
            existingAirport.Longtitude = airport.Longtitude;

            if (!await _airportRepository.UpdateAsync(existingAirport))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }
    }
}