using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Service;

namespace LteVideoPlayer.Api.CronJob.Convert
{
    public class ConvertQueueCronJob
    {
        private static readonly int _threadCount = Environment.ProcessorCount;

        private readonly CancellationToken _cancellationToken;
        private readonly VideoConfig _videoConfig;
        private readonly int _concurrentConverts;
        private readonly IServiceProvider _services;
        private List<ConvertTask> _runningTasks;

        public ConvertQueueCronJob(
            IHostApplicationLifetime applicationLifetime,
            VideoConfig videoConfig,
            IServiceProvider services)
        {
            _cancellationToken = applicationLifetime.ApplicationStopping;
            _videoConfig = videoConfig;
            _services = services;
            _concurrentConverts = 1;
            _runningTasks = new List<ConvertTask>();
        }

        public void StartQueue()
        {
            Task.Run(() => WatchQueue());
        }

        public List<ConvertFileDto> CurrentConverts()
        {
            lock (_runningTasks)
                return new List<ConvertFileDto>(_runningTasks.Select(x => x.ConvertFile));
        }

        private void WatchQueue()
        {
            using (var scope = _services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ConvertQueueCronJob>>();
                var convertFileService = scope.ServiceProvider.GetRequiredService<IConvertFileService>();
                var directoryService = scope.ServiceProvider.GetRequiredService<IDirectoryService>();

                if (!Directory.Exists(_videoConfig.VideoPath))
                    Directory.CreateDirectory(_videoConfig.VideoPath);
                if (!Directory.Exists(_videoConfig.StagePath))
                    Directory.CreateDirectory(_videoConfig.StagePath);

                lock (_runningTasks)
                    _runningTasks.Clear();
                var queuedConverts = new List<ConvertFileDto>();
                List<ConvertFileDto> nonQueuedConverts;
                while (!_cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        lock (convertFileService)
                        {
                            // Lower how many times the database is hit
                            if (_runningTasks.Count != _concurrentConverts)
                            {
                                var task = convertFileService.GetIncompleteConvertsAsync(_concurrentConverts * 2);
                                task.Wait();
                                nonQueuedConverts = task.Result
                                    .Where(x => queuedConverts.Where(y => x.Id == y.Id).Count() == 0)
                                    .ToList();
                            }
                            else
                                nonQueuedConverts = new List<ConvertFileDto>();
                        }

                        if (nonQueuedConverts.Count == 0 && _runningTasks.Count == 0)
                            Task.Delay(6000, _cancellationToken).Wait(_cancellationToken);
                        else if (_runningTasks.Count != _concurrentConverts)
                        {
                            while (!_cancellationToken.IsCancellationRequested &&
                                _runningTasks.Count != _concurrentConverts &&
                                nonQueuedConverts.Count > 0)
                            {
                                var convert = nonQueuedConverts[0];
                                nonQueuedConverts.RemoveAt(0);
                                queuedConverts.Add(convert);

                                lock (_runningTasks)
                                {
                                    _runningTasks.Add(new ConvertTask
                                    {
                                        Task = Task.Run(() => ProcessConvert(
                                            _cancellationToken,
                                            convertFileService,
                                            logger,
                                            convert,
                                            _videoConfig
                                            )),
                                        ConvertFile = convert
                                    });
                                }
                            }
                        }

                        for (var i = 0; i < _runningTasks.Count; i++)
                        {
                            var task = _runningTasks[i];
                            if (task.Task.IsCompleted)
                            {
                                lock (_runningTasks)
                                {
                                    queuedConverts.RemoveAt(i);
                                    _runningTasks.RemoveAt(i);
                                }
                                i--;
                            }
                        }

                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }
                }
            }
        }

