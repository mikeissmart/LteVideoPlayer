using LteVideoPlayer.Api.Persistance;
using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Persistance.Repositories;
using LteVideoPlayer.Api.Services;
using Microsoft.EntityFrameworkCore;
using LteVideoPlayer.Api.Models.Entities;

namespace LteVideoPlayer.Api.Persistance
{
    public static class PersistanceExtensions
    {
        public static IServiceCollection AddPersistance(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")!));
            services.AddDbContext<LogDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")!));

            ServicesRegisterHelper.AddRepositories(
                services,
                typeof(IRepositorySetup),
                typeof(IBaseRepositorySetup));

            return services;
        }

        public static IServiceScope StartPersistance(this IServiceScope serviceScope)
        {
            serviceScope.ServiceProvider
                .GetRequiredService<AppDbContext>()
                .Database
                .Migrate();
            serviceScope.ServiceProvider
                .GetRequiredService<LogDbContext>()
                .Database
                .Migrate();

            return serviceScope;
        }
    }
}
