using LteVideoPlayer.Api.CronJobs;
using LteVideoPlayer.Api.Helpers;

namespace LteVideoPlayer.Api.Logging
{
    public static class LoggingExtensions
    {
        public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder, ConfigurationManager configuration)
        {
            builder.Logging.AddProvider(new DbLoggerProvider(configuration.GetConnectionString("DefaultConnection")!));

            return builder;
        }
    }
}
