using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFly.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly IUserRepository _userRepository;

        public AuditService(IAuditRepository auditRepository, IUserRepository userRepository)
        {
            _auditRepository = auditRepository;
            _userRepository = userRepository;
        }

        public async Task<Response> CreateAudit(Guid userId, AuditDto auditDto)
        {
            Response response = new Response();

            User? user = await _userRepository.GetByIdAsync(userId, true);

            if (user == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.UserNotFound;
                return response;
            }

            Audit audit = new Audit()
            {
                ModifiedAt = auditDto.ModifiedAt,
                Message = auditDto.Message,
                UserId = user.Id
            };

            if (!await _auditRepository.InsertAsync(audit))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            return response;
        }

        public async Task<Response> DeleteAudit(Guid id)
        {
            Response response = new Response();

            Audit? audit = await _auditRepository.GetByIdAsync(id, true);

            if (audit == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.AuditNotFound;
                return response;
            }

            if (!await _auditRepository.DeleteAsync(audit))
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.Unexpected;
            }

            response.Success = true;

            return response;
        }

        public async Task<Response> DeleteOldestAudits(DateTime before)
        {
            Response response = new Response();

            var audits = await _auditRepository.GetAllByAsync(x => x.ModifiedAt < before);

            foreach (var audit in audits)
            {
                if (!await _auditRepository.DeleteAsync(audit))
                {
                    response.Success = false;
                    response.ErrorMessage = ResponseConstants.Unexpected;
                    return response;
                }
            }

            response.Success = true;

            return response;
        }

        public async Task<DataResponse<AuditViewModel>> GetAudit(Guid id)
        {
            DataResponse<AuditViewModel> response = new DataResponse<AuditViewModel>();

            Audit? audit = await _auditRepository.GetByIdAsync(id, false);

            if (audit == null)
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.AuditNotFound;
                return response;
            }

            User? user = await _userRepository.GetByAsync(x => x.Id.ToString() == audit.UserId);

            response.Data = new AuditViewModel()
            {
                Id = audit.Id,
                ModifiedAt = audit.ModifiedAt,
                Message = audit.Message,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber ?? "No phone provided"
                }
            };

            return response;
        }

        public async Task<DataResponse<AuditPagedViewModel>> GetAuditsPaged(int page, int size)
        {
            DataResponse<AuditPagedViewModel> response = new DataResponse<AuditPagedViewModel>();
            response.Data = new AuditPagedViewModel();

            var audits = await _auditRepository.GetPagedAsync(false, page, size);

/*            if (!audits.Any())
            {
                response.Success = false;
                response.ErrorMessage = ResponseConstants.AuditNotFound;
                return response;
            }*/

            response.Data.AuditViewModels = audits
                .Select(audit => new AuditViewModel()
                {
                    Id = audit.Id,
                    ModifiedAt = audit.ModifiedAt,
                    Message = audit.Message,
                    User = new UserDto
                    {
                        Id = audit.User.Id,
                        Username = audit.User.UserName,
                        Email = audit.User.Email,
                        PhoneNumber = audit.User.PhoneNumber ?? "No phone provided"
                    }
                });

            response.Data.TotalPages = await _auditRepository.GetPageCount(size);

            return response;
        }
    }
}