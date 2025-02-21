using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.CronJobs;
using LteVideoPlayer.Api.Logging;
using LteVideoPlayer.Api.Models;
using LteVideoPlayer.Api.Persistance;
using LteVideoPlayer.Api.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;
        var services = builder.Services;

        // Add services to the container.

        builder.AddLogging(configuration);

        services
            .AddConfigs(configuration)
            .AddCronJobServices()
            .AddPersistance(configuration)
            .AddServices()
            .AddAutoMapper(x => x.AddProfile(new MappingProfile()))
            .AddCors()
            .AddSignalR();

        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
            });

        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            scope
                .CheckConfigs()
                .StartPersistance()
                .StartConnectServices()
                .StartCronJobCronQueues();
        }

        // Configure the HTTP request pipeline.

        //app.UseHttpsRedirection();

        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}