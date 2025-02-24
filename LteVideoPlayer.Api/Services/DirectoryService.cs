using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Models.Dtos;
using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Enums;
using LteVideoPlayer.Api.Persistance.Repositories;
using Microsoft.Extensions.Logging;
using System.IO;

namespace LteVideoPlayer.Api.Services
{
    public interface IDirectoryService : IService
    {
        Task<DirsAndFilesDto> GetDirsAndFilesAsync(DirectoryEnum dirEnum, string path, bool useThumbnailDir);
        FileDto? GetNextFile(DirectoryEnum dirEnum, FileDto file);
    }

    public class DirectoryService : BaseService, IDirectoryService
    {
        private readonly ILogger<DirectoryService> _logger;
        private readonly IVideoConfigService _videoConfigService;
        private readonly IConvertFileRepository _convertFileRepository;

        public DirectoryService(ILogger<DirectoryService> logger, IVideoConfigService videoConfigService, IConvertFileRepository convertFileRepository)
        {
            _logger = logger;
            _videoConfigService = videoConfigService;
            _convertFileRepository = convertFileRepository;
        }

        public async Task<DirsAndFilesDto> GetDirsAndFilesAsync(DirectoryEnum dirEnum, string path, bool useThumbnailDir)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                var dirFiles = new DirsAndFilesDto
                {
                    Dirs = GetDirs(path, videoConfig, useThumbnailDir),
                    Files = GetFiles(path, videoConfig, useThumbnailDir)
                        .Where(x => !x.File.Contains("Converting_"))
                        .ToList()
                };

                foreach (var file in dirFiles.Files)
                {
                    var convertFiles = await _convertFileRepository.GetConvertFilesByOriginalAsync(dirEnum, file.Path, file.File);
                    file.IsConvertQueued = convertFiles.Any(x => x.EndedDate == null);
                }

                return dirFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public FileDto? GetNextFile(DirectoryEnum dirEnum, FileDto file)
        {
            try
            {
                FileDto? nextFile = null;
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                var files = GetFiles(file.Path, videoConfig, false);

                var fileIndex = files.FindIndex(x => x.FullPath == file.FullPath);
                if (fileIndex == -1)
                    throw new FileNotFoundException($"File not found {file.FullPath}");

                if (fileIndex == files.Count - 1)
                {
                    // Last file go to next dir
                    var fileVideoFullPath = Path.Combine(videoConfig.RootVideoDir, file.Path);
                    var parentVideoFullPath = Directory.GetParent(fileVideoFullPath)!.FullName;
                    var dirs = GetDirs(parentVideoFullPath, videoConfig, false);

                    var dirIndex = dirs.FindIndex(x => x.FullPath == fileVideoFullPath);
                    if (dirIndex > -1 && dirIndex != dirs.Count - 1)
                    {
                        files = GetFiles(dirs[dirIndex + 1].Path, videoConfig, false);
                        if (files.Count > 0)
                            nextFile = files[0];
                    }
                }
                else
                {
                    nextFile = files[fileIndex + 1];
                }

                return nextFile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private List<DirDto> GetDirs(string path, VideoConfig videoConfig, bool useThumbnailDir)
        {
            var fullPath = Path.Combine(useThumbnailDir ? videoConfig.RootThumbnailDir! : videoConfig.RootVideoDir, path);
            return Directory.GetDirectories(fullPath)
                .OrderBy(x => x)
                .ToList()
                .Select(x => new DirDto
                {
                    Path = path,
                    Name = x.Replace(fullPath, "").Substring(1)
                })
                .ToList();
        }

        private List<FileDto> GetFiles(string path, VideoConfig videoConfig, bool useThumbnailDir)
        {
            var fullPath = Path.Combine(useThumbnailDir ? videoConfig.RootThumbnailDir! : videoConfig.RootVideoDir, path);
            return Directory.GetFiles(fullPath)
                .OrderBy(x => x)
                .ToList()
                .Select(x => new FileDto
                {
                    Path = path,
                    File = Path.GetFileName(x),
                })
                .ToList();
        }
    }
}
