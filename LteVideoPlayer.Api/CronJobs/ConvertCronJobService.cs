
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

            var runThumbnailCronJobService = false;
            var convertFiles = await _convertFileService.GetAllIncompleteConvertFilesAsync();
            foreach (var file in convertFiles)
            {
                runThumbnailCronJobService = true;
                await ConvertFilesAsync(file, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
            }

            if (runThumbnailCronJobService)
                scope.ServiceProvider.GetRequiredService<ThumbnailCronJobService>().DoWorkNow();

            return null;
        }

        private async Task ConvertFilesAsync(ConvertFileDto convertFile, CancellationToken cancellationToken)
        {
            var videoConfig = _videoConfigService.GetVideoConfig(convertFile.DirectoryEnum);
            if (!videoConfig.CanConvertVideo)
                return;

            _convertFile = convertFile;
            var stage = "";
            try
            {
                var videoRootDirFullPath = Path.Combine(videoConfig.RootVideoDir, convertFile.OriginalFile.FullPath);
                var activeRootDirFullPath = Path.Combine(videoConfig.RootVideoDir, convertFile.OriginalFile.Path, "Converting_" + convertFile.ConvertedFile.File);

                convertFile.StartedDate = DateTime.Now;
                await _convertFileService.UpdateConvertAsync(convertFile);

                var threadStr = _fmegConfig.FfmpegThreads > 0
                    ? $"-threads {_fmegConfig.FfmpegThreads}"
                    : "";

                stage = "Starting convert";
                var result = await ProcessHelper.RunProcessAsync(
                    _fmegConfig.RootFfmpegFile,
                    $@"-i ""{videoRootDirFullPath}"" {threadStr} -map 0:v:0 -map 0:a:{convertFile.AudioStreamNumber - 1} -c:v libx264 -crf 23 -profile:v baseline -level 3.0 -pix_fmt yuv420p -c:a aac -ac 2 -b:a 128k -y ""{activeRootDirFullPath}""",
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
                    var convertRootDirFullPath = Path.Combine(videoConfig.ConvertRootFullPath, convertFile.ConvertedFile.Path);
                    if (!Directory.Exists(convertRootDirFullPath))
                        Directory.CreateDirectory(convertRootDirFullPath);

                    stage = "Move Converted File";
                    File.Move(
                        activeRootDirFullPath,
                        Path.Combine(videoConfig.ConvertRootFullPath, convertFile.ConvertedFile.FullPath),
                        true);

                    stage = "Delete Original File";
                    //File.Delete(videoRootDirFullPath);

                    var checkDeletePath = Path.Combine(videoConfig.RootVideoDir, convertFile.OriginalFile.Path);
                    if (Directory.GetFiles(checkDeletePath).Length == 0
                        && Directory.GetDirectories(checkDeletePath).Length == 0)
                    {
                        stage = "Delete Original Directory";
                        Directory.Delete(checkDeletePath);
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
                if (convertFile.Errored)
                    await _convertFileService.UpdateConvertAsync(convertFile);
                else
                    await _convertFileService.DeleteConvertAsync(convertFile);
            }
        }
    }
}
