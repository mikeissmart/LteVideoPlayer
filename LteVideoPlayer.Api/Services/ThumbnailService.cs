using AutoMapper;
using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.CronJobs;
using LteVideoPlayer.Api.Models.DataTypes;
using LteVideoPlayer.Api.Models.Dtos;
using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Enums;
using LteVideoPlayer.Api.Persistance;
using LteVideoPlayer.Api.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LteVideoPlayer.Api.Services
{
    public interface IThumbnailService : IService
    {
        string GetFolderThumbnail(DirectoryEnum dirEnum, string fullPath);
        string GetFileThumbnail(DirectoryEnum dirEnum, string fullPath);
        void DeleteThumbnail(DirectoryEnum dirEnum, string fullPath);
        Task<ThumbnailError> AddOrUpdateThumbnailErrorsAsync(DirectoryEnum dirEnum, FileDto file, string error);
        Task<List<ThumbnailErrorDto>> GetAllThumbnailErrorsAsync();
        Task DeleteThumbnailErrorAsync(DirectoryEnum dirEnum, FileDto file);
        string GetCurrentThumbnail();
    }

    public class ThumbnailService : BaseService, IThumbnailService
    {
        private readonly ILogger<ThumbnailService> _logger;
        private readonly IMapper _mapper;
        private readonly IVideoConfigService _videoConfigService;
        private readonly ThumbnailCronJobService _thumbnailCronJobService;
        private readonly IThumbnailErrorRepository _thumbnailErrorRepository;

        public ThumbnailService(ILogger<ThumbnailService> logger, IMapper mapper, IVideoConfigService videoConfigService, ThumbnailCronJobService thumbnailCronJobService, IThumbnailErrorRepository thumbnailErrorRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _videoConfigService = videoConfigService;
            _thumbnailCronJobService = thumbnailCronJobService;
            _thumbnailErrorRepository = thumbnailErrorRepository;
        }

        public string GetFolderThumbnail(DirectoryEnum dirEnum, string fullPath)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanThumbnailVideo)
                    throw new Exception("Thumbnails disabled for this directory");

                var thumbnailRootFullPath = Path.Combine(videoConfig.RootThumbnailDir!, fullPath);
                if (!Directory.Exists(thumbnailRootFullPath))
                    return "";

                var thumbnails = Directory.GetFiles(thumbnailRootFullPath)
                    .OrderBy(x => x)
                    .ToList();
                if (thumbnails.Count > 0)
                    return thumbnails[0];

                foreach (var subpath in Directory.GetDirectories(thumbnailRootFullPath))
                {
                    var tn = GetFolderThumbnail(dirEnum, subpath.Replace(videoConfig.RootThumbnailDir, "").Substring(1));
                    if (tn != "")
                        return tn;
                }

                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public string GetFileThumbnail(DirectoryEnum dirEnum, string fullPath)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanThumbnailVideo)
                    throw new Exception("Thumbnails disabled for this directory");

                var thumbnailRootFullPath = GetThumbnailRootFullPath(videoConfig, fullPath);
                if (File.Exists(thumbnailRootFullPath))
                    return thumbnailRootFullPath;

                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public void DeleteThumbnail(DirectoryEnum dirEnum, string fullPath)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanThumbnailVideo)
                    throw new Exception("Thumbnails disabled for this directory");

                var thumbnailRootFullPath = GetThumbnailRootFullPath(videoConfig, fullPath);
                if (File.Exists(thumbnailRootFullPath))
                    File.Delete(thumbnailRootFullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<ThumbnailError> AddOrUpdateThumbnailErrorsAsync(DirectoryEnum dirEnum, FileDto file, string error)
        {
            try
            {
                var entity = await _thumbnailErrorRepository.GetThumbnailErrorByPathAndFileAsync(dirEnum, file.Path, file.File);
                if (entity == null)
                {
                    entity = new ThumbnailError
                    {
                        DirectoryEnum = dirEnum,
                        Error = error,
                        File = _mapper.Map<FileDataType>(file),
                    };
                    await _thumbnailErrorRepository.AddAsync(entity);
                }

                entity.TimesFailed++;
                entity.Error = error;
                entity.LastError = DateTime.Now;

                await _thumbnailErrorRepository.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<ThumbnailErrorDto>> GetAllThumbnailErrorsAsync()
        {
            try
            {
                return _mapper.Map<List<ThumbnailErrorDto>>(await _thumbnailErrorRepository.GetAllThumbnailErrorsAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteThumbnailErrorAsync(DirectoryEnum dirEnum, FileDto file)
        {
            try
            {
                var entity = await _thumbnailErrorRepository.GetThumbnailErrorByPathAndFileAsync(dirEnum, file.Path, file.File);
                if (entity != null)
                {
                    _thumbnailErrorRepository.Remove(entity);
                    await _thumbnailErrorRepository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public string GetCurrentThumbnail()
        {
            return _thumbnailCronJobService.CurrentThumbnail();
        }

        private string GetThumbnailRootFullPath(VideoConfig config, string fullPath)
        {
            var thumbnailRootFullPath = Path.Combine(config.RootThumbnailDir!, fullPath);

            // Think the file is sent and this gets the directory from it
            var path = Path.GetDirectoryName(thumbnailRootFullPath)!;
            var thumbnailFile = Path.GetFileNameWithoutExtension(fullPath) + ".jpeg";
            var thumbnailPath = Path.Combine(path, thumbnailFile);

            return Path.Combine(config.RootThumbnailDir!, thumbnailPath);
        }
    }
}
