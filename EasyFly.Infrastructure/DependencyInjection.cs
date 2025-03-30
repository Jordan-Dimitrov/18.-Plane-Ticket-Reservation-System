using EasyFly.Application.Abstractions;
using EasyFly.Application.Configurations;
using EasyFly.Infrastructure.BackgroundJobs;
using EasyFly.Infrastructure.BackgtoundJobs;
using EasyFly.Infrastructure.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using QuartzHostedService;
using Stripe;
using System.Reflection;

namespace EasyFly.Infrastructure
{
    public static class DependencyInjection
    {
        private const int _Interval = 1;
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string assemblyName = currentAssembly.GetName().Name;

            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            services.AddScoped<IPlaneService, PlaneService>();
            services.AddScoped<IAirportService, AirportService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<ISeatService, SeatService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IQrCodeService, QrCodeService>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddHostedService<RemoveOldAuditsBackgroundService>();
            services.AddHostedService<RemoveUnreservedTicketsBackgroundService>();

            return services;
        }
    }
}
