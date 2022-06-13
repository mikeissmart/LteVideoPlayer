using LteVideoPlayer.Api.Dtos;
using Microsoft.Extensions.Logging;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using System.Reflection;

namespace LteVideoPlayer.Spa.ViewModels
{
	public static class ReinforcedTypingsConfiguration
	{
		public static void Configure(ConfigurationBuilder builder)
		{
			var iRefactorType = typeof(IRefactorType);
			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x => x.GetInterfaces().Any(y => y == iRefactorType));

			builder.ExportAsInterfaces(types,
				c => c.WithPublicProperties(p => p.ForceNullable())
						.ExportTo("models.d.ts")
						.DontIncludeToNamespace());

			/*builder.ExportAsEnums(new[]
				{
				}, c => c.ExportTo("models.enums.ts") 
					.DontIncludeToNamespace());*/

			// Global type substitutions
			builder.Substitute(typeof(DateTimeOffset), new RtSimpleTypeName("string"));
			builder.Substitute(typeof(Guid), new RtSimpleTypeName("string"));
			builder.Substitute(typeof(DateTime), new RtSimpleTypeName("Date"));
			builder.Substitute(typeof(DateTime?), new RtSimpleTypeName("Date"));

			// Gobal settings
			builder.Global(x =>
			{
				x.CamelCaseForProperties();
				x.UseModules();
			});
		}
	}
}
