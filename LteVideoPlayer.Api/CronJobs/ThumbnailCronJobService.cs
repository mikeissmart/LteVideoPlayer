﻿using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Models.Dtos;
using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Enums;
using LteVideoPlayer.Api.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LteVideoPlayer.Api.CronJobs
{
    public class ThumbnailCronJobService : BaseCronJobService
    {
        private readonly FfmegConfig _fmegConfig;
        private readonly int _imgWidth = 512;
        private readonly int _imgHeight = 512;
        private ILogger<ThumbnailCronJobService> _logger;
        private IVideoConfigService _videoConfigService;
        private IDirectoryService _directoryService;
        private IThumbnailService _thumbnailService;
        private List<ThumbnailErrorDto> _thumbnailErrors;
        private string _currentThumbnail = "";

        public ThumbnailCronJobService(FfmegConfig fmegConfig, CronJobConfig config, IServiceProvider services)
            : base(config, services, nameof(CronJobConfig.ThumbnailCronJob))
        {
            _fmegConfig = fmegConfig;
        }

        public string CurrentThumbnail() => _currentThumbnail;

        protected override async Task<string?> DoWorkAsync(IServiceScope scope, CancellationToken cancellationToken)
        {
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<ThumbnailCronJobService>>();
            _videoConfigService = scope.ServiceProvider.GetRequiredService<IVideoConfigService>();
            _directoryService = scope.ServiceProvider.GetRequiredService<IDirectoryService>();
            _thumbnailService = scope.ServiceProvider.GetRequiredService<IThumbnailService>();

            _thumbnailErrors = await _thumbnailService.GetAllThumbnailErrorsAsync();
            foreach (DirectoryEnum dirEnum in Enum.GetValues(typeof(DirectoryEnum)))
            {
                if (dirEnum == DirectoryEnum.Unknown)
                    continue;

                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanThumbnailVideo)
                    continue;

                if (!Directory.Exists(videoConfig.RootVideoDir))
                    Directory.CreateDirectory(videoConfig.RootVideoDir);
                if (!Directory.Exists(videoConfig.RootThumbnailDir!))
                    Directory.CreateDirectory(videoConfig.RootThumbnailDir!);

                await CreateThumbnailsAsync(videoConfig, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;

                await PruneThumbnailsAsync(videoConfig, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
            }

            return null;
        }

        private async Task CreateThumbnailsAsync(VideoConfig videoConfig, CancellationToken cancellationToken)
        {
            var videos = await GetDirectoryVideosMissingThumbnailsAsync(videoConfig, "");
            if (cancellationToken.IsCancellationRequested)
                return;

            foreach (var video in videos)
            {
                _currentThumbnail = $"Creating thumbnail for: {video.FullPath}";
                await CreateThunbnailAsync(videoConfig, video, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }

        private async Task<List<FileDto>> GetDirectoryVideosMissingThumbnailsAsync(VideoConfig videoConfig, string path)
        {
            var missingVideos = new List<FileDto>();
            var thumbnailRootFullPath = Path.Combine(videoConfig.RootThumbnailDir!, path);
            if (!Directory.Exists(thumbnailRootFullPath))
                Directory.CreateDirectory(thumbnailRootFullPath);

            var dirsAndFiles = await _directoryService.GetDirsAndFilesAsync(videoConfig.DirectoryEnum, path, false);
            if (dirsAndFiles.Files.Count > 0)
            {
                var thumbnails = FilePathsToFilesDto(
                    videoConfig,
                    Directory.GetFiles(Path.Combine(videoConfig.RootThumbnailDir!, path)),
                    false);

                dirsAndFiles.Files = dirsAndFiles.Files
                    .Where(x => !thumbnails.Any(y => x.FileWOExt == y.FileWOExt))
                    .ToList();

                foreach (var file in dirsAndFiles.Files)
                {
                    var error = _thumbnailErrors.FirstOrDefault(x => x.File.FullPath ==  file.FullPath);
                    if (error == null ||
                        (error.LastError.AddDays(videoConfig.RetryThumbnailAfterDays) < DateTime.Now 
                        && error.TimesFailed < videoConfig.MaxThumbnailRetrys!))
                    {
                        missingVideos.Add(file);
                    }
                }
            }

            foreach (var dir in dirsAndFiles.Dirs)
                missingVideos.AddRange(await GetDirectoryVideosMissingThumbnailsAsync(videoConfig, dir.FullPath));

            return missingVideos;
        }

        private List<FileDto> GetPrunableThumbnails(VideoConfig videoConfig, List<FileDto> videos, string path)
        {
            var thumbnails = FilePathsToFilesDto(
                videoConfig,
                Directory.GetFiles(Path.Combine(videoConfig.RootThumbnailDir!, path)),
                false);

            var retThumbnail = new List<FileDto>();
            foreach (var thumbnail in thumbnails)
            {
                if (!videos.Any(x => x.FileWOExt == thumbnail.FileWOExt))
                    retThumbnail.Add(thumbnail);
            }

            return retThumbnail;
        }

        private List<FileDto> FilePathsToFilesDto(VideoConfig videoConfig, IEnumerable<string> paths, bool isVideoPath)
        {
            var files = new List<FileDto>();
            foreach (var path in paths)
            {
                var subPath = Path.GetDirectoryName(path)!;

                if (isVideoPath)
                    subPath = subPath.Replace(videoConfig.RootVideoDir, "");
                else
                    subPath = subPath.Replace(videoConfig.RootThumbnailDir!, "");

                files.Add(new FileDto
                {
                    File = Path.GetFileName(path),
                    Path = subPath
                });

            }

            return files;
        }

        private async Task CreateThunbnailAsync(VideoConfig videoConfig, FileDto videoFile, CancellationToken cancellationToken)
        {
            (float duration, string durationError) = await GetDurationAsync(videoConfig, videoFile, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;

            if (duration <= 0)
            {
                await _thumbnailService.AddOrUpdateThumbnailErrorsAsync(videoConfig.DirectoryEnum, videoFile, durationError);
                return;
            }

            var min = duration * (videoConfig.ThumbnailMinSeekPercent! / 100.0);
            var max = duration * (videoConfig.ThumbnailMaxSeekPercent! / 100.0);
            var frameTime = SecondsToHHMMSS(Random.Shared.Next((int)min, (int)max));
            var thumbnailFile = new FileDto
            {
                File = videoFile.FileWOExt + ".jpeg",
                Path = videoFile.Path,
            };
            var videoRootFullPath = Path.Combine(videoConfig.RootVideoDir, videoFile.FullPath);
            var thumbnailRootFullPath = Path.Combine(videoConfig.RootThumbnailDir!, thumbnailFile.FullPath);

            var threadStr = _fmegConfig.FfmpegThreads > 0
                ? $"-threads {_fmegConfig.FfmpegThreads}"
                : "";

            var result = await ProcessHelper.RunProcessAsync(
                _fmegConfig.RootFfmpegFile,
                $@"-i ""{videoRootFullPath}"" -ss {frameTime} {threadStr} -vf ""scale=-1:{_imgHeight}"" -q:v 2 -vframes 1 ""{thumbnailRootFullPath}""",
                cancellationToken);

            if (!File.Exists(thumbnailRootFullPath))
                await _thumbnailService.AddOrUpdateThumbnailErrorsAsync(videoConfig.DirectoryEnum, videoFile, result.Error);
            else
                await _thumbnailService.DeleteThumbnailErrorAsync(videoConfig.DirectoryEnum, videoFile);
        }

        private async Task<(float, string)> GetDurationAsync(VideoConfig videoConfig, FileDto file, CancellationToken cancellationToken)
        {
            var result = await ProcessHelper.RunProcessAsync(
                _fmegConfig.RootFfprobeFile,
                $@"-i ""{Path.Combine(videoConfig.RootVideoDir, file.FullPath)}"" -show_entries format=duration -v quiet -of csv=""p=0""",
                cancellationToken);

            try
            {
                return (float.Parse(result.Output.Replace("\r\n", "")), "");
            }
            catch (Exception ex)
            {
                var thumbnailError = $"Unable to get duration: Path - {file.FullPath}, Output - {result.Output}, Error - {result.Error}";
                _logger.LogError(thumbnailError);
                return (-1, thumbnailError);
            }
        }

        private string SecondsToHHMMSS(float seconds)
        {
            var t = TimeSpan.FromSeconds(seconds);
            return $"{((int)t.TotalHours).ToString("D2")}:{t.Minutes.ToString("D2")}:{t.Seconds.ToString("D2")}";
        }

        private async Task PruneThumbnailsAsync(VideoConfig videoConfig, CancellationToken cancellationToken)
        {
            var fileThumbnails = await GetFileThumbnailsWithoutVideosAsync(videoConfig, "");
            if (cancellationToken.IsCancellationRequested)
                return;

            foreach (var thumbnail in fileThumbnails)
            {
                File.Delete(Path.Combine(videoConfig.RootThumbnailDir!, thumbnail.FullPath));
                if (cancellationToken.IsCancellationRequested)
                    break;
            }

            (List<DirDto> directoryThumbnails, bool _) = await GetDirectoryThumbnailsWithoutVideosAsync(videoConfig, "");
            if (cancellationToken.IsCancellationRequested)
                return;

            foreach (var thumbnail in directoryThumbnails)
            {
                Directory.Delete(Path.Combine(videoConfig.RootThumbnailDir!, thumbnail.FullPath));
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }

        private async Task<List<FileDto>> GetFileThumbnailsWithoutVideosAsync(VideoConfig videoConfig, string path)
        {
            var pruneThumbnails = new List<FileDto>();

            var dirsAndFiles = await _directoryService.GetDirsAndFilesAsync(videoConfig.DirectoryEnum, path, true);
            if (dirsAndFiles.Files.Count > 0)
            {
                var rootVideoDirPath = Path.Combine(videoConfig.RootVideoDir, path);
                if (Directory.Exists(rootVideoDirPath))
                {
                    var videos = FilePathsToFilesDto(
                        videoConfig,
                        Directory.GetFiles(Path.Combine(videoConfig.RootVideoDir, path)),
                        true);

                    pruneThumbnails.AddRange(dirsAndFiles.Files
                        .Where(x => !videos.Any(y => x.FileWOExt == y.FileWOExt)));
                }
                else
                    pruneThumbnails.AddRange(dirsAndFiles.Files);
            }

            return pruneThumbnails;
        }

        private async Task<(List<DirDto>, bool)> GetDirectoryThumbnailsWithoutVideosAsync(VideoConfig videoConfig, string path)
        {
            var pruneThumbnails = new List<DirDto>();

            var addSelf = false;
            var dirsAndFiles = await _directoryService.GetDirsAndFilesAsync(videoConfig.DirectoryEnum, path, true);
            if (dirsAndFiles.Dirs.Count == 0 && dirsAndFiles.Files.Count == 0)
                addSelf = true;
            else if (dirsAndFiles.Files.Count == 0)
            {
                foreach (var dir in dirsAndFiles.Dirs)
                {
                    (List<DirDto> result, bool addDir) = await GetDirectoryThumbnailsWithoutVideosAsync(videoConfig, dir.FullPath);
                    if (addDir)
                        pruneThumbnails.Add(dir);
                    pruneThumbnails.AddRange(result);
                }
            }

            return (pruneThumbnails, addSelf);
        }
    }
}
