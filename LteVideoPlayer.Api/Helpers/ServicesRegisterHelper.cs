using AutoMapper.Internal;
using LteVideoPlayer.Api.CronJobs;
using LteVideoPlayer.Api.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace LteVideoPlayer.Api.Helpers
{
    public static class ServicesRegisterHelper
    {
        private readonly static Type _type = typeof(ServicesRegisterHelper);

        public static List<TypePair> GetTypes(Type iType, Type type)
        {
            var iTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => iType.IsAssignableFrom(x) &&
                    x.IsInterface &&
                    x != iType);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => type.IsAssignableFrom(x) &&
                    !x.IsAbstract &&
                    x.IsClass &&
                    x != type);

            var ret = new List<TypePair>();
            foreach (var t in types)
            {
                var i = iTypes.FirstOrDefault(x => t.IsAssignableTo(x) &&
                    x.IsInterface &&
                    x != t);
                if (i == null)
                    throw new Exception($"Unknown {iType}: {t}");

                ret.Add(new TypePair
                {
                    IType = i,
                    Type = t
                });
            }

            return ret;
        }

        public static List<Type> GetTypes(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => type.IsAssignableFrom(x) &&
                    !x.IsAbstract &&
                    x.IsClass &&
                    x != type)
                .ToList();
        }

        public static void AddServices(IServiceCollection services, Type iType, Type type)
        {
            var method = _type.GetStaticMethod(nameof(Service));
            foreach (var pair in GetTypes(iType, type))
            {
                method.MakeGenericMethod(pair.IType, pair.Type)
                    .Invoke(null, new[] { services });
            }
        }

        public static void AddRepositories(IServiceCollection services, Type iType, Type type)
        {
            var method = _type.GetStaticMethod(nameof(Repository));
            foreach (var pair in GetTypes(iType, type))
            {
                method.MakeGenericMethod(pair.IType, pair.Type)
                    .Invoke(null, new[] { services });
            }
        }

        public static void AddHostedServices(IServiceCollection services, Type type)
        {
            var method = _type.GetStaticMethod(nameof(HostedService));
            foreach (var t in GetTypes(type))
            {
                method.MakeGenericMethod(t)
                    .Invoke(null, new[] { services });
            }
        }

        public static void AddSingletons(IServiceCollection services, Type type)
        {
            var method = _type.GetStaticMethod(nameof(Singleton));
            foreach (var t in GetTypes(type))
            {
                method.MakeGenericMethod(t)
                    .Invoke(null, new[] { services });
            }
        }

        private static void Service<TIService, TService>(IServiceCollection services)
            where TIService : class
            where TService : class, TIService
            => services.AddTransient<TIService, TService>();

        private static void HostedService<T>(IServiceCollection services) where T : class, IHostedService
        {
            services.AddSingleton<T>();
            services.AddHostedService(x => x.GetRequiredService<T>());
        }

        private static void Singleton<T>(IServiceCollection services) where T : class
            => services.AddSingleton<T>();

        private static void Repository<TIRepository, TRepository>(IServiceCollection services)
            where TIRepository : class
            where TRepository : class, TIRepository
            => services.AddTransient<TIRepository, TRepository>();

        public class TypePair
        {
            public required Type IType { get; set; }
            public required Type Type { get; set; }
        }
    }
}