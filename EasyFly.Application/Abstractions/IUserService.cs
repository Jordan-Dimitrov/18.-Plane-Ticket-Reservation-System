using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;

namespace EasyFly.Application.Abstractions
{
    public interface IUserService
    {
        Task<Response> DeleteUser(Guid id);
        Task<Response> UpdateUser(UserDto user, Guid id);
        Task<DataResponse<UserDto>> GetUser(Guid id);
        Task<DataResponse<UserPagedViewModel>> GetUsersPaged(int page, int size);
        Task<DataResponse<int>> GetUserCount();
    }
}
