using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EasyFly.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string assemblyName = currentAssembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, x => x.MigrationsAssembly(assemblyName));
            });

            return services;
        }
    }
}
