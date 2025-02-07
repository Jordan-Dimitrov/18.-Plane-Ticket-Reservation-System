using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.Abstractions
{
    public interface IAuditService
    {
        Task<Response> CreateAudit(string userId, AuditDto auditDto);
        Task<Response> DeleteAudit(Guid id);
        Task<Response> DeleteOldestAudits(DateTime before);
        Task<DataResponse<AuditViewModel>> GetAudit(Guid id);
        Task<DataResponse<AuditPagedViewModel>> GetAuditsPaged(int page, int size);
    }
}
