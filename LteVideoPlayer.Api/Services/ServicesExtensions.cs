using LteVideoPlayer.Api.CronJobs;
using LteVideoPlayer.Api.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace LteVideoPlayer.Api.Services
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            ServicesRegisterHelper.AddServices(services,
                typeof(IService),
                typeof(BaseService));

            services.AddSingleton<ConnectService>();

            return services;
        }

        public static IServiceScope StartConnectServices(this IServiceScope serviceScope)
        {
            serviceScope.ServiceProvider.GetRequiredService<ConnectService>().ConnectAll();

            return serviceScope;
        }
    }
}