        private static void ProcessConvert(
            CancellationToken cancellationToken,
            IConvertFileService convertFileService,
            ILogger<ConvertQueueCronJob> logger,
            ConvertFileDto convert,
            VideoConfig config)
        {
            var stage = "";
            try
            {
                var renameFileName = Path.GetExtension(convert.OriginalFile.FileName).Contains("_converting")
                    ? convert.OriginalFile.FileName
                    : convert.OriginalFile.FileName + "_converting";
                var renameFilePathName = Path.Combine(convert.OriginalFile.FilePath, renameFileName);
                var convertedFileName = "Converting_" + convert.ConvertedFile.FileName;
                var convertedFilePathName = Path.Combine(convert.OriginalFile.FilePath, convertedFileName);

                stage = "Renaming original file";
                if (!File.Exists(config.StagePath + renameFilePathName))
                {
                    File.Move(
                        config.StagePath + convert.OriginalFile.FilePathName,
                        config.StagePath + renameFilePathName,
                        true);
                }
                convert.StartedDate = DateTime.Now;
                lock (convertFileService)
                {
                    convertFileService.UpdateConvertAsync(convert).Wait();
                }

                var threadStr = config.FfmpegThreads > 0
                    ? $"-threads {config.FfmpegThreads}"
                    : "";

                var result = ProcessHelper.RunProcess(
                    config.FfmpegFile,
                    $@"-i ""{config.StagePath + renameFilePathName}"" {threadStr} -map 0:v:0 -map 0:a:{convert.AudioStream} -c:v libx264 -crf 23 -profile:v baseline -level 3.0 -pix_fmt yuv420p -c:a aac -ac 2 -b:a 128k -y ""{config.StagePath + convertedFilePathName}""");

                /*var threadStr = config.FfmpegThreads > 0
                    ? $" -threads {config.FfmpegThreads} "
                    : "";
                var startInfo = new ProcessStartInfoS
                {
                    FileName = config.FfmpegFile,
                    // TODO added -map 0:v:0 -map 0:a:1, to select different audio track for english
                    Arguments = $@"-i ""{config.StagePath + renameFilePathName}"" {threadStr} -c:v libx264 -crf 23 -profile:v baseline -level 3.0 -pix_fmt yuv420p -c:a aac -ac 2 -b:a 128k -y ""{config.StagePath + convertedFilePathName}""",
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                };

                stage = "Converting Original File";
                var output = new StringBuilder();
                using (var process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.ErrorDataReceived += (s, e) =>
                    {
                        lock (output)
                        {
                            output.AppendLine(e.Data);
                        }
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    while (!process.HasExited && !cancellationToken.IsCancellationRequested)
                    {
                        Thread.Sleep(5000);
                    }
                    if (cancellationToken.IsCancellationRequested)
                    {
                        process.Kill(true);
                        return;
                    }
                }
                convert.Output = output.ToString();*/

                stage = "Check if convert sucessfull";
                if (!File.Exists(config.StagePath + convertedFilePathName))
                {
                    convert.Errored = true;
                    convert.Output = result.Error;
                }
                else
                {
                    stage = "Create Convert directory";
                    // Convert successful
                    var pathParts = convert.ConvertedFile.FilePath.Split("\\");
                    var checkPath = config.VideoPath;
                    for (var i = 0; i < pathParts.Length; i++)
                    {
                        checkPath = Path.Combine(checkPath, pathParts[i]);
                        if (!Directory.Exists(checkPath))
                            Directory.CreateDirectory(checkPath);
                    }

                    stage = "Move Convert File";
                    File.Move(
                        config.StagePath + convertedFilePathName,
                        config.VideoPath + convert.ConvertedFile.FilePathName,
                        true);

                    stage = "Delete Original File";
                    File.Delete(config.StagePath + renameFilePathName);

                    var originalStagePath = Path.Combine(config.StagePath, convert.OriginalFile.FilePath);
                    if (Directory.GetFiles(originalStagePath).Length == 0 &&
                        Directory.GetDirectories(originalStagePath).Length == 0)
                    {
                        stage = "Delete Original Directory";
                        Directory.Delete(originalStagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(stage + ": " + ex.Message);
                convert.Errored = true;
                convert.Output = stage + ": " + ex.Message;
            }
            finally
            {
                convert.EndedDate = DateTime.Now;
                lock (convertFileService)
                {
                    if (convert.Errored)
                        convertFileService.UpdateConvertAsync(convert).Wait();
                    else
                        convertFileService.DeleteConvertAsync(convert).Wait();
                }
            }
        }

        private class ConvertTask
        {
            public Task Task { get; set; }
            public ConvertFileDto ConvertFile { get; set; }
        }
    }

}
