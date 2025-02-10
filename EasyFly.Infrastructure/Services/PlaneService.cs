using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;

namespace EasyFly.Infrastructure.Services
{
    internal class PlaneService : IPlaneService
    {
        private readonly IPlaneRepository _planeRepository;

        public PlaneService(IPlaneRepository planeRepository)
        {
            _planeRepository = planeRepository;
        }

        public async Task<Response> CreatePlane(PlaneDto plane)
        {
            Response response = new Response();

            if (await _planeRepository.ExistsAsync(x => x.Name == plane.Name))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.PlaneExists;
                return response;
            }

            Plane newPlane = new Plane()
            {
                Name = plane.Name
            };

            if (!await _planeRepository.InsertAsync(newPlane))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<Response> DeletePlane(Guid id)
        {
            Response response = new Response();

            Plane? plane = await _planeRepository.GetByIdAsync(id, true);

            if (plane == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.PlaneNotFound;
                return response;
            }

            if (!await _planeRepository.DeleteAsync(plane))
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

            Plane? plane = await _planeRepository.GetByIdAsync(id, false);

            if (plane == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.PlaneNotFound;
                return response;
            }

            response.Data = new PlaneViewModel()
            {
                Id = plane.Id,
                Name = plane.Name
            };

            return response;
        }

        public async Task<DataResponse<PlanePagedViewModel>> GetPlanesPaged(int page, int size)
        {
            DataResponse<PlanePagedViewModel> response = new DataResponse<PlanePagedViewModel>();
            response.Data = new PlanePagedViewModel();

            var planes = await _planeRepository.GetPagedAsync(false, page, size);

            if (!planes.Any())
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.PlaneNotFound;
                return response;
            }

            response.Data.Planes = planes
                .Select(plane => new PlaneViewModel()
                {
                    Id = plane.Id,
                    Name = plane.Name
                });

            response.Data.TotalPages = await _planeRepository.GetPageCount(size);

            return response;
        }

        public async Task<Response> UpdatePlane(PlaneDto plane, Guid id)
        {
            Response response = new Response();

            Plane? existingPlane = await _planeRepository.GetByIdAsync(id, true);

            if (existingPlane == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.PlaneNotFound;
                return response;
            }

            existingPlane.Name = plane.Name;

            if (!await _planeRepository.UpdateAsync(existingPlane))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }
    }
}