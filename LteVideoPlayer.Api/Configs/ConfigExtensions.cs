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
            var videoConfigs = ServicesRegisterHelper.GetTypes(typeof(IVideoConfig))
                .Select(x => (IVideoConfig)serviceScope.ServiceProvider.GetService(x)!)
                .ToList();
            foreach (var config in videoConfigs)
            {
                var name = config.GetType().Name;
                if (config.RootDir.Length == 0)
                    errors.Add($"Config: {name}, RootDir is required");
                if (config.VideosSubDir.Length == 0)
                    errors.Add($"Config: {name}, VideosSubDir is required");
                if (config.CanConvertVideo)
                {
                    if (string.IsNullOrEmpty(config.ConvertToConfigName))
                        errors.Add($"Config: {name}, ConvertToConfigName is required when CanConvertVideo is true");

                    var converToConfig = videoConfigs.FirstOrDefault(x => x.GetType().Name == config.ConvertToConfigName);
                    if (converToConfig == null)
                        errors.Add($"Config: {name}, Could not find VideoConfig '{config.ConvertToConfigName}'");
                    else
                        config.ConvertToConfig = converToConfig;
                }
                if (config.CanThumbnailVideo)
                {
                    if (config.ThumbnailMinSeekPercent == null)
                        errors.Add($"Config: {name}, ThumbnailMinSeekPercent is required when CanThumbnailVideo is true");
                    else if (config.ThumbnailMinSeekPercent < 0 || config.ThumbnailMinSeekPercent > 100)
                        errors.Add($"Config: {name}, ThumbnailMinSeekPercent '{config.ThumbnailMinSeekPercent}' must be between 0 and 100");

                    if (config.ThumbnailMaxSeekPercent == null)
                        errors.Add($"Config: {name}, ThumbnailMaxSeekPercent is required when CanThumbnailVideo is true");
                    else if (config.ThumbnailMaxSeekPercent < 0 || config.ThumbnailMaxSeekPercent > 100)
                        errors.Add($"Config: {name}, ThumbnailMaxSeekPercent '{config.ThumbnailMaxSeekPercent}' must be between 0 and 100");

                    if (config.ThumbnailMinSeekPercent != null && config.ThumbnailMaxSeekPercent != null
                        && config.ThumbnailMinSeekPercent > config.ThumbnailMaxSeekPercent)
                        errors.Add($"Config: {name}, ThumbnailMinSeekPercent '{config.ThumbnailMinSeekPercent}' must lower than ThumbnailMaxSeekPercent '{config.ThumbnailMaxSeekPercent}'");
                }
            }

            return serviceScope;
        }
    }
}
