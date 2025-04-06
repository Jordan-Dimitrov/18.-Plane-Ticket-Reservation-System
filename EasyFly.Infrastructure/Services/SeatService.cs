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
        private readonly ISeatRepository _SeatRepository;
        private readonly IPlaneRepository _PlaneRepository;

        public SeatService(ISeatRepository seatRepository, IPlaneRepository planeRepository)
        {
            _SeatRepository = seatRepository;
            _PlaneRepository = planeRepository;
        }

        public async Task<Response> CreateSeat(SeatDto seat)
        {
            Response response = new Response();

            if (await _SeatRepository.ExistsAsync(x => x.Row == seat.Row && x.SeatLetter == seat.SeatLetter && x.PlaneId == seat.PlaneId))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.SeatExists;
                return response;
            }

            Plane plane = await _PlaneRepository.GetByIdAsync(seat.PlaneId, false);

            if (plane == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.InvalidData;
                return response;
            }

            Seat newSeat = new Seat()
            {
                Row = seat.Row,
                SeatLetter = seat.SeatLetter,
                PlaneId = seat.PlaneId
            };

            if (!await _SeatRepository.InsertAsync(newSeat))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<Response> DeleteSeat(Guid id)
        {
            Response response = new Response();

            Seat? seat = await _SeatRepository.GetByIdAsync(id, true);

            if (seat == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.SeatNotFound;
                return response;
            }

            if (!await _SeatRepository.DeleteAsync(seat))
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

            Seat? seat = await _SeatRepository.GetByIdAsync(id, false);

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
                Plane = new PlaneViewModel
                {
                    Id = seat.PlaneId,
                    Name = seat.Plane.Name,
                    Seats = seat.Plane.Seats.Count
                }
            };

            return response;
        }

        public async Task<DataResponse<SeatPagedViewModel>> GetSeatsPaged(int page, int size)
        {
            DataResponse<SeatPagedViewModel> response = new DataResponse<SeatPagedViewModel>();
            response.Data = new SeatPagedViewModel();

            var seats = await _SeatRepository.GetPagedAsync(false, page, size);

            if (!seats.Any())
            {
                return response;
            }

            response.Data.Seats = seats
                .Select(seat => new SeatViewModel()
                {
                    Id = seat.Id,
                    Row = seat.Row,
                    SeatLetter = seat.SeatLetter,
                    Plane = new PlaneViewModel
                    {
                        Id = seat.PlaneId,
                        Name = seat.Plane.Name,
                        Seats = seat.Plane.Seats.Count
                    }
                });

            response.Data.TotalPages = await _SeatRepository.GetPageCount(size);

            return response;
        }

        public async Task<DataResponse<SeatPagedViewModel>> GetSeatsPagedForFlight(Guid flightId, int page, int size)
        {
            DataResponse<SeatPagedViewModel> response = new DataResponse<SeatPagedViewModel>();
            response.Data = new SeatPagedViewModel();

            var seats = await _SeatRepository.GetPagedForFlightAsync(flightId, false, page, size);

            if (!seats.Any())
            {
                return response;
            }

            response.Data.Seats = seats
                .Select(seat => new SeatViewModel()
                {
                    Id = seat.Id,
                    Row = seat.Row,
                    SeatLetter = seat.SeatLetter,
                    Plane = new PlaneViewModel
                    {
                        Id = seat.PlaneId,
                        Name = seat.Plane.Name,
                        Seats = seat.Plane.Seats.Count
                    }
                });

            response.Data.TotalPages = await _SeatRepository.GetPageCount(size);

            return response;
        }

        public async Task<Response> UpdateSeat(SeatDto seat, Guid id)
        {
            Response response = new Response();

            Seat? existingSeat = await _SeatRepository.GetByIdAsync(id, true);

            if (existingSeat == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.SeatNotFound;
                return response;
            }

            Plane plane = await _PlaneRepository.GetByIdAsync(seat.PlaneId, true);

            if (plane == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.InvalidData;
                return response;
            }

            existingSeat.Row = seat.Row;
            existingSeat.SeatLetter = seat.SeatLetter;
            existingSeat.PlaneId = seat.PlaneId;

            if (!await _SeatRepository.UpdateAsync(existingSeat))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }
    }
}