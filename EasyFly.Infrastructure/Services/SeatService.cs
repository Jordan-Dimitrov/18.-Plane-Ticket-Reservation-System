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
    internal class SeatService : ISeatService
    {
        public Task<Response> CreateSeat(SeatDto plane)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteSeat(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<SeatViewModel>> GetSeat(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<SeatPagedViewModel>> GetSeatsPaged(int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateSeat(SeatDto plane, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
