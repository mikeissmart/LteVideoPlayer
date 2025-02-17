using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Service;
using System.Drawing;

namespace LteVideoPlayer.Api.CronJob.Convert
{
    public class ThumbnailCronJob
    {
        private readonly CancellationToken _cancellationToken;
        private readonly VideoConfig _videoConfig;
        private readonly IServiceProvider _services;
        private readonly int _imgWidth = 512;
        private readonly int _imgHeight = 512;
        private List<ThumbnailErrorDto> _thumbnailErrors;
        private string _currentThumbnail = "";

        public ThumbnailCronJob(
            IHostApplicationLifetime applicationLifetime,
            VideoConfig videoConfig,
            IServiceProvider services)
        {
            _cancellationToken = applicationLifetime.ApplicationStopping;
            _videoConfig = videoConfig;
            _services = services;
        }

        public void StartThumbnails()
        {
            Task.Run(async () => await ManageThumbnailsAsync());
        }

        public string GetWorkingThunbmail()
        {
            return _currentThumbnail.Replace(_videoConfig.VideoPath, "");
        }

        private async Task ManageThumbnailsAsync()
        {
            using (var scope = _services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ThumbnailCronJob>>();
                var thumbnailService = scope.ServiceProvider.GetRequiredService<IThumbnailService>();

                if (!Directory.Exists(_videoConfig.ThumbnailPath))
                    Directory.CreateDirectory(_videoConfig.ThumbnailPath);
                CheckDefaultThumbnail();

                while (!_cancellationToken.IsCancellationRequested)
                {
                    _thumbnailErrors = await thumbnailService.GetThumbnailErrorsAsync();

                    var missingThumbnails = GetDirectoryThumbnails("", false);
                    if (_cancellationToken.IsCancellationRequested)
                        break;

                    if (missingThumbnails.Count > 0)
                    {
                        foreach (var thumbnail in missingThumbnails)
                        {
                            _currentThumbnail = $"Create Thumbnail: {thumbnail}";
                            await CreateThumbnailAsync(thumbnail, logger, thumbnailService);
                            if (_cancellationToken.IsCancellationRequested)
                                break;
                        }
                    }
                    else
                    {
                        var pruneThumbnails = GetDirectoryThumbnails("", true);
                        if (pruneThumbnails.Count > 0)
                        {
                            foreach (var thumbnail in pruneThumbnails)
                            {
                                _currentThumbnail = $"Prune Thumbnail: {thumbnail}";
                                await PruneThumbnailAsync(thumbnail, thumbnailService);
                                if (_cancellationToken.IsCancellationRequested)
                                    break;
                            }
                        }
                        else
                        {
                            _currentThumbnail = $"Pending";
                            Thread.Sleep(5000);
                        }
                    }
                }
            }
        }

