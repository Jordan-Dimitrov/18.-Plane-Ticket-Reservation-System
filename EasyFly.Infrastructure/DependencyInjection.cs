using EasyFly.Application.Abstractions;
using EasyFly.Infrastructure.Services;
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

            return services;
        }
    }
}
