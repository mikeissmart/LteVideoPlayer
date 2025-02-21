using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Entities.Logging;
using Microsoft.EntityFrameworkCore;

namespace LteVideoPlayer.Api.Persistance
{
    public class AppDbContext : DbContext
    {
        public DbSet<ConvertFile> ConvertFiles => Set<ConvertFile>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<ThumbnailError> ThumbnailErrors => Set<ThumbnailError>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
    }
}
