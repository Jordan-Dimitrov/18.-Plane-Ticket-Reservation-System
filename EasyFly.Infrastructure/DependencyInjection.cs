using EasyFly.Application.Abstractions;
using EasyFly.Application.Configurations;
using EasyFly.Infrastructure.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.Reflection;

namespace EasyFly.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string assemblyName = currentAssembly.GetName().Name;

            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            services.AddScoped<IPlaneService, PlaneService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddTransient<IEmailSender, EmailSender>();

            return services;
        }
    }
}
