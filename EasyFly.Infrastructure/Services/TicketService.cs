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
    internal class TicketService : ITicketService
    {
        public Task<Response> CreateTicket(TicketDto plane)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteTicket(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<TicketViewModel>> GetTicket(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<TicketPagedViewModel>> GetTicketsPaged(int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateTicket(TicketDto plane, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
