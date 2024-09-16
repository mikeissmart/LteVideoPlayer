using LteVideoPlayer.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LteVideoPlayer.Api.Persistance
{
    public class AppDbContext : DbContext
    {
        public DbSet<ConvertFile> ConvertFiles => Set<ConvertFile>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<FileHistory> FileHistories => Set<FileHistory>();
        public DbSet<ThumbnailError> ThumbnailErrors => Set<ThumbnailError>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
    }
}
