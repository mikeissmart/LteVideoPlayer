using LteVideoPlayer.Api.CronJobs;
using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LteVideoPlayer.Api.Configs
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddConfigs(this IServiceCollection services, ConfigurationManager configuration)
        {
            foreach(var type in ServicesRegisterHelper.GetTypes(typeof(IConfig)))
            {
                var config = configuration.GetSection(type.Name).Get(type);
                if (config == null)
                    throw new Exception($"Missing config in appsettings: {type}");

                services.AddSingleton(type, config);
            }

            return services;
        }

        public static IServiceScope CheckConfigs(this IServiceScope serviceScope)
        {
            var errors = new List<string>();
            var videoConfigs = serviceScope.ServiceProvider.GetRequiredService<VideoConfigs>();
            foreach (var config in videoConfigs.Configs)
            {
                if (config.FriendlyName.Length == 0)
                    errors.Add($"Config: {config.FriendlyName}, RootDir is required");
                if (config.RootDir.Length == 0)
                    errors.Add($"Config: {config.FriendlyName}, RootDir is required");
                if (config.VideosSubDir.Length == 0)
                    errors.Add($"Config: {config.FriendlyName}, VideosSubDir is required");
                if (config.CanConvertVideo)
                {
                    if (config.ConvertRootFullPath.Length == 0)
                        errors.Add($"Config: {config.FriendlyName}, ConvertRootFullPath is required when CanConvertVideo is true");
                }
                if (config.CanThumbnailVideo)
                {
                    if (config.ThumbnailMinSeekPercent < 0 || config.ThumbnailMinSeekPercent > 100)
                        errors.Add($"Config: {config.FriendlyName}, ThumbnailMinSeekPercent '{config.ThumbnailMinSeekPercent}' must be between 0 and 100");

                    if (config.ThumbnailMaxSeekPercent < 0 || config.ThumbnailMaxSeekPercent > 100)
                        errors.Add($"Config: {config.FriendlyName}, ThumbnailMaxSeekPercent '{config.ThumbnailMaxSeekPercent}' must be between 0 and 100");

                    if (!(config.ThumbnailMinSeekPercent < config.ThumbnailMaxSeekPercent))
                        errors.Add($"Config: {config.FriendlyName}, ThumbnailMinSeekPercent '{config.ThumbnailMinSeekPercent}' must lower than ThumbnailMaxSeekPercent '{config.ThumbnailMaxSeekPercent}'");
                }
            }

            return serviceScope;
        }
    }
}
