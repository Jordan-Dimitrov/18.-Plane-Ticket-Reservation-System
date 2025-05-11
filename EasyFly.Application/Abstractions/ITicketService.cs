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
    public interface ITicketService
    {
        Task<Response> CreateTicket(TicketDto plane);
        Task<DataResponse<CheckoutDto>> CreateTickets(List<ReserveTicketDto> ticketDtos);
        Task<Response> DeleteTicket(Guid id);
        Task<Response> UpdateTicket(TicketDto plane, Guid id);
        Task<DataResponse<TicketViewModel>> GetTicket(Guid id);
        Task<DataResponse<TicketPagedViewModel>> GetTicketsPaged(
             int page, int size,
             string? searchTerm,
             string? personType,
             string? luggageSize);

        Task<DataResponse<TicketPagedViewModel>> GetTicketsPagedByUserId(
            string userId, int page, int size,
            string? searchTerm,
            string? personType,
            string? luggageSize);

        Task<DataResponse<TicketPagedViewModel>> GetTicketsPagedByFlightId(
            Guid flightId, int page, int size,
            string? searchTerm,
            string? personType,
            string? luggageSize);

        Task<DataResponse<int>> GetTicketCount();
        Task<Response> UpdateTicketStatus(List<Guid> tickets);
        Task<Response> RemoveUnreservedTickets();
    }
}
