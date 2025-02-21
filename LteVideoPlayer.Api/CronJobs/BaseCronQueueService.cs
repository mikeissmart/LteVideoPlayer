using Cronos;
using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Models.Entities;
using LteVideoPlayer.Api.Models.Entities.Logging;
using LteVideoPlayer.Api.Persistance.Repositories.Logging;
using Microsoft.Extensions.Hosting.Internal;

namespace LteVideoPlayer.Api.CronJobs
{
    public abstract class BaseCronQueueService
    {
        private readonly BaseCronQueueConfig _config;
        private readonly CancellationToken _cancellationToken;
        private readonly IServiceProvider _services;
        private bool _isRunning;
        private bool _pendingStop;

        public BaseCronQueueService(
            IHostApplicationLifetime applicationLifetime,
            IServiceProvider services,
            BaseCronQueueConfig config)
        {
            _cancellationToken = applicationLifetime.ApplicationStopping;
            _services = services;
            _config = config;
        }

        public void AutoStartQueue()
        {
            if (_config.AutoRun)
                StartQueue();
        }

        public bool StartQueue()
        {
            if (!_isRunning)
            {
                Task.Run(async () => await RunQueueAsync());
                return true;
            }

            return false;
        }

        public bool StopQueue()
        {
            if (!_pendingStop)
            {
                _pendingStop = true;
                return true;
            }

            return false;
        }

        public bool IsRunning() => _isRunning;

        public bool IsPendingStop() => _pendingStop;

        protected abstract Task<string?> DoQueueAsync(IServiceScope scope, CancellationToken cancellationToken);

        private async Task RunQueueAsync()
        {
            _isRunning = true;
            _pendingStop = false;
            while (!_pendingStop || _cancellationToken.IsCancellationRequested)
            {
                var log = new CronLog()
                {
                    Name = GetType().Name,
                    StartDateTime = DateTime.Now,
                };
                using (var scope = _services.CreateScope())
                {
                    string? result = null;
                    try
                    {
                        result = await DoQueueAsync(scope, _cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        // Consume error
                        ;
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(result) && !string.IsNullOrWhiteSpace(result))
                        {
                            log.Message = result;
                            log.EndDateTime = DateTime.Now;
                            log.IsCanceled = _cancellationToken.IsCancellationRequested;

                            var cronLogRepository = scope.ServiceProvider.GetRequiredService<ICronLogRepository>();
                            await cronLogRepository.AddAsync(log);
                            await cronLogRepository.SaveChangesAsync();
                        }
                    }
                }

                for (var i = 0; i < _config.MinutesBetweenRuns; i++)
                {
                    Thread.Sleep(60000);
                    if (_pendingStop || _cancellationToken.IsCancellationRequested)
                        break;
                }
            }
            _isRunning = false;
        }
    }
}
