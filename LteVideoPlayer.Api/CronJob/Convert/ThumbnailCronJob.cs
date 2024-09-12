using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Helpers;
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
        private readonly List<string> _errorPaths = new List<string>();

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
            Task.Run(() => ManageThumbnails());
        }

        private void ManageThumbnails()
        {
            using (var scope = _services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ThumbnailCronJob>>();

                if (!Directory.Exists(_videoConfig.ThumbnailPath))
                    Directory.CreateDirectory(_videoConfig.ThumbnailPath);
                CheckDefaultThumbnail();

                while (!_cancellationToken.IsCancellationRequested)
                {
                    var missingThumbnails = GetDirectoryThumbnails("", false);
                    if (_cancellationToken.IsCancellationRequested)
                        break;

                    if (missingThumbnails.Count > 0)
                    {
                        foreach (var thumbnail in missingThumbnails)
                        {
                            CreateThumbnail(thumbnail, _videoConfig, logger, _cancellationToken);
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
                                PruneThumbnail(thumbnail);
                                if (_cancellationToken.IsCancellationRequested)
                                    break;
                            }
                        }
                        else
                            Thread.Sleep(5000);
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

        private List<string> GetDirectoryThumbnails(string subpath, bool isPruning)
        {
            var thumbnailPath = Path.Combine(_videoConfig.ThumbnailPath, subpath);
            if (!Directory.Exists(thumbnailPath))
                Directory.CreateDirectory(thumbnailPath);
            var videoPath = Path.Combine(_videoConfig.VideoPath, subpath);

            var videoFiles = Directory.GetFiles(videoPath);
            if (videoFiles.Length > 0)
            {
                var thumbnails = GetThumbnails(subpath, isPruning)
                    .Where(x => !_errorPaths.Contains(x))
                    .ToList();
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

            return new List<string>();
        }

        private List<string> GetThumbnails(string subpath, bool isPruning)
        {
            var thumbnailPath = Path.Combine(_videoConfig.ThumbnailPath, subpath);
            var thumbnails = Directory.GetFiles(thumbnailPath);

            var videoPath = Path.Combine(_videoConfig.VideoPath, subpath);
            var videos = Directory.GetFiles(videoPath);

            var retThumbnail = new List<string>();
            if (isPruning)
            {
                for (var i = 0; i < videos.Length; i++)
                    videos[i] = Path.GetFileNameWithoutExtension(videos[i]);

                for (var i = 0; i < thumbnails.Length; i++)
                {
                    var tn = Path.GetFileNameWithoutExtension(thumbnails[i]);
                    if (!videos.Any(x => x == tn))
                        retThumbnail.Add(thumbnails[i]);
                }
            }
            else
            {
                for (var i = 0; i < thumbnails.Length; i++)
                    thumbnails[i] = Path.GetFileNameWithoutExtension(thumbnails[i]);

                for (var i = 0; i < videos.Length; i++)
                {
                    var v = Path.GetFileNameWithoutExtension(videos[i]);
                    if (!thumbnails.Any(x => x == v))
                        retThumbnail.Add(videos[i]);
                }
            }

            return retThumbnail;
        }

        private void CreateThumbnail(
            string path,
            VideoConfig config,
            ILogger<ThumbnailCronJob> logger,
            CancellationToken cancellationToken)
        {
            var duration = GetDuration(path, config, logger, cancellationToken);
            if (duration <= 0 || cancellationToken.IsCancellationRequested)
            {
                _errorPaths.Add(path);
                return;
            }

            var min = duration * (config.ThumbnailMinPercent / 100.0);
            var max = duration * (config.ThumbnailMaxPercent / 100.0);
            var frameTime = SecondsToHHMMSS(Random.Shared.Next((int)min, (int)max));

            var thumbnailName = Path.GetFileNameWithoutExtension(path) + ".jpeg";
            var thumbnailPath = Path.GetDirectoryName(path)!;
            thumbnailPath = thumbnailPath.Replace(config.VideoPath, config.ThumbnailPath);
            thumbnailPath = Path.Combine(thumbnailPath, thumbnailName);

            var threadStr = config.FfmpegThreads > 0
                ? $"-threads {config.FfmpegThreads}"
                : "";

            ProcessHelper.RunProcess(
                config.FfmpegFile,
                $@"-i ""{path}"" -ss {frameTime} {threadStr} -vf ""scale=-1:{_imgHeight}"" -q:v 2 -vframes 1 ""{thumbnailPath}""",
                out var output,
                out var error,
                cancellationToken);
        }

        private void PruneThumbnail(string subpath)
        {
            File.Delete(Path.Combine(_videoConfig.ThumbnailPath, subpath));
        }

        private float GetDuration(
            string path,
            VideoConfig config,
            ILogger<ThumbnailCronJob> logger,
            CancellationToken cancellationToken)
        {
            ProcessHelper.RunProcess(
                config.FfprobeFile,
                $@"-i ""{path}"" -show_entries format=duration -v quiet -of csv=""p=0""",
                out var output,
                out var error,
                cancellationToken);

                try
                {
                    return float.Parse(output.Replace("\r\n", ""));
                }
                catch (Exception ex)
                {
                    logger.LogError($"Unable to get duration: Path - {path}, Output - {output}, Error - {error}");
                    return -1;
                }
        }

        private string SecondsToHHMMSS(float seconds)
        {
            var t = TimeSpan.FromSeconds(seconds);
            return $"{((int)t.TotalHours).ToString("D2")}:{t.Minutes.ToString("D2")}:{t.Seconds.ToString("D2")}";
        }
    }
}
