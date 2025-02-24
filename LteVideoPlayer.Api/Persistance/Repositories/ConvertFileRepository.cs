using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Enums;

namespace LteVideoPlayer.Api.Persistance.Repositories
{
    public interface IConvertFileRepository : IRepository<ConvertFile>
    {
        Task<List<ConvertFile>> GetAllConvertFilesAsync();
        Task<ConvertFile?> GetConvertFileByIdAsync(int id);
        Task<List<ConvertFile>> GetConvertFilesByOriginalPathAsync(DirectoryEnum dirEnum, string originaPath);
        Task<List<ConvertFile>> GetConvertFilesByOriginalAsync(DirectoryEnum dirEnum, string originalPath, string originalFile);
        Task<List<ConvertFile>> GetDirectoryIncompleteConvertFilessAsync(DirectoryEnum dirEnum, bool tracking);
        Task<List<ConvertFile>> GetAllIncompleteConvertFilessAsync(bool tracking);
    }

    public class ConvertFileRepository : BaseRepository<ConvertFile>, IConvertFileRepository
    {
        public ConvertFileRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<List<ConvertFile>> GetAllConvertFilesAsync()
            => await GetAsync(orderBy: x => x.OrderBy(x => x.CreatedDate));

        public async Task<ConvertFile?> GetConvertFileByIdAsync(int id)
            => await GetFirstOrDefaultAsync(
                predicate: x => x.Id == id);

        public async Task<List<ConvertFile>> GetConvertFilesByOriginalPathAsync(DirectoryEnum dirEnum, string originaPath)
            => await GetAsync(
                predicate: x => x.DirectoryEnum == dirEnum && x.EndedDate != null && x.OriginalFile.Path == originaPath,
                orderBy: x => x.OrderBy(x => x.CreatedDate));

        public async Task<List<ConvertFile>> GetConvertFilesByOriginalAsync(DirectoryEnum dirEnum, string originalPath, string originalFile)
            => await GetAsync(
                predicate: x => x.DirectoryEnum == dirEnum && x.OriginalFile.Path == originalPath && x.OriginalFile.File == originalFile,
                orderBy: x=> x.OrderBy(x => x.CreatedDate));

        public async Task<List<ConvertFile>> GetDirectoryIncompleteConvertFilessAsync(DirectoryEnum dirEnum, bool tracking)
            => await GetAsync(
                predicate: x => x.DirectoryEnum == dirEnum && x.EndedDate == null,
                orderBy: x => x.OrderBy(x => x.CreatedDate),
                tracking: tracking);

        public async Task<List<ConvertFile>> GetAllIncompleteConvertFilessAsync(bool tracking)
            => await GetAsync(
                predicate: x => x.EndedDate == null,
                orderBy: x => x.OrderBy(x => x.CreatedDate),
                tracking: tracking);
    }
}
