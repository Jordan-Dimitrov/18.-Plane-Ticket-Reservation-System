using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System.Net.Sockets;

namespace EasyFly.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFlightRepository _flightRepository;

        public TicketService(ITicketRepository ticketRepository, ISeatRepository seatRepository, IUserRepository userRepository, IFlightRepository flightRepository)
        {
            _ticketRepository = ticketRepository;
            _seatRepository = seatRepository;
            _userRepository = userRepository;
            _flightRepository = flightRepository;
        }

        public async Task<Response> CreateTicket(TicketDto ticket)
        {
            Response response = new Response();

            Seat seat = await _seatRepository.GetByIdAsync(ticket.SeatId, true);
            User user = await _userRepository.GetByAsync(x => x.Id == ticket.UserId);
            Flight flight = await _flightRepository.GetByAsync(x => x.Id == ticket.FlightId);

            if (seat == null || user == null || flight == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.InvalidData;
                return response;
            }

            if (ticket.LuggageSize == Domain.Enums.LuggageSize.Medium)
            {
                ticket.Price = ticket.Price * 0.8m;
            }
            else if (ticket.LuggageSize == Domain.Enums.LuggageSize.Small)
            {
                ticket.Price = ticket.Price * 0.6m;
            }

            Ticket newTicket = new Ticket()
            {
                SeatId = ticket.SeatId,
                PersonType = ticket.PersonType,
                UserId = ticket.UserId,
                PersonFirstName = ticket.PersonFirstName,
                PersonLastName = ticket.PersonLastName,
                Gender = ticket.Gender,
                Price = ticket.Price,
                LuggageSize = ticket.LuggageSize,
                Flight = flight,
                CreatedAt = DateTime.UtcNow
            };

            if (!await _ticketRepository.InsertAsync(newTicket))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<DataResponse<CheckoutDto>> CreateTickets(List<ReserveTicketDto> ticketDtos)
        {
            DataResponse<CheckoutDto> response = new DataResponse<CheckoutDto>();

            List<Ticket> tickets = new List<Ticket>();
            Flight flight = await _flightRepository.GetByAsync(x => x.Id == ticketDtos[0].FlightId);
            User user = await _userRepository.GetByAsync(x => x.Id == ticketDtos[0].UserId);

            var availableSeats = (await _seatRepository.GetFreeSeatsForFlightAsync(flight.Id, true, ticketDtos.Count())).ToList();

            if (user == null || flight == null || availableSeats == null || availableSeats.Count() < ticketDtos.Count())
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.InvalidData;
                return response;
            }
            for (int i = 0; i < ticketDtos.Count(); i++)
            {
                var price = flight.TicketPrice;

                if (ticketDtos[i].LuggageSize == Domain.Enums.LuggageSize.Medium)
                {
                    price = price * 0.8m;
                }
                else if (ticketDtos[i].LuggageSize == Domain.Enums.LuggageSize.Small)
                {
                    price = price * 0.6m;
                }

                Ticket newTicket = new Ticket()
                {
                    PersonType = ticketDtos[i].PersonType,
                    UserId = ticketDtos[i].UserId,
                    PersonFirstName = ticketDtos[i].PersonFirstName,
                    PersonLastName = ticketDtos[i].PersonLastName,
                    Gender = ticketDtos[i].Gender,
                    Seat = availableSeats[i],
                    Price = price,
                    LuggageSize = ticketDtos[i].LuggageSize,
                    Flight = flight,
                    CreatedAt = DateTime.UtcNow,
                    Reserved = false
                };

                tickets.Add(newTicket);
            }

            if (!await _ticketRepository.InsertBulkAsync(tickets))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            response.Data = new CheckoutDto()
            {
                ProductName = $"Ticket for {tickets[0].Flight.ArrivalAirport.Name}",
                ProductDescription = $"Nice",
                Amount = tickets.Sum(x => x.Price),
                Currency = "bgn",
                Tickets = tickets.Select(x => x.Id).ToList()
            };

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
                        Seats = ticket.Seat.Plane.Seats.Count
                    }
                },
                IsReserved = ticket.Reserved,
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
                Price = ticket.Price,
                LuggageSize = ticket.LuggageSize,
                FlightId = ticket.FlightId,
                Flight = new FlightViewModel()
                {
                    FlightNumber = ticket.Flight.FlightNumber,
                    DepartureTime = ticket.Flight.DepartureTime,
                    ArrivalTime = ticket.Flight.ArrivalTime,
                    ArrivalAirport = new AirportViewModel()
                    {
                        Name = ticket.Flight.ArrivalAirport.Name
                    },
                    DepartureAirport = new AirportViewModel()
                    {
                        Name = ticket.Flight.DepartureAirport.Name
                    }
                }
            };

            return response;
        }

        public async Task<DataResponse<int>> GetTicketCount()
        {
            var response = new DataResponse<int>();
            response.Data = await _ticketRepository.Count();
            return response;
        }

        public async Task<DataResponse<TicketPagedViewModel>> GetTicketsPaged(int page, int size,
            string? search, string? typeFilter, string? luggageFilter)
        {
            DataResponse<TicketPagedViewModel> response = new DataResponse<TicketPagedViewModel>();
            response.Data = new TicketPagedViewModel();

            var tickets = await _ticketRepository.GetPagedWithFilterAsync(false, page, size, search, typeFilter, luggageFilter);

            if (!tickets.Any())
            {
                return response;
            }

            var ticketViewModels = new List<TicketViewModel>();

            foreach (var ticket in tickets)
            {
                var ticketViewModel = new TicketViewModel()
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
                            Seats = ticket.Seat.Plane.Seats.Count
                        }
                    },
                    IsReserved = ticket.Reserved,
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
                    LuggageSize = ticket.LuggageSize,
                    FlightId = ticket.FlightId,
                    CreatedAt = ticket.CreatedAt,
                    Flight = new FlightViewModel()
                    {
                        FlightNumber = ticket.Flight.FlightNumber,
                        DepartureTime = ticket.Flight.DepartureTime,
                        ArrivalTime = ticket.Flight.ArrivalTime,
                        ArrivalAirport = new AirportViewModel()
                        {
                            Name = ticket.Flight.ArrivalAirport.Name
                        },
                        DepartureAirport = new AirportViewModel()
                        {
                            Name = ticket.Flight.DepartureAirport.Name
                        }
                    }
                };

                ticketViewModels.Add(ticketViewModel);
            }

            response.Data.Tickets = ticketViewModels;
            response.Data.TotalPages = await _ticketRepository.GetPageCountWithFilter(size, search, typeFilter, luggageFilter);

            return response;
        }

        public async Task<DataResponse<TicketPagedViewModel>> GetTicketsPagedByFlightId(Guid flightId, int page, int size,
            string? search, string? typeFilter, string? luggageFilter)
        {
            DataResponse<TicketPagedViewModel> response = new DataResponse<TicketPagedViewModel>();
            response.Data = new TicketPagedViewModel();

            var tickets = await _ticketRepository.GetPagedByFlightIdAsync(flightId, false, page, size, search, typeFilter, luggageFilter);

            if (!tickets.Any())
            {
                return response;
            }

            var ticketViewModels = new List<TicketViewModel>();

            foreach (var ticket in tickets)
            {
                var ticketViewModel = new TicketViewModel()
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
                            Seats = ticket.Seat.Plane.Seats.Count
                        }
                    },
                    IsReserved = ticket.Reserved,
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
                    LuggageSize = ticket.LuggageSize,
                    FlightId = ticket.FlightId,
                    CreatedAt = ticket.CreatedAt,
                    Flight = new FlightViewModel()
                    {
                        FlightNumber = ticket.Flight.FlightNumber,
                        DepartureTime = ticket.Flight.DepartureTime,
                        ArrivalTime = ticket.Flight.ArrivalTime,
                        ArrivalAirport = new AirportViewModel()
                        {
                            Name = ticket.Flight.ArrivalAirport.Name
                        },
                        DepartureAirport = new AirportViewModel()
                        {
                            Name = ticket.Flight.DepartureAirport.Name
                        }
                    }
                };

                ticketViewModels.Add(ticketViewModel);
            }

            response.Data.Tickets = ticketViewModels;
            response.Data.TotalPages = await _ticketRepository.GetPageCountForFlight(size, flightId, search, typeFilter, luggageFilter);

            return response;
        }

        public async Task<DataResponse<TicketPagedViewModel>> GetTicketsPagedByUserId(string userId, int page, int size, string? search, string? typeFilter, string? luggageFilter)
        {
            DataResponse<TicketPagedViewModel> response = new DataResponse<TicketPagedViewModel>();
            response.Data = new TicketPagedViewModel();

            var tickets = await _ticketRepository.GetPagedByUserIdAsync(userId, false, page, size, search, typeFilter, luggageFilter);

            if (!tickets.Any())
            {
                return response;
            }

            var ticketViewModels = new List<TicketViewModel>();

            foreach (var ticket in tickets)
            {
                var ticketViewModel = new TicketViewModel()
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
                            Seats = ticket.Seat.Plane.Seats.Count
                        }
                    },
                    IsReserved = ticket.Reserved,
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
                    LuggageSize = ticket.LuggageSize,
                    FlightId = ticket.FlightId,
                    CreatedAt = ticket.CreatedAt,
                    Flight = new FlightViewModel()
                    {
                        FlightNumber = ticket.Flight.FlightNumber,
                        DepartureTime = ticket.Flight.DepartureTime,
                        ArrivalTime = ticket.Flight.ArrivalTime,
                        ArrivalAirport = new AirportViewModel()
                        {
                            Name = ticket.Flight.ArrivalAirport.Name
                        },
                        DepartureAirport = new AirportViewModel()
                        {
                            Name = ticket.Flight.DepartureAirport.Name
                        }
                    }
                };

                ticketViewModels.Add(ticketViewModel);
            }

            response.Data.Tickets = ticketViewModels;
            response.Data.TotalPages = await _ticketRepository.GetPageCountForUser(size, userId, search, typeFilter, luggageFilter);

            return response;
        }

        public async Task<Response> RemoveUnreservedTickets()
        {
            Response response = new Response();

            if (!await _ticketRepository.RemoveUnreservedTickets())
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

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
            Flight flight = await _flightRepository.GetByAsync(x => x.Id == ticket.FlightId);

            if (seat == null || user == null || flight == null)
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
            existingTicket.LuggageSize = ticket.LuggageSize;
            existingTicket.Flight = flight;

            if (!await _ticketRepository.UpdateAsync(existingTicket))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<Response> UpdateTicketStatus(List<Guid> tickets)
        {
            Response response = new Response();

            if (!await _ticketRepository.UpdateTicketStatus(tickets))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }
    }
}