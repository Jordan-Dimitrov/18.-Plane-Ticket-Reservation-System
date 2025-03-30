using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        public async Task<DataResponse<int>> GetUserCount()
        {
            var response = new DataResponse<int>();
            response.Data = await _UserRepository.Count();
            return response;
        }

        public async Task<DataResponse<UserPagedViewModel>> GetUsersPaged(int page, int size)
        {
            DataResponse<UserPagedViewModel> response = new DataResponse<UserPagedViewModel>();
            response.Data = new UserPagedViewModel();

            var users = await _UserRepository.GetPagedAsync(false, page, size);

            if (!users.Any())
            {
                return response;
            }

            response.Data.Users = users.Select(x => new UserDto()
            {
                Id = x.Id,
                Username = x.UserName,
                PhoneNumber = x.PhoneNumber ?? "No phone provided",
                Email = x.Email,
            });

            response.Data.TotalPages = await _UserRepository.GetPageCount(page);

            return response;
        }

        public async Task<Response> UpdateUser(UserDto user, Guid id)
        {
            Response response = new Response();

            User? existingUser = await _UserRepository.GetByIdAsync(id, true);

            if (existingUser == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.UserNotFound;
                return response;
            }

            existingUser.UserName = user.Username;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;

            if (!await _UserRepository.UpdateAsync(existingUser))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }
    }
}
