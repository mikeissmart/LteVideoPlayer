using LteVideoPlayer.Api.Models;
using LteVideoPlayer.Api.Models.Enums;
using Reinforced.Typings;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;
using System.Reflection;

namespace LteVideoPlayer.Spa.ViewModels
{
	public static class ReinforcedTypingsConfiguration
	{
		public static void Configure(ConfigurationBuilder builder)
		{
            /*
            If getting RT0999 error. Dont use anything from Microsoft.Extensions.Logging.Abstractions, Version=9.0.0.0
            Example: AppLog.LogLevel from Microsoft.Extensions.Logging.
            Create your own LogLevelEnum and use that
             */

            // OverrideName breaks generic class names
            /*var specialDtos = new List<Type>
            {
                typeof(Paginate),
                typeof(PaginateGeneric<>),
                typeof(PaginateGenericFilter<>)
            };
            builder.ExportAsInterfaces(specialDtos, x => x
                .FlattenHierarchy()
                .WithPublicProperties(p =>
                {
                    if (p.Member.IsReferenceForcedNullable())
                        p.ForceNullable();
                })
                .ExportTo("models.d.ts")
                .DontIncludeToNamespace());*/

            var iRefactorType = typeof(IRefactorType);
			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x => iRefactorType.IsAssignableFrom(x) &&
                    x != iRefactorType &&
                    !x.IsGenericType)
				.ToList();
            builder.ExportAsInterfaces(types, x => x
                .FlattenHierarchy()
                .WithPublicProperties(p =>
                {
                    if (p.Member.IsReferenceForcedNullable())
                        p.ForceNullable();
                })
                .ExportTo("models.d.ts")
                .OverrideName(x.Type.Name.Replace("Dto", ""))
                .DontIncludeToNamespace());

            builder.ExportAsEnums(new Type[]
                {
                    typeof(DirectoryEnum)
                }, x =>
                {
                    x.ExportTo("model.enum.ts")
                        .DontIncludeToNamespace();
                    if (!x.Type.Name.Contains("Enum"))
                        x.OverrideName(x.Type.Name + "Enum");
                });

            // Global type substitutions
            builder.Substitute(typeof(DateTimeOffset), new RtSimpleTypeName("string"));
			builder.Substitute(typeof(Guid), new RtSimpleTypeName("string"));
			builder.Substitute(typeof(DateTime), new RtSimpleTypeName("Date"));
			builder.Substitute(typeof(DateTime?), new RtSimpleTypeName("Date"));

			// Gobal settings
			builder.Global(x =>
            {
                x.CamelCaseForProperties()
                    .UseModules()
                    .AutoOptionalProperties();
                x.UseVisitor<NullablePropertyOverridingVisitor>();
            });
		}
	}
}
