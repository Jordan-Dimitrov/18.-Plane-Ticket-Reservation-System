using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.Abstractions
{
    public interface IFlightService
    {
        Task<Response> CreateFlight(FlightDto flight);
        Task<Response> DeleteFlight(Guid id);
        Task<Response> UpdateFlight(FlightDto flight, Guid id);
        Task<DataResponse<FlightViewModel>> GetFlight(Guid id);
        Task<DataResponse<FlightPagedViewModel>> GetFlightsPaged(int page, int size);
        Task<DataResponse<FlightPagedViewModel>> GetFlightsPagedByPlane(Guid planeId, int page, int size);
    }
}
