using AutoMapper;
using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.CronJob.Convert;
using LteVideoPlayer.Api.DataTypes;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Entities;
using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Persistance;
using Microsoft.EntityFrameworkCore;

namespace LteVideoPlayer.Api.Service
{
    public interface IThumbnailService
    {
        string GetFolderThumbnail(string filePathName);
        string GetFileThumbnail(string filePathName);
        void DeleteThumbnail(string filePathName);
        Task<MetaDataDto> GetVideoMetaAsync(string filePathName, bool isStaging);
        string GetWorkingThumbnail();
        Task<ThumbnailError> AddOrUpdateThumbnailErrorsAsync(FileDto file, string error);
        Task<List<ThumbnailErrorDto>> GetThumbnailErrorsAsync();
        Task DeleteThumbnailErrorAsync(FileDto file);
    }

    public class ThumbnailService : IThumbnailService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<ThumbnailService> _logger;
        private readonly VideoConfig _videoConfig;
        private readonly ThumbnailCronJob _thumbnailCronJob;
        private readonly IMapper _mapper;

        public ThumbnailService(AppDbContext appDbContext,
            ILogger<ThumbnailService> logger,
            VideoConfig videoConfig,
            ThumbnailCronJob thumbnailCronJob,
            IMapper mapper)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _videoConfig = videoConfig;
            _thumbnailCronJob = thumbnailCronJob;
            _mapper = mapper;
        }

        public string GetFolderThumbnail(string filePathName)
        {
            var thumbnailPath = Path.Combine(_videoConfig.ThumbnailPath, filePathName);
            if (!Directory.Exists(thumbnailPath))
                return "";

            var thumbnails = Directory.GetFiles(thumbnailPath)
                .OrderBy(x => x)
                .ToList();
            if (thumbnails.Count > 0)
                return thumbnails[0];

            foreach (var subpath in Directory.GetDirectories(thumbnailPath))
            {
                var tn = GetFolderThumbnail(subpath.Replace(_videoConfig.ThumbnailPath, ""));
                if (tn != "")
                    return tn;
            }

            return "";
        }

        public string GetFileThumbnail(string filePathName)
        {
            var path = Path.GetDirectoryName(filePathName);
            var thumbnail = Path.GetFileNameWithoutExtension(filePathName) + ".jpeg";
            var thumbnailPath = Path.Combine(path, thumbnail);

            thumbnailPath = Path.Combine(_videoConfig.ThumbnailPath, thumbnailPath);
            if (File.Exists(thumbnailPath))
                return thumbnailPath;

            return "";
        }

        public void DeleteThumbnail(string filePathName)
        {
            var path = Path.GetDirectoryName(filePathName);
            var thumbnail = Path.GetFileNameWithoutExtension(filePathName) + ".jpeg";
            var thumbnailPath = Path.Combine(path, thumbnail);

            thumbnailPath = Path.Combine(_videoConfig.ThumbnailPath, thumbnailPath);
            if (File.Exists(thumbnailPath))
                File.Delete(thumbnailPath);
        }

        public async Task<MetaDataDto> GetVideoMetaAsync(string filePathName, bool isStaging)
        {
            var rootPath = isStaging ? _videoConfig.StagePath : _videoConfig.VideoPath;
            var file = Path.Combine(rootPath, filePathName);

            var result = await ProcessHelper.RunProcessAsync(
                _videoConfig.FfprobeFile,
                $@"-hide_banner -i ""{file}""");

            //$@"-v quiet -print_format json -show_format -show_streams ""{file}""",

            /*try
            {
                output = JToken.Parse(output).ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch { }
            try
            {
                error = JToken.Parse(error).ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch { }*/

            return new MetaDataDto
            {
                Output = result.Output,
                Error = result.Error,
            };
        }

        public string GetWorkingThumbnail()
        {
            return _thumbnailCronJob.GetWorkingThunbmail();
        }

        public async Task<ThumbnailError> AddOrUpdateThumbnailErrorsAsync(FileDto file, string error)
        {
            try
            {
                var entity = await _appDbContext.ThumbnailErrors
                    .FirstOrDefaultAsync(x =>
                        x.File.FileName == file.FileName &&
                        x.File.FilePath == file.FilePath);

                if (entity == null)
                {
                    entity = new ThumbnailError
                    {
                        File = _mapper.Map<FileEntity>(file),
                    };
                    await _appDbContext.AddAsync(entity);
                }

                entity.TimesFailed++;
                entity.Error = error;
                entity.LastError = DateTime.Now;

                await _appDbContext.SaveChangesAsync();
                _appDbContext.Entry(entity).State = EntityState.Detached;

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<ThumbnailErrorDto>> GetThumbnailErrorsAsync()
        {
            try
            {
                var errors = await _appDbContext.ThumbnailErrors
                    .OrderByDescending(x => x.LastError)
                    .ToListAsync();

                return _mapper.Map<List<ThumbnailErrorDto>>(errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteThumbnailErrorAsync(FileDto file)
        {
            try
            {
                var entity = await _appDbContext.ThumbnailErrors
                    .FirstOrDefaultAsync(x =>
                        x.File.FileName == file.FileName &&
                        x.File.FilePath == file.FilePath);

                if (entity != null)
                {
                    _appDbContext.Entry(entity).State = EntityState.Deleted;
                    await _appDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
