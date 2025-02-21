using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LteVideoPlayer.Api.CronJobs
{
    public static class CronJobsExtensions
    {
        public static IServiceCollection AddCronJobServices(this IServiceCollection services)
        {
            ServicesRegisterHelper.AddHostedServices(services,
                typeof(BaseCronJobService));

            ServicesRegisterHelper.AddSingletons(services,
                typeof(BaseCronQueueService));

            return services;
        }

        public static IServiceScope StartCronJobCronQueues(this IServiceScope serviceScope)
        {
            foreach (var type in ServicesRegisterHelper.GetTypes(typeof(BaseCronQueueService)))
            {
                ((BaseCronQueueService?)serviceScope
                    .ServiceProvider
                    .GetService(type))!
                    .AutoStartQueue();
            }

            return serviceScope;
        }
    }
}
