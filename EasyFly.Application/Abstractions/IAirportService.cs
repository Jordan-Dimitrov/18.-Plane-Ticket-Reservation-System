using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;

namespace EasyFly.Application.Abstractions
{
    public interface IAirportService
    {
        Task<Response> CreateAirport(AirportDto airport);
        Task<Response> DeleteAirport(Guid id);
        Task<Response> UpdateAirport(AirportDto airport, Guid id);
        Task<DataResponse<AirportViewModel>> GetAirport(Guid id);
        Task<DataResponse<AirportPagedViewModel>> GetAirportsPaged(int page, int size);
    }
}
