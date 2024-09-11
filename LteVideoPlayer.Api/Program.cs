using LteVideoPlayer.Api;
using LteVideoPlayer.Api.CronJob.Convert;
using LteVideoPlayer.Api.Hubs;
using LteVideoPlayer.Api.Persistance;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.

services
    .AddConfigs(configuration)
    .AddAutoMapper(x => x.AddProfile(new MappingProfile()))
    //.AddDbContext<AppDbContext>(x => x.UseSqlite(configuration.GetConnectionString("SqliteDefaultConnection")))
    .AddDbContext<AppDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
    .AddServices()
    .AddCronJobs()
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
        options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
    });
services.AddSignalR();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Start ConvertQueueCronJob
    scope.ServiceProvider
        .GetRequiredService<ShareConnect>()
        .Connect();
    // Migrate Database
    scope.ServiceProvider
        .GetRequiredService<AppDbContext>()
        .Database
        .Migrate();

    scope.ServiceProvider
        .GetRequiredService<ConvertQueueCronJob>()
        .StartQueue();
    scope.ServiceProvider
        .GetRequiredService<ThumbnailCronJob>()
        .StartThumbnails();
}

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseCors(x => x
    //.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials());
app.UseAuthorization();

app.MapControllers();

app.MapHub<RemoteHub>("/hub/remotehub");

app.Run();
