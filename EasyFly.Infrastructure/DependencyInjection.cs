using EasyFly.Application.Abstractions;
using EasyFly.Infrastructure.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EasyFly.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string assemblyName = currentAssembly.GetName().Name;

            services.AddScoped<IPlaneService, PlaneService>();
            services.AddTransient<IEmailSender, EmailSender>();

            return services;
        }
    }
}
