using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.CronJob.Convert;
using LteVideoPlayer.Api.Service;
using System.Reflection;

namespace LteVideoPlayer.Api
{
    public static class ServicesExtentions
	{
		public static IServiceCollection AddConfigs(this IServiceCollection services, ConfigurationManager configuration)
		{
			var iConfigType = typeof(IConfig);
			var types = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(x => x.GetInterfaces().Any(y => y == iConfigType));

			foreach (var type in types)
			{
				var config = configuration.GetSection(type.Name).Get(type);
				if (config != null)
					services.AddSingleton(type, config);
			}

			return services;
		}

		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services
				.AddTransient<IConvertFileService, ConvertFileService>()
				.AddTransient<IDirectoryService, DirectoryService>()
				.AddTransient<IFileHistoryService, FileHistoryService>()
				.AddTransient<IUserProfileService, UserProfileService>();

			return services;
		}

		public static IServiceCollection AddCronJobs(this IServiceCollection services)
        {
			services.AddSingleton<ConvertQueueCronJob>();

			return services;
        }
	}
}
