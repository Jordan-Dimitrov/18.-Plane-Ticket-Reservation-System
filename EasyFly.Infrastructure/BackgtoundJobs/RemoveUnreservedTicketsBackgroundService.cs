using EasyFly.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Infrastructure.BackgtoundJobs
{
    public class RemoveUnreservedTicketsBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RemoveUnreservedTicketsBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);

        public RemoveUnreservedTicketsBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<RemoveUnreservedTicketsBackgroundService> logger)
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
                        var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();
                        await ticketService.RemoveUnreservedTickets();
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
