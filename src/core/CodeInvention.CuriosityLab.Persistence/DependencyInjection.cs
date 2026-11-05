using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeInvention.CuriosityLab.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultConnectionString = configuration.GetConnectionString("Default");
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(defaultConnectionString));
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddScoped<IApplicationDbContextInitializer, ApplicationDbContextInitializer>();
            return services;
        }
    }
}
