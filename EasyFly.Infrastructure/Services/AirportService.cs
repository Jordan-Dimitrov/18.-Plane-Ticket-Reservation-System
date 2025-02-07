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
    internal class AirportService : IAirportService
    {
        public Task<Response> CreateAirport(AirportDto airport)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteAirport(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<AirportViewModel>> GetAirport(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<AirportPagedViewModel>> GetAirportsPaged(int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateAirport(AirportDto airport, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
