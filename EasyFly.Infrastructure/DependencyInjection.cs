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

            return services;
        }
    }
}
