using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;

namespace EasyFly.Infrastructure.Services
{
    internal class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IFlightRepository _flightRepository;

        public SeatService(ISeatRepository seatRepository, IFlightRepository flightRepository)
        {
            _seatRepository = seatRepository;
            _flightRepository = flightRepository;
        }

        public async Task<Response> CreateSeat(SeatDto seat)
        {
            Response response = new Response();

            if (await _seatRepository.ExistsAsync(x => x.Row == seat.Row && x.SeatLetter == seat.SeatLetter && x.FlightId == seat.FlightId))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.SeatExists;
                return response;
            }

            Flight flight = await _flightRepository.GetByIdAsync(seat.FlightId, false);

            if (flight == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.InvalidData;
                return response;
            }

            Seat newSeat = new Seat()
            {
                Row = seat.Row,
                SeatLetter = seat.SeatLetter,
                FlightId = seat.FlightId
            };

            if (!await _seatRepository.InsertAsync(newSeat))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<Response> DeleteSeat(Guid id)
        {
            Response response = new Response();

            Seat? seat = await _seatRepository.GetByIdAsync(id, true);

            if (seat == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.SeatNotFound;
                return response;
            }

            if (!await _seatRepository.DeleteAsync(seat))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            response.Success = true;

            return response;
        }

        public async Task<DataResponse<SeatViewModel>> GetSeat(Guid id)
        {
            DataResponse<SeatViewModel> response = new DataResponse<SeatViewModel>();

            Seat? seat = await _seatRepository.GetByIdAsync(id, false);

            if (seat == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.SeatNotFound;
                return response;
            }

            response.Data = new SeatViewModel()
            {
                Id = seat.Id,
                Row = seat.Row,
                SeatLetter = seat.SeatLetter,
                FlightId = seat.FlightId,
                Flight = new FlightViewModel
                {
                    Id = seat.Flight.Id,
                    FlightNumber = seat.Flight.FlightNumber,
                    DepartureTime = seat.Flight.DepartureTime,
                    ArrivalTime = seat.Flight.ArrivalTime,
                    DepartureAirportId = seat.Flight.DepartureAirportId,
                    DepartureAirport = new AirportViewModel
                    {
                        Id = seat.Flight.DepartureAirport.Id,
                        Name = seat.Flight.DepartureAirport.Name,
                        Latitude = seat.Flight.DepartureAirport.Latitude,
                        Longtitude = seat.Flight.DepartureAirport.Longtitude
                    },
                    ArrivalAirportId = seat.Flight.ArrivalAirportId,
                    ArrivalAirport = new AirportViewModel
                    {
                        Id = seat.Flight.ArrivalAirport.Id,
                        Name = seat.Flight.ArrivalAirport.Name,
                        Latitude = seat.Flight.ArrivalAirport.Latitude,
                        Longtitude = seat.Flight.ArrivalAirport.Longtitude
                    },
                    PlaneId = seat.Flight.PlaneId,
                    Plane = new PlaneViewModel
                    {
                        Id = seat.Flight.Plane.Id,
                        Name = seat.Flight.Plane.Name
                    }
                }
            };

            return response;
        }

        public async Task<DataResponse<SeatPagedViewModel>> GetSeatsPaged(int page, int size)
        {
            DataResponse<SeatPagedViewModel> response = new DataResponse<SeatPagedViewModel>();
            response.Data = new SeatPagedViewModel();

            var seats = await _seatRepository.GetPagedAsync(false, page, size);

            if (!seats.Any())
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.SeatNotFound;
                return response;
            }

            response.Data.Seats = seats
                .Select(seat => new SeatViewModel()
                {
                    Id = seat.Id,
                    Row = seat.Row,
                    SeatLetter = seat.SeatLetter,
                    FlightId = seat.FlightId,
                    Flight = new FlightViewModel
                    {
                        Id = seat.Flight.Id,
                        FlightNumber = seat.Flight.FlightNumber,
                        DepartureTime = seat.Flight.DepartureTime,
                        ArrivalTime = seat.Flight.ArrivalTime,
                        DepartureAirportId = seat.Flight.DepartureAirportId,
                        DepartureAirport = new AirportViewModel
                        {
                            Id = seat.Flight.DepartureAirport.Id,
                            Name = seat.Flight.DepartureAirport.Name,
                            Latitude = seat.Flight.DepartureAirport.Latitude,
                            Longtitude = seat.Flight.DepartureAirport.Longtitude
                        },
                        ArrivalAirportId = seat.Flight.ArrivalAirportId,
                        ArrivalAirport = new AirportViewModel
                        {
                            Id = seat.Flight.ArrivalAirport.Id,
                            Name = seat.Flight.ArrivalAirport.Name,
                            Latitude = seat.Flight.ArrivalAirport.Latitude,
                            Longtitude = seat.Flight.ArrivalAirport.Longtitude
                        },
                        PlaneId = seat.Flight.PlaneId,
                        Plane = new PlaneViewModel
                        {
                            Id = seat.Flight.Plane.Id,
                            Name = seat.Flight.Plane.Name
                        }
                    }
                });

            response.Data.TotalPages = await _seatRepository.GetPageCount(size);

            return response;
        }

        public async Task<Response> UpdateSeat(SeatDto seat, Guid id)
        {
            Response response = new Response();

            Seat? existingSeat = await _seatRepository.GetByIdAsync(id, true);

            if (existingSeat == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.SeatNotFound;
                return response;
            }

            Flight flight = await _flightRepository.GetByIdAsync(seat.FlightId, true);

            if (flight == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.InvalidData;
                return response;
            }

            existingSeat.Row = seat.Row;
            existingSeat.SeatLetter = seat.SeatLetter;
            existingSeat.FlightId = seat.FlightId;

            if (!await _seatRepository.UpdateAsync(existingSeat))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }
    }
}