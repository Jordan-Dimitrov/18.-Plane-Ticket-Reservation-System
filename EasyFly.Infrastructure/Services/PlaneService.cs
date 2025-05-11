using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;

namespace EasyFly.Infrastructure.Services
{
    public class PlaneService : IPlaneService
    {
        private readonly IPlaneRepository _PlaneRepository;
        private readonly ISeatRepository _SeatRepository;

        public PlaneService(IPlaneRepository planeRepository, ISeatRepository seatRepository)
        {
            _PlaneRepository = planeRepository;
            _SeatRepository = seatRepository;
        }

        public async Task<Response> CreatePlane(PlaneDto plane)
        {
            Response response = new Response();

            if (await _PlaneRepository.ExistsAsync(x => x.Name == plane.Name))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.PlaneExists;
                return response;
            }

            Plane newPlane = new Plane()
            {
                Name = plane.Name
            };

            if (!await _PlaneRepository.InsertAsync(newPlane))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }
            
            if (!await _SeatRepository.GenerateSeatsForPlane(plane.AvailableSeats, newPlane.Id))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<Response> DeletePlane(Guid id)
        {
            Response response = new Response();

            Plane? plane = await _PlaneRepository.GetByIdAsync(id, true);

            if (plane == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.PlaneNotFound;
                return response;
            }

            if (!await _PlaneRepository.DeleteAsync(plane))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            response.Success = true;

            return response;
        }

        public async Task<DataResponse<PlaneViewModel>> GetPlane(Guid id)
        {
            DataResponse<PlaneViewModel> response = new DataResponse<PlaneViewModel>();

            Plane? plane = await _PlaneRepository.GetByIdAsync(id, false);

            if (plane == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.PlaneNotFound;
                return response;
            }

            response.Data = new PlaneViewModel()
            {
                Id = plane.Id,
                Name = plane.Name,
                Seats = plane.Seats.Count
            };

            return response;
        }

        public async Task<DataResponse<PlanePagedViewModel>> GetPlanesPaged(int page, int size)
        {
            DataResponse<PlanePagedViewModel> response = new DataResponse<PlanePagedViewModel>();
            response.Data = new PlanePagedViewModel();

            var planes = await _PlaneRepository.GetPagedAsync(false, page, size);

            if (!planes.Any())
            {
                return response;
            }

            response.Data.Planes = planes
                .Select(plane => new PlaneViewModel()
                {
                    Id = plane.Id,
                    Name = plane.Name,
                    Seats = plane.Seats.Count
                });

            response.Data.TotalPages = await _PlaneRepository.GetPageCount(size);

            return response;
        }

        public async Task<Response> UpdatePlane(PlaneDto plane, Guid id)
        {
            Response response = new Response();

            Plane? existingPlane = await _PlaneRepository.GetByIdAsync(id, true);

            if (existingPlane == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.PlaneNotFound;
                return response;
            }

            existingPlane.Name = plane.Name;

            if (!await _PlaneRepository.UpdateAsync(existingPlane))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }
    }
}