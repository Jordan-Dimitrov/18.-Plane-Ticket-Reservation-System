using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Infrastructure.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository _UserRepository;
        public UserService(IUserRepository userRepository)
        {
            _UserRepository = userRepository;
        }
        public async Task<Response> DeleteUser(Guid id)
        {
            Response response = new Response();

            User? user = await _UserRepository.GetByIdAsync(id, true);

            if (user is null)
            {
                response.Success = false;
                response.ErrorMessage = "No such user";
                return response;
            }

            if (!await _UserRepository.DeleteAsync(user))
            {
                response.Success = false;
                response.ErrorMessage = "Unexpected error";
                return response;
            }

            return response;
        }

        public async Task<DataResponse<UserDto>> GetUser(Guid id)
        {
            DataResponse<UserDto> response = new DataResponse<UserDto>();

            User? user = await _UserRepository.GetByIdAsync(id, true);

            if (user is null)
            {
                response.Success = false;
                response.ErrorMessage = "No such user";
                return response;
            }

            response.Data = new UserDto()
            {
                Username = user.UserName
            };

            return response;
        }

        public async Task<DataResponse<IEnumerable<UserDto>>> GetUsersPaged(int page, int size)
        {
            DataResponse<IEnumerable<UserDto>> response = new DataResponse<IEnumerable<UserDto>>();

            var users = await _UserRepository.GetPagedAsync(false, page, size);

            if (!users.Any())
            {
                response.Success = false;
                response.ErrorMessage = "No such users";
                return response;
            }

            response.Data = users.Select(x => new UserDto()
            {
                Username = x.UserName,
            });

            return response;
        }
    }
}
