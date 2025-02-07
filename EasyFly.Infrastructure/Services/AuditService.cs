using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Infrastructure.Services
{
    internal class AuditService : IAuditService
    {
        public Task<Response> CreateAudit(string userId, AuditDto auditDto)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteAudit(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteOldestAudits(DateTime before)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<AuditViewModel>> GetAudit(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse<AuditPagedViewModel>> GetAuditsPaged(int page, int size)
        {
            throw new NotImplementedException();
        }
    }
}
