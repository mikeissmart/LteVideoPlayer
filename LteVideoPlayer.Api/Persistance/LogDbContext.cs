using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Entities.Logging;
using Microsoft.EntityFrameworkCore;

namespace LteVideoPlayer.Api.Persistance
{
    public class LogDbContext : DbContext
    {
        public DbSet<AppLog> AppLogs => Set<AppLog>();
        public DbSet<CronLog> CronLogs => Set<CronLog>();

        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
        }
    }
}