        private void CheckDefaultThumbnail()
        {
            var thumbnailPath = Path.Combine(_videoConfig.ThumbnailPath, _videoConfig.DefaultThumbnail);
            if (File.Exists(thumbnailPath))
                return;

            var img = new Bitmap(_imgWidth, _imgHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var c = Color.FromArgb(255, 33, 37, 41);
            for (int x = 0; x < _imgWidth; x++)
            {
                for (var y = 0; y < _imgHeight; y++)
                {
                    img.SetPixel(x, y, c);
                }
            }

            img.Save(thumbnailPath);
        }

        private List<FileDto> GetDirectoryThumbnails(string subpath, bool isPruning)
        {
            var thumbnailPath = Path.Combine(_videoConfig.ThumbnailPath, subpath);
            if (!Directory.Exists(thumbnailPath))
                Directory.CreateDirectory(thumbnailPath);
            var videoPath = Path.Combine(_videoConfig.VideoPath, subpath);

            var videoFiles = Directory.GetFiles(videoPath);
            if (videoFiles.Length > 0)
            {
                var thumbnails = new List<FileDto>();
                foreach (var thumbnail in GetThumbnails(subpath, isPruning))
                {
                    var thError = _thumbnailErrors.FirstOrDefault(x => x.File.FilePathName == thumbnail.FilePathName);
                    if (thError == null ||
                        (thError.LastError.AddDays(_videoConfig.RetryThumbnailAfterDays) < DateTime.Now && thError.TimesFailed < _videoConfig.MaxRetrys))
                        thumbnails.Add(thumbnail);
                }

                if (thumbnails.Count > 0)
                    return thumbnails;
            }

            var videoFolders = new List<DirectoryInfo>();
            foreach (var dir in Directory.GetDirectories(videoPath))
                videoFolders.Add(new DirectoryInfo(dir));
            videoFolders = isPruning
                ? videoFolders.OrderBy(x => x.LastWriteTimeUtc).ToList()
                : videoFolders.OrderByDescending(x => x.LastWriteTimeUtc).ToList();

            foreach (var folder in videoFolders)
            {
                var ret = GetDirectoryThumbnails(folder.FullName.Replace(_videoConfig.VideoPath, ""), isPruning);
                if (ret.Count > 0)
                    return ret;
            }

            return new List<FileDto>();
        }

        private List<FileDto> GetThumbnails(string subpath, bool isPruning)
        {
            var videos = FilePathsToFilesDto(
                Directory.GetFiles(Path.Combine(_videoConfig.VideoPath, subpath)),
                true);
            var thumbnails = FilePathsToFilesDto(
                Directory.GetFiles(Path.Combine(_videoConfig.ThumbnailPath, subpath)),
                false);

            var retThumbnail = new List<FileDto>();
            if (isPruning)
            {
                foreach (var thumbnail in thumbnails)
                {
                    if (!videos.Any(x => x.FilePathNameWithoutExtension == thumbnail.FilePathNameWithoutExtension))
                        retThumbnail.Add(thumbnail);
                }
            }
            else
            {
                foreach (var video in videos)
                {
                    if (!thumbnails.Any(x => x.FilePathNameWithoutExtension == video.FilePathNameWithoutExtension))
                        retThumbnail.Add(video);
                }
            }

            return retThumbnail;
        }

        private async Task CreateThumbnailAsync(
            FileDto file,
            ILogger<ThumbnailCronJob> logger,
            IThumbnailService thumbnailService)
        {
            (float duration, string durationError) = await GetDurationAsync(file, logger);
            var thumbnailFile = new FileDto
            {
                FileName = file.FileNameWithoutExtension + ".jpeg",
                FilePath = file.FilePath.Replace(_videoConfig.VideoPath, _videoConfig.ThumbnailPath)
            };

            if (duration <= 0)
            {
                await thumbnailService.AddOrUpdateThumbnailErrorsAsync(file, durationError);
                return;
            }

            var min = duration * (_videoConfig.ThumbnailMinPercent / 100.0);
            var max = duration * (_videoConfig.ThumbnailMaxPercent / 100.0);
            var frameTime = SecondsToHHMMSS(Random.Shared.Next((int)min, (int)max));

            var videoFullFilePathName = Path.Combine(_videoConfig.VideoPath, file.FilePathName);
            var thumbnailFullFilePathName = Path.Combine(_videoConfig.ThumbnailPath, thumbnailFile.FilePathName);

            var threadStr = _videoConfig.FfmpegThreads > 0
                ? $"-threads {_videoConfig.FfmpegThreads}"
                : "";

            var result = await ProcessHelper.RunProcessAsync(
                _videoConfig.FfmpegFile,
                $@"-i ""{videoFullFilePathName}"" -ss {frameTime} {threadStr} -vf ""scale=-1:{_imgHeight}"" -q:v 2 -vframes 1 ""{thumbnailFullFilePathName}""",
                _cancellationToken);

            if (!File.Exists(thumbnailFullFilePathName))
                await thumbnailService.AddOrUpdateThumbnailErrorsAsync(file, result.Error);
            else
                await thumbnailService.DeleteThumbnailErrorAsync(file);
        }

        private async Task PruneThumbnailAsync(FileDto file, IThumbnailService thumbnailService)
        {
            File.Delete(Path.Combine(_videoConfig.ThumbnailPath, file.FilePathName));
            await thumbnailService.DeleteThumbnailErrorAsync(file);
        }

        private async Task<(float, string)> GetDurationAsync(
            FileDto file,
            ILogger<ThumbnailCronJob> logger)
        {
            var path = Path.Combine(_videoConfig.VideoPath, file.FilePathName);

            var result = await ProcessHelper.RunProcessAsync(
                _videoConfig.FfprobeFile,
                $@"-i ""{path}"" -show_entries format=duration -v quiet -of csv=""p=0""",
                _cancellationToken);

                try
                {
                    return (float.Parse(result.Output.Replace("\r\n", "")), "");
                }
                catch (Exception ex)
                {
                    var thumbnailError = $"Unable to get duration: Path - {path}, Output - {result.Output}, Error - {result.Error}";
                    logger.LogError(thumbnailError);
                    return (-1, thumbnailError);
                }
        }

        private string SecondsToHHMMSS(float seconds)
        {
            var t = TimeSpan.FromSeconds(seconds);
            return $"{((int)t.TotalHours).ToString("D2")}:{t.Minutes.ToString("D2")}:{t.Seconds.ToString("D2")}";
        }

        private FileDto FilePathToFileDto(string path, bool replaceVideoPath)
        {
            var file = Path.GetFileName(path);
            var subPath = Path.GetDirectoryName(path)! + "\\";
            
            if (replaceVideoPath)
                subPath = subPath.Replace(_videoConfig.VideoPath, "");
            else
                subPath = subPath.Replace(_videoConfig.ThumbnailPath, "");

            return new FileDto
            {
                FileName = file,
                FilePath = subPath
            };
        }

        private List<FileDto> FilePathsToFilesDto(IEnumerable<string> paths, bool replaceVideoPath)
        {
            var files = new List<FileDto>();

            foreach (var path in paths)
                files.Add(FilePathToFileDto(path, replaceVideoPath));

            return files;
        }
    }
}
