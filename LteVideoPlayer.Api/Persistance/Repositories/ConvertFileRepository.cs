using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Enums;

namespace LteVideoPlayer.Api.Persistance.Repositories
{
    public interface IConvertFileRepository : IRepository<ConvertFile>
    {
        Task<List<ConvertFile>> GetAllConvertFilesAsync(DirectoryEnum dirEnum);
        Task<ConvertFile?> GetConvertFileByIdAsync(int id);
        Task<List<ConvertFile>> GetAllConvertFilesAsync(DirectoryEnum dirEnum, string originaPath);
        Task<ConvertFile?> GetConvertFileByOriginalAsync(DirectoryEnum dirEnum, string originalPath, string originalFile);
        Task<List<ConvertFile>> GetAllIncompleteConvertFilessAsync(DirectoryEnum dirEnum, bool tracking);
    }

    public class ConvertFileRepository : BaseRepository<ConvertFile>, IConvertFileRepository
    {
        public ConvertFileRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<List<ConvertFile>> GetAllConvertFilesAsync(DirectoryEnum dirEnum)
            => await GetAsync(x => x.DirectoryEnum == dirEnum);

        public async Task<ConvertFile?> GetConvertFileByIdAsync(int id)
            => await GetFirstOrDefaultAsync(
                predicate: x => x.Id == id);

        public async Task<List<ConvertFile>> GetAllConvertFilesAsync(DirectoryEnum dirEnum, string originaPath)
            => await GetAsync(
                predicate: x => x.DirectoryEnum == dirEnum && x.EndedDate != null && x.OriginalFile.Path == originaPath,
                orderBy: x => x.OrderBy(x => x.CreatedDate));

        public async Task<ConvertFile?> GetConvertFileByOriginalAsync(DirectoryEnum dirEnum, string originalPath, string originalFile)
            => await GetFirstOrDefaultAsync(
                predicate: x => x.DirectoryEnum == dirEnum && x.OriginalFile.Path == originalPath && x.OriginalFile.File == originalFile);

        public async Task<List<ConvertFile>> GetAllIncompleteConvertFilessAsync(DirectoryEnum dirEnum, bool tracking)
            => await GetAsync(
                predicate: x => x.DirectoryEnum == dirEnum && x.EndedDate == null,
                orderBy: x => x
                    .OrderBy(x => x.OriginalFile.Path)
                        .ThenBy(x => x.OriginalFile.File),
                tracking: tracking);
    }
}
