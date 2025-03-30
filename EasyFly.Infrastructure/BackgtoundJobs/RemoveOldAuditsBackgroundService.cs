using EasyFly.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyFly.Infrastructure.BackgroundJobs
{
    public class RemoveOldAuditsBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RemoveOldAuditsBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

        public RemoveOldAuditsBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<RemoveOldAuditsBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_interval);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                        await auditService.DeleteOldestAudits(DateTime.UtcNow.AddMinutes(-1));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while deleting old audits.");
                }

                await timer.WaitForNextTickAsync(stoppingToken);
            }
        }
    }
}
