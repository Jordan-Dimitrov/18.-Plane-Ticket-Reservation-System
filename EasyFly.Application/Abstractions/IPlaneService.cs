using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;

namespace EasyFly.Application.Abstractions
{
    public interface IPlaneService
    {
        Task<Response> CreatePlane(PlaneDto plane);
        Task<Response> DeletePlane(Guid id);
        Task<Response> UpdatePlane(PlaneDto plane, Guid id);
        Task<DataResponse<PlaneViewModel>> GetPlane(Guid id);
        Task<DataResponse<PlanePagedViewModel>> GetPlanesPaged(int page, int size);
    }
}
