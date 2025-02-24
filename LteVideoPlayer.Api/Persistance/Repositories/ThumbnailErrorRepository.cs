using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Enums;

namespace LteVideoPlayer.Api.Persistance.Repositories
{
    public interface IThumbnailErrorRepository : IRepository<ThumbnailError>
    {
        Task<List<ThumbnailError>> GetAllThumbnailErrorsAsync();
        Task<ThumbnailError?> GetThumbnailErrorByPathAndFileAsync(DirectoryEnum dirEnum, string path, string file);
    }

    public class ThumbnailErrorRepository : BaseRepository<ThumbnailError>, IThumbnailErrorRepository
    {
        public ThumbnailErrorRepository(AppDbContext dbContext) : base(dbContext)
        {
            
        }

        public async Task<List<ThumbnailError>> GetAllThumbnailErrorsAsync()
            => await GetAsync(
                orderBy: x => x
                    .OrderBy(x => x.LastError)
                    .ThenBy(x => x.Id));

        public async Task<ThumbnailError?> GetThumbnailErrorByPathAndFileAsync(DirectoryEnum dirEnum, string path, string file)
            => await GetFirstOrDefaultAsync(x => x.DirectoryEnum == dirEnum && x.File.Path == path && x.File.Path == file);
    }
}
