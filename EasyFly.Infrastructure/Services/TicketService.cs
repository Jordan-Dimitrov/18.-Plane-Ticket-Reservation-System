using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;

namespace EasyFly.Infrastructure.Services
{
    internal class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IUserRepository _userRepository;

        public TicketService(ITicketRepository ticketRepository, ISeatRepository seatRepository, IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _seatRepository = seatRepository;
            _userRepository = userRepository;
        }

        public async Task<Response> CreateTicket(TicketDto ticket)
        {
            Response response = new Response();

            Seat seat = await _seatRepository.GetByIdAsync(ticket.SeatId, true);
            User user = await _userRepository.GetByAsync(x => x.Id == ticket.UserId);

            if (seat == null || user == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.InvalidData;
                return response;
            }

            Ticket newTicket = new Ticket()
            {
                SeatId = ticket.SeatId,
                PersonType = ticket.PersonType,
                UserId = ticket.UserId,
                PersonFirstName = ticket.PersonFirstName,
                PersonLastName = ticket.PersonLastName,
                Gender = ticket.Gender,
                Price = ticket.Price
            };

            if (!await _ticketRepository.InsertAsync(newTicket))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<Response> DeleteTicket(Guid id)
        {
            Response response = new Response();

            Ticket? ticket = await _ticketRepository.GetByIdAsync(id, true);

            if (ticket == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.TicketNotFound;
                return response;
            }

            if (!await _ticketRepository.DeleteAsync(ticket))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            response.Success = true;

            return response;
        }

        public async Task<DataResponse<TicketViewModel>> GetTicket(Guid id)
        {
            DataResponse<TicketViewModel> response = new DataResponse<TicketViewModel>();

            Ticket? ticket = await _ticketRepository.GetByIdAsync(id, false);

            if (ticket == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.TicketNotFound;
                return response;
            }

            response.Data = new TicketViewModel()
            {
                Id = ticket.Id,
                Seat = new SeatViewModel
                {
                    Id = ticket.Seat.Id,
                    Row = ticket.Seat.Row,
                    SeatLetter = ticket.Seat.SeatLetter,
                    Plane = new PlaneViewModel
                    {
                        Id = ticket.Seat.PlaneId,
                        Name = ticket.Seat.Plane.Name,
                    }
                },
                PersonType = ticket.PersonType,
                User = new UserDto
                {
                    Id = ticket.User.Id,
                    Username = ticket.User.UserName,
                    Email = ticket.User.Email,
                    PhoneNumber = ticket.User.PhoneNumber ?? "No phone provided"
                },
                PersonFirstName = ticket.PersonFirstName,
                PersonLastName = ticket.PersonLastName,
                Gender = ticket.Gender,
                Price = ticket.Price
            };

            return response;
        }

        public async Task<DataResponse<TicketPagedViewModel>> GetTicketsPaged(int page, int size)
        {
            DataResponse<TicketPagedViewModel> response = new DataResponse<TicketPagedViewModel>();
            response.Data = new TicketPagedViewModel();

            var tickets = await _ticketRepository.GetPagedAsync(false, page, size);

            if (!tickets.Any())
            {
                return response;
            }

            response.Data.Tickets = tickets
                .Select(ticket => new TicketViewModel()
                {
                    Id = ticket.Id,
                    Seat = new SeatViewModel
                    {
                        Id = ticket.Seat.Id,
                        Row = ticket.Seat.Row,
                        SeatLetter = ticket.Seat.SeatLetter,
                        Plane = new PlaneViewModel
                        {
                            Id = ticket.Seat.PlaneId,
                            Name = ticket.Seat.Plane.Name,
                        }
                    },
                    PersonType = ticket.PersonType,
                    User = new UserDto
                    {
                        Id = ticket.User.Id,
                        Username = ticket.User.UserName,
                        PhoneNumber = ticket.User.PhoneNumber ?? "No phone provided",
                        Email = ticket.User.Email
                    },
                    PersonFirstName = ticket.PersonFirstName,
                    PersonLastName = ticket.PersonLastName,
                    Gender = ticket.Gender,
                    Price = ticket.Price,
                });

            response.Data.TotalPages = await _ticketRepository.GetPageCount(size);

            return response;
        }

        public async Task<Response> UpdateTicket(TicketDto ticket, Guid id)
        {
            Response response = new Response();

            Ticket? existingTicket = await _ticketRepository.GetByIdAsync(id, true);

            if (existingTicket == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.TicketNotFound;
                return response;
            }

            Seat seat = await _seatRepository.GetByIdAsync(ticket.SeatId, true);
            User user = await _userRepository.GetByAsync(x => x.Id == ticket.UserId);

            if (seat == null || user == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.InvalidData;
                return response;
            }

            existingTicket.SeatId = ticket.SeatId;
            existingTicket.PersonType = ticket.PersonType;
            existingTicket.UserId = ticket.UserId;
            existingTicket.PersonFirstName = ticket.PersonFirstName;
            existingTicket.PersonLastName = ticket.PersonLastName;
            existingTicket.Gender = ticket.Gender;
            existingTicket.Price = ticket.Price;

            if (!await _ticketRepository.UpdateAsync(existingTicket))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }
    }
}