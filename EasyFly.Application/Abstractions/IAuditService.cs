using EasyFly.Application.Dtos;
using EasyFly.Application.Responses;
using EasyFly.Application.ViewModels;

namespace EasyFly.Application.Abstractions
{
    public interface IAuditService
    {
        Task<Response> CreateAudit(Guid userId, AuditDto auditDto);
        Task<Response> DeleteAudit(Guid id);
        Task<Response> DeleteOldestAudits(DateTime before);
        Task<DataResponse<AuditViewModel>> GetAudit(Guid id);
        Task<DataResponse<AuditPagedViewModel>> GetAuditsPaged(int page, int size);
    }
}
