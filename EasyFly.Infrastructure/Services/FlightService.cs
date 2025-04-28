using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System.Net.Sockets;
using System.Numerics;
using Plane = EasyFly.Domain.Models.Plane;

namespace EasyFly.Infrastructure.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IPlaneRepository _planeRepository;
        private readonly ISeatRepository _seatRepository;

        public FlightService(IFlightRepository flightRepository,
            IAirportRepository airportRepository,
            IPlaneRepository planeRepository,
            ISeatRepository seatRepository)
        {
            _flightRepository = flightRepository;
            _airportRepository = airportRepository;
            _planeRepository = planeRepository;
            _seatRepository = seatRepository;
        }

        public async Task<Response> CreateFlight(FlightDto flight)
        {
            Response response = new Response();

            if (await _flightRepository.ExistsAsync(x => x.FlightNumber == flight.FlightNumber))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.FlightExists;
                return response;
            }

            Airport departureAirport = await _airportRepository
                .GetByIdAsync(flight.DepartureAirportId, true);

            Airport arrivalAirport = await _airportRepository
                .GetByIdAsync(flight.ArrivalAirportId, true);

            Plane plane = await _planeRepository
                .GetByIdAsync(flight.PlaneId, true);

            if (departureAirport == null || arrivalAirport == null || plane == null 
                || flight.ArrivalTime <= flight.DepartureTime || flight.DepartureAirportId == flight.ArrivalAirportId)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.InvalidData;
                return response;
            }

            Flight newFlight = new Flight()
            {
                FlightNumber = flight.FlightNumber,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                DepartureAirportId = flight.DepartureAirportId,
                ArrivalAirportId = flight.ArrivalAirportId,
                PlaneId = flight.PlaneId,
                TicketPrice = flight.TicketPrice,
            };

            if (!await _flightRepository.InsertAsync(newFlight))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<Response> DeleteFlight(Guid id)
        {
            Response response = new Response();

            Flight? flight = await _flightRepository.GetByIdAsync(id, true);

            if (flight == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.FlightNotFound;
                return response;
            }

            if (!await _flightRepository.DeleteAsync(flight))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            response.Success = true;

            return response;
        }

        public async Task<DataResponse<FlightPagedViewModel>> GeFlightstPagedByArrivalAndDepartureAirportsAsync(Guid departureId, Guid arrivalId, int requiredSeats, int page, int size)
        {
            DataResponse<FlightPagedViewModel> response = new DataResponse<FlightPagedViewModel>();
            response.Data = new FlightPagedViewModel();

            var flights = await _flightRepository.GetPagedByArrivalAndDepartureAirportsAsync(departureId, arrivalId, false, requiredSeats, page, size);

            if (!flights.Any())
            {
                return response;
            }

            var returningFlights = await _flightRepository.GetPagedByArrivalAndDepartureWithoutConcreteDateAsync(
                   arrivalId, departureId, DateTime.UtcNow.AddDays(1), false, requiredSeats, page, size);


            var flightViewModels = await Task.WhenAll(flights.Select(async flight =>
            {
                var returningFlightViewModels = returningFlights.Select(rf => new FlightViewModel
                {
                    Id = rf.Id,
                    FlightNumber = rf.FlightNumber,
                    DepartureTime = rf.DepartureTime,
                    ArrivalTime = rf.ArrivalTime,
                    DepartureAirportId = rf.DepartureAirportId,
                    TicketPrice = rf.TicketPrice,
                    DepartureAirport = new AirportViewModel
                    {
                        Id = rf.DepartureAirport.Id,
                        Name = rf.DepartureAirport.Name,
                        Latitude = rf.DepartureAirport.Latitude,
                        Longtitude = rf.DepartureAirport.Longtitude
                    },
                    ArrivalAirportId = rf.ArrivalAirportId,
                    ArrivalAirport = new AirportViewModel
                    {
                        Id = rf.ArrivalAirport.Id,
                        Name = rf.ArrivalAirport.Name,
                        Latitude = rf.ArrivalAirport.Latitude,
                        Longtitude = rf.ArrivalAirport.Longtitude
                    },
                    PlaneId = rf.PlaneId,
                    Plane = new PlaneViewModel
                    {
                        Id = rf.Plane.Id,
                        Name = rf.Plane.Name,
                        Seats = rf.Plane.Seats.Count
                    },
                    ReturningFlights = null
                });

                return new FlightViewModel
                {
                    Id = flight.Id,
                    FlightNumber = flight.FlightNumber,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    DepartureAirportId = flight.DepartureAirportId,
                    TicketPrice = flight.TicketPrice,
                    DepartureAirport = new AirportViewModel
                    {
                        Id = flight.DepartureAirport.Id,
                        Name = flight.DepartureAirport.Name,
                        Latitude = flight.DepartureAirport.Latitude,
                        Longtitude = flight.DepartureAirport.Longtitude
                    },
                    ArrivalAirportId = flight.ArrivalAirportId,
                    ArrivalAirport = new AirportViewModel
                    {
                        Id = flight.ArrivalAirport.Id,
                        Name = flight.ArrivalAirport.Name,
                        Latitude = flight.ArrivalAirport.Latitude,
                        Longtitude = flight.ArrivalAirport.Longtitude
                    },
                    PlaneId = flight.PlaneId,
                    Plane = new PlaneViewModel
                    {
                        Id = flight.Plane.Id,
                        Name = flight.Plane.Name,
                        Seats = flight.Plane.Seats.Count
                    },
                    ReturningFlights = returningFlightViewModels.Where(x => x.DepartureTime > flight.ArrivalTime)
                };
            }));

            response.Data.Flights = flightViewModels;
            response.Data.TotalPages = await _flightRepository.GetPageCount(size);

            return response;
        }

        public async Task<DataResponse<FlightViewModel>> GetFlight(Guid id)
        {
            DataResponse<FlightViewModel> response = new DataResponse<FlightViewModel>();

            Flight? flight = await _flightRepository.GetByIdAsync(id, false);

            if (flight == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.FlightNotFound;
                return response;
            }

            var seatResponse = await _seatRepository.GetFreeSeatsForFlightAsync(id, false, int.MaxValue);

            response.Data = new FlightViewModel()
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                DepartureAirportId = flight.DepartureAirportId,
                TicketPrice = flight.TicketPrice,
                DepartureAirport = new AirportViewModel
                {
                    Id = flight.DepartureAirport.Id,
                    Name = flight.DepartureAirport.Name,
                    Latitude = flight.DepartureAirport.Latitude,
                    Longtitude = flight.DepartureAirport.Longtitude
                },
                ArrivalAirportId = flight.ArrivalAirportId,
                ArrivalAirport = new AirportViewModel
                {
                    Id = flight.ArrivalAirport.Id,
                    Name = flight.ArrivalAirport.Name,
                    Latitude = flight.ArrivalAirport.Latitude,
                    Longtitude = flight.ArrivalAirport.Longtitude
                },
                PlaneId = flight.PlaneId,
                Plane = new PlaneViewModel
                {
                    Id = flight.Plane.Id,
                    Name = flight.Plane.Name,
                    Seats = flight.Plane.Seats.Count
                },
                FreeSeatCount = seatResponse.Count()
            };

            return response;
        }

        public async Task<DataResponse<FlightPagedViewModel>> GetFlightsPaged(int page, int size)
        {
            DataResponse<FlightPagedViewModel> response = new DataResponse<FlightPagedViewModel>();
            response.Data = new FlightPagedViewModel();

            var flights = await _flightRepository.GetPagedAsync(false, page, size);
            if (!flights.Any())
            {
                return response;
            }

            var flightViewModels = new List<FlightViewModel>();
            foreach (var flight in flights)
            {
                var freeSeats = await _seatRepository.GetFreeSeatsForFlightAsync(flight.Id, false, int.MaxValue);
                flightViewModels.Add(new FlightViewModel()
                {
                    Id = flight.Id,
                    FlightNumber = flight.FlightNumber,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    DepartureAirportId = flight.DepartureAirportId,
                    DepartureAirport = new AirportViewModel
                    {
                        Id = flight.DepartureAirport.Id,
                        Name = flight.DepartureAirport.Name,
                        Latitude = flight.DepartureAirport.Latitude,
                        Longtitude = flight.DepartureAirport.Longtitude
                    },
                    ArrivalAirportId = flight.ArrivalAirportId,
                    ArrivalAirport = new AirportViewModel
                    {
                        Id = flight.ArrivalAirport.Id,
                        Name = flight.ArrivalAirport.Name,
                        Latitude = flight.ArrivalAirport.Latitude,
                        Longtitude = flight.ArrivalAirport.Longtitude
                    },
                    PlaneId = flight.PlaneId,
                    Plane = new PlaneViewModel
                    {
                        Id = flight.Plane.Id,
                        Name = flight.Plane.Name,
                        Seats = flight.Plane.Seats.Count
                    },
                    FreeSeatCount = freeSeats.Count()
                });
            }

            response.Data.Flights = flightViewModels;
            response.Data.TotalPages = await _flightRepository.GetPageCount(size);
            return response;
        }


        public async Task<DataResponse<FlightPagedViewModel>> GetFlightsPagedByArrivalAndDepartureAsync(
            Guid departureId, Guid arrivalId, DateTime departure, int requiredSeats, int page, int size)
        {
            var response = new DataResponse<FlightPagedViewModel>
            {
                Data = new FlightPagedViewModel()
            };

            var flights = await _flightRepository.GetPagedByArrivalAndDepartureAsync(
                departureId, arrivalId, departure, false, requiredSeats, page, size);

            if (!flights.Any())
            {
                return response;
            }

            var returningFlights = await _flightRepository.GetPagedByArrivalAndDepartureWithoutConcreteDateAsync(
                   arrivalId, departureId, departure.AddDays(1), false, requiredSeats, page, size);

            var flightViewModels = await Task.WhenAll(flights.Select(async flight =>
            {
                var returningFlightViewModels = returningFlights.Select(rf => new FlightViewModel
                {
                    Id = rf.Id,
                    FlightNumber = rf.FlightNumber,
                    DepartureTime = rf.DepartureTime,
                    ArrivalTime = rf.ArrivalTime,
                    DepartureAirportId = rf.DepartureAirportId,
                    TicketPrice = rf.TicketPrice,
                    DepartureAirport = new AirportViewModel
                    {
                        Id = rf.DepartureAirport.Id,
                        Name = rf.DepartureAirport.Name,
                        Latitude = rf.DepartureAirport.Latitude,
                        Longtitude = rf.DepartureAirport.Longtitude
                    },
                    ArrivalAirportId = rf.ArrivalAirportId,
                    ArrivalAirport = new AirportViewModel
                    {
                        Id = rf.ArrivalAirport.Id,
                        Name = rf.ArrivalAirport.Name,
                        Latitude = rf.ArrivalAirport.Latitude,
                        Longtitude = rf.ArrivalAirport.Longtitude
                    },
                    PlaneId = rf.PlaneId,
                    Plane = new PlaneViewModel
                    {
                        Id = rf.Plane.Id,
                        Name = rf.Plane.Name,
                        Seats = flight.Plane.Seats.Count
                    },
                    ReturningFlights = null
                });

                return new FlightViewModel
                {
                    Id = flight.Id,
                    FlightNumber = flight.FlightNumber,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    DepartureAirportId = flight.DepartureAirportId,
                    TicketPrice = flight.TicketPrice,
                    DepartureAirport = new AirportViewModel
                    {
                        Id = flight.DepartureAirport.Id,
                        Name = flight.DepartureAirport.Name,
                        Latitude = flight.DepartureAirport.Latitude,
                        Longtitude = flight.DepartureAirport.Longtitude
                    },
                    ArrivalAirportId = flight.ArrivalAirportId,
                    ArrivalAirport = new AirportViewModel
                    {
                        Id = flight.ArrivalAirport.Id,
                        Name = flight.ArrivalAirport.Name,
                        Latitude = flight.ArrivalAirport.Latitude,
                        Longtitude = flight.ArrivalAirport.Longtitude
                    },
                    PlaneId = flight.PlaneId,
                    Plane = new PlaneViewModel
                    {
                        Id = flight.Plane.Id,
                        Name = flight.Plane.Name,
                        Seats = flight.Plane.Seats.Count
                    },
                    ReturningFlights = returningFlightViewModels.Where(x => x.DepartureTime > flight.ArrivalTime)
                };
            }));

            response.Data.Flights = flightViewModels;
            response.Data.TotalPages = await _flightRepository.GetPageCount(size);

            return response;
        }


        public async Task<DataResponse<FlightPagedViewModel>> GetFlightsPagedByPlane(Guid planeId, int page, int size)
        {
            DataResponse<FlightPagedViewModel> response = new DataResponse<FlightPagedViewModel>();
            response.Data = new FlightPagedViewModel();

            var flights = await _flightRepository.GetPagedByPlaneIdAsync(planeId, false, page, size);
            if (!flights.Any())
            {
                return response;
            }

            var flightViewModels = new List<FlightViewModel>();
            foreach (var flight in flights)
            {
                var freeSeats = await _seatRepository.GetFreeSeatsForFlightAsync(flight.Id, false, int.MaxValue);
                flightViewModels.Add(new FlightViewModel()
                {
                    Id = flight.Id,
                    FlightNumber = flight.FlightNumber,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    DepartureAirportId = flight.DepartureAirportId,
                    TicketPrice = flight.TicketPrice,
                    DepartureAirport = new AirportViewModel
                    {
                        Id = flight.DepartureAirport.Id,
                        Name = flight.DepartureAirport.Name,
                        Latitude = flight.DepartureAirport.Latitude,
                        Longtitude = flight.DepartureAirport.Longtitude
                    },
                    ArrivalAirportId = flight.ArrivalAirportId,
                    ArrivalAirport = new AirportViewModel
                    {
                        Id = flight.ArrivalAirport.Id,
                        Name = flight.ArrivalAirport.Name,
                        Latitude = flight.ArrivalAirport.Latitude,
                        Longtitude = flight.ArrivalAirport.Longtitude
                    },
                    PlaneId = flight.PlaneId,
                    Plane = new PlaneViewModel
                    {
                        Id = flight.Plane.Id,
                        Name = flight.Plane.Name,
                        Seats = flight.Plane.Seats.Count
                    },
                    FreeSeatCount = freeSeats.Count()
                });
            }

            response.Data.Flights = flightViewModels;
            response.Data.TotalPages = await _flightRepository.GetPageCount(size);
            return response;
        }


        public async Task<Response> UpdateFlight(FlightDto flight, Guid id)
        {
            Response response = new Response();

            Flight? existingFlight = await _flightRepository.GetByIdAsync(id, true);

            if (existingFlight == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.FlightNotFound;
                return response;
            }

            Airport departureAirport = await _airportRepository
                .GetByIdAsync(flight.DepartureAirportId, true);

            Airport arrivalAirport = await _airportRepository
                .GetByIdAsync(flight.ArrivalAirportId, true);

            Plane plane = await _planeRepository
                .GetByIdAsync(existingFlight.PlaneId, true);

            if (departureAirport == null || arrivalAirport == null || plane == null
                || flight.ArrivalTime <= flight.DepartureTime || flight.DepartureAirportId == flight.ArrivalAirportId)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.InvalidData;
                return response;
            }

            existingFlight.FlightNumber = flight.FlightNumber;
            existingFlight.DepartureTime = flight.DepartureTime;
            existingFlight.ArrivalTime = flight.ArrivalTime;
            existingFlight.DepartureAirportId = flight.DepartureAirportId;
            existingFlight.ArrivalAirportId = flight.ArrivalAirportId;
            existingFlight.PlaneId = existingFlight.PlaneId;

            if (!await _flightRepository.UpdateAsync(existingFlight))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }
    }
}