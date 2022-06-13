using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Service;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace LteVideoPlayer.Api.CronJob.Convert
{
    public class ConvertQueueCronJob
    {
        private readonly CancellationToken _cancellationToken;
        private readonly VideoConfig _videoConfig;
        private readonly int _concurrentConverts;
        private readonly IServiceProvider _services;

        public ConvertQueueCronJob(
            IHostApplicationLifetime applicationLifetime,
            VideoConfig videoConfig,
            IServiceProvider services)
        {
            _cancellationToken = applicationLifetime.ApplicationStopping;
            _videoConfig = videoConfig;
            _services = services;
            _concurrentConverts = 5;
        }

        public void StartQueue()
        {
            Task.Run(() => WatchQueue());
        }

        private void WatchQueue()
        {
            using (var scope = _services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ConvertQueueCronJob>>();
                var convertFileService = scope.ServiceProvider.GetRequiredService<IConvertFileService>();
                var directoryService = scope.ServiceProvider.GetRequiredService<IDirectoryService>();

                var runningTasks = new List<Task>();
                var queuedConverts = new List<ConvertFileDto>();
                List<ConvertFileDto> nonQueuedConverts;
                while (!_cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        lock (convertFileService)
                        {
                            var task = convertFileService.GetIncompleteConvertsAsync(_concurrentConverts * 2);
                            task.Wait();
                            nonQueuedConverts = task.Result;
                        }
                        nonQueuedConverts = nonQueuedConverts
                                .Where(x => queuedConverts.Where(y => x.Id == y.Id).Count() == 0)
                                .ToList();
                        if (nonQueuedConverts.Count == 0 && runningTasks.Count == 0)
                            Task.Delay(6000, _cancellationToken).Wait(_cancellationToken);
                        else
                        {
                            while (!_cancellationToken.IsCancellationRequested &&
                                runningTasks.Count != _concurrentConverts &&
                                nonQueuedConverts.Count > 0)
                            {
                                var convert = nonQueuedConverts[0];
                                nonQueuedConverts.RemoveAt(0);
                                queuedConverts.Add(convert);

                                runningTasks.Add(Task.Run(() => ProcessConvert(
                                    convertFileService,
                                    logger,
                                    convert,
                                    _videoConfig
                                    )));
                            }

                        }

                        for (var i = 0; i < runningTasks.Count; i++)
                        {
                            var task = runningTasks[i];
                            if (task.IsCompleted)
                            {
                                queuedConverts.RemoveAt(i);
                                runningTasks.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }
                }

                #region a
                /*// Prepopulate
                foreach (var item in await convertVideoFileService.GetAllIncompleteConvertsAsync())
                    _queue.Enqueue(item);

                var runningTasks = new List<Task>();
                var runningConvertFiles = new List<ConvertFileDto>();
                while (!_cancellationToken.IsCancellationRequested)
                {
                    while (_queue.Count > 0 && runningTasks.Count < _concurrentConverts)
                    {
                        if (_queue.TryDequeue(out var convertFile))
                        {
                            convertFile = await convertVideoFileService.GetConvertsByIdAsync(convertFile.Id);
                            var videoFile = await videoFileService.GetVideoFilesByIdAsync(convertFile!.VideoFileId);
                            if (convertFile != null)
                            {
                                ProcessConvert(logger,
                                    convertVideoFileService,
                                    videoFileService,
                                    convertFile,
                                    videoFile,
                                    _videoConfig);
                                /*runningConvertFiles.Add(convertFile);
                                runningTasks.Add(Task.Run(() => ProcessConvert(
                                    logger,
                                    convertVideoFileService,
                                    videoFileService,
                                    convertFile,
                                    videoFile,
                                    _videoConfig)));* /
                            }
                        }
                    }

                    /*for (var i = 0; i < runningTasks.Count; i++)
                    {
                        var task = runningTasks[i];
                        if (task.IsCompleted)
                        {
                            runningConvertFiles.RemoveAt(i);
                            runningTasks.RemoveAt(i);
                            i--;
                        }
                    }* /
                }*/
                #endregion
            }
        }

        /*private async Task<List<FileDto>> GetAllFilesAsync(DirDto dir, IDirectoryService directoryService)
        {
            var files = await directoryService.GetFilesAsync(dir.DirPathName, true);
            foreach (var dirDto in await directoryService.GetDirsAsync(dir.DirPathName, true))
                files.AddRange(await GetAllFilesAsync(dirDto, directoryService));

            return files;
        }*/

        private static void ProcessConvert(
            IConvertFileService convertFileService,
            ILogger<ConvertQueueCronJob> logger,
            ConvertFileDto convert,
            VideoConfig config)
        {
            try
            {
                var renameFileName = Path.GetExtension(convert.OriginalFile.FileName).Contains("_converting")
                    ? convert.OriginalFile.FileName
                    : convert.OriginalFile.FileName + "_converting";
                var renameFilePathName = Path.Combine(convert.OriginalFile.FilePath, renameFileName);
                var convertedFileName = "Converting_" + convert.ConvertedFile.FileName;
                var convertedFilePathName = Path.Combine(convert.OriginalFile.FilePath, convertedFileName);

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

                var startInfo = new ProcessStartInfo
                {
                    FileName = config.FfmpegFile,
                    Arguments = $@"-i ""{config.StagePath + renameFilePathName}"" -vcodec libx264 -acodec aac -y ""{config.StagePath + convertedFilePathName}""",
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                };

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
                    process.WaitForExit();
                }
                convert.Output = output.ToString();

                if (File.Exists(config.StagePath + convertedFilePathName))
                {
                    // Convert successful
                    var pathParts = convert.ConvertedFile.FilePath.Split("\\");
                    var checkPath = config.RootPath;
                    for (var i = 0; i < pathParts.Length; i++)
                    {
                        checkPath = Path.Combine(checkPath, pathParts[i]);
                        if (!Directory.Exists(checkPath))
                            Directory.CreateDirectory(checkPath);
                    }
                    File.Move(
                        config.StagePath + convertedFilePathName,
                        config.RootPath + convert.ConvertedFile.FilePathName,
                        true);
                    File.Delete(config.StagePath + renameFilePathName);

                    var originalStagePath = Path.Combine(config.StagePath, convert.OriginalFile.FilePath);
                    if (Directory.GetFiles(originalStagePath).Length == 0 &&
                        Directory.GetDirectories(originalStagePath).Length == 0)
                    {
                        Directory.Delete(originalStagePath);
                    }
                }
                else
                {
                    // Convert errored
                    convert.Errored = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                convert.Errored = true;
                convert.Output = ex.Message;
            }
            finally
            {
                convert.EndedDate = DateTime.Now;
                lock (convertFileService)
                {
                    convertFileService.UpdateConvertAsync(convert).Wait();
                }
            }
        }

        /*private static void ProcessConvert(ILogger<ConvertQueueCronJob> logger,
            IConvertVideoFileService convertVideoFileService,
            IVideoFileService videoFileService,
            ConvertFileDto convertFile,
            FileDto? videoFile,
            VideoConfig config)
        {
            try
            {
                if (videoFile == null)
                    throw new ArgumentException("VideoFile not found");
                var videoPathName = videoFile.VideoPathName;
                var renameVideoName = Path.GetExtension(videoFile.VideoName).Contains("_converting")
                    ? videoFile.VideoName
                    : videoFile.VideoName + "_converting";
                var renameVideoPathName = Path.Combine(videoFile.VideoPath, renameVideoName);
                var convertVideoPathName = videoPathName.Remove(videoPathName.Length - Path.GetExtension(videoPathName).Length);
                convertVideoPathName += ".mp4";

                File.Move(config.RootPath + videoPathName, config.RootPath + renameVideoPathName);
                videoFile.VideoName = renameVideoName;
                videoFile.IsConverting = true;
                videoFileService.UpdateVideoFileAsync(videoFile).Wait();

                var startInfo = new ProcessStartInfo
                {
                    FileName = config.FfmpegFile,
                    Arguments = $@"-i ""{config.RootPath + renameVideoPathName}"" -vcodec libx264 -acodec aac -y ""{config.RootPath + convertVideoPathName}""",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                };
                /*
                 When the file is not convertable no .mp4 is created
                 When the file is converable .mp4 file is create
                 * /
                var output = new StringBuilder();
                var error = new StringBuilder();
                using (var process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.OutputDataReceived += (s, e) =>
                    {
                        lock (output)
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (s, e) =>
                    {
                        lock (error)
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                }
                convertFile.Output = output.ToString();
                convertFile.Error = error.ToString();

                /*                 
                var output = new StringBuilder();
                using (var exeProcess = Process.Start(startInfo)!)
                {
                    convertFile.Output = exeProcess.StandardOutput.ReadToEnd();
                    convertFile.Error = exeProcess.StandardError.ReadToEnd();
                    exeProcess.WaitForExit();
                }
                File.Delete(config.RootPath + renameVideoPathName);
                 * /

                //File.Delete(config.RootPath + renameVideoPathName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                convertFile.Error = ex.Message;
            }
            finally
            {
                if (videoFile != null)
                {
                    videoFile.IsConverting = false;
                    videoFileService.UpdateVideoFileAsync(videoFile).Wait();
                }
                convertFile.EndedDate = DateTime.Now;
                convertVideoFileService.UpdateConvertAsync(convertFile).Wait();
            }
        }*/
    }
}
