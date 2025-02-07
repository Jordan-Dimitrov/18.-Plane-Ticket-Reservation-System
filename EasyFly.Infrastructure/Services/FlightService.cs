using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Infrastructure.Services
{
    internal class FlightService : IFlightService
    {
        public Task<Response> CreateFlight(FlightDto flight)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteFlight(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<FlightViewModel>> GetFlight(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<FlightPagedViewModel>> GetFlightsPaged(int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateFlight(FlightDto flight, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
