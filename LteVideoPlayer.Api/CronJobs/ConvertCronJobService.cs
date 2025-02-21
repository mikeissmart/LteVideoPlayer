
using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Helpers;
using LteVideoPlayer.Api.Models.Dtos;
using LteVideoPlayer.Api.Models.Enums;
using LteVideoPlayer.Api.Services;
using Microsoft.Extensions.Logging;
using System;

namespace LteVideoPlayer.Api.CronJobs
{
    public class ConvertCronJobService : BaseCronJobService
    {
        private readonly FfmegConfig _fmegConfig;
        private ILogger<ConvertCronJobService> _logger;
        private IVideoConfigService _videoConfigService;
        private IDirectoryService _directoryService;
        private IConvertFileService _convertFileService;
        private ConvertFileDto? _convertFile;

        public ConvertCronJobService(FfmegConfig fmegConfig, CronJobConfig config, IServiceProvider services)
            : base(config, services, nameof(CronJobConfig.ConvertCronJob))
        {
            _fmegConfig = fmegConfig;
        }

        public ConvertFileDto? CurrentConvertFile() => _convertFile;

        protected override async Task<string?> DoWorkAsync(IServiceScope scope, CancellationToken cancellationToken)
        {
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<ConvertCronJobService>>();
            _videoConfigService = scope.ServiceProvider.GetRequiredService<IVideoConfigService>();
            _directoryService = scope.ServiceProvider.GetRequiredService<IDirectoryService>();
            _convertFileService = scope.ServiceProvider.GetRequiredService<IConvertFileService>();

            foreach (DirectoryEnum dirEnum in Enum.GetValues(typeof(DirectoryEnum)))
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanConvertVideo)
                    continue;

                if (!Directory.Exists(videoConfig.RootVideoDir))
                    Directory.CreateDirectory(videoConfig.RootVideoDir);
                if (!Directory.Exists(videoConfig.ConvertToConfig!.RootVideoDir))
                    Directory.CreateDirectory(videoConfig.ConvertToConfig!.RootVideoDir);

                await ConvertFilesAsync(videoConfig, dirEnum, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
            }

            return null;
        }

        private async Task ConvertFilesAsync(IVideoConfig videoConfig, DirectoryEnum dirEnum, CancellationToken cancellationToken)
        {
            var convertFiles = await _convertFileService.GetAllIncompleteConvertFilesAsync(dirEnum);
            foreach (var convertFile in convertFiles)
            {
                _convertFile = convertFile;
                if (cancellationToken.IsCancellationRequested)
                    break;

                var stage = "";
                try
                {
                    var renameFileName = convertFile.OriginalFile.File + "_converting";
                    var renameRootDirFullPath = Path.Combine(videoConfig.RootVideoDir, convertFile.OriginalFile.Path, renameFileName);

                    var activeFileName = "Converting_" + convertFile.ConvertedFile.File;
                    var activeRootDirFullPath = Path.Combine(videoConfig.RootVideoDir, convertFile.OriginalFile.Path, activeFileName);

                    stage = "Renaming original file";
                    if (!File.Exists(renameRootDirFullPath))
                    {
                        File.Move(
                            Path.Combine(videoConfig.RootVideoDir, convertFile.OriginalFile.FullPath),
                            renameRootDirFullPath,
                            true);
                    }

                    convertFile.StartedDate = DateTime.Now;
                    await _convertFileService.UpdateConvertAsync(convertFile);

                    var threadStr = _fmegConfig.FfmpegThreads > 0
                        ? $"-threads {_fmegConfig.FfmpegThreads}"
                        : "";

                    var result = await ProcessHelper.RunProcessAsync(
                        _fmegConfig.RootFfmpegFile,
                        $@"-i ""{renameRootDirFullPath}"" {threadStr} -map 0:v:0 -map 0:a:{convertFile.AudioStreamIndex} -c:v libx264 -crf 23 -profile:v baseline -level 3.0 -pix_fmt yuv420p -c:a aac -ac 2 -b:a 128k -y ""{activeRootDirFullPath}""",
                        cancellationToken);

                    stage = "Check if convert sucessfull";
                    if (!File.Exists(activeRootDirFullPath))
                    {
                        convertFile.Errored = true;
                        convertFile.Output = result.Error;
                    }
                    else
                    {
                        stage = "Create Convert directory";
                        var convertRootDirFullPath = Path.Combine(videoConfig.ConvertToConfig!.RootVideoDir, convertFile.ConvertedFile.Path);
                        if (!Directory.Exists(convertRootDirFullPath))
                            Directory.CreateDirectory(convertRootDirFullPath);

                        stage = "Move Converted File";
                        File.Move(
                            activeRootDirFullPath,
                            Path.Combine(videoConfig.ConvertToConfig!.RootVideoDir, convertFile.ConvertedFile.FullPath),
                            true);

                        stage = "Delete Original File";
                        //File.Delete(renameRootDirFullPath);

                        if (Directory.GetFiles(Path.Combine(videoConfig.RootVideoDir, convertFile.OriginalFile.FullPath)).Length == 0
                            || Directory.GetDirectories(Path.Combine(videoConfig.RootVideoDir, convertFile.OriginalFile.FullPath)).Length == 0)
                        {
                            stage = "Delete Original Directory";
                            Directory.Delete(convertFile.OriginalFile.Path);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(stage + ": " + ex.Message);
                    convertFile.Errored = true;
                    convertFile.Output = stage + ": " + ex.Message;
                }
                finally
                {
                    convertFile.EndedDate = DateTime.Now;
                    lock (_convertFileService)
                    {
                        if (convertFile.Errored)
                            _convertFileService.UpdateConvertAsync(convertFile).Wait();
                        else
                            _convertFileService.DeleteConvertAsync(convertFile).Wait();
                    }
                }
            }
        }

        private async Task ProcessConvertAsync()
        {
        }
    }
}
