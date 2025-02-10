using EasyFly.Application.Abstractions;
using Quartz;

namespace EasyFly.Infrastructure.BackgtoundJobs
{
    [DisallowConcurrentExecution]
    public class RemoveOldAuditsBackgroundJob : IJob
    {
        private readonly IAuditService _AuditService;
        public RemoveOldAuditsBackgroundJob(IAuditService auditService)
        {
            _AuditService = auditService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _AuditService.DeleteOldestAudits(DateTime.UtcNow.AddDays(-1));
        }
    }
}
