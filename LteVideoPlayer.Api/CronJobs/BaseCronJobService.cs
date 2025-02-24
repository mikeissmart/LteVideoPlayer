using Cronos;
using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Models.Entities.Logging;
using LteVideoPlayer.Api.Persistance.Repositories.Logging;
using Microsoft.Extensions.Hosting.Internal;
using System.Threading;

namespace LteVideoPlayer.Api.CronJobs
{
    public abstract class BaseCronJobService : IHostedService, IDisposable
    {
        private System.Timers.Timer? _timer;
        private DateTime? _lastRun;
        private readonly IServiceProvider _services;
        private readonly CronExpression _cronExpression;
        private readonly TimeZoneInfo _timeZoneInfo;
        private bool _isRunning;

        public BaseCronJobService(CronJobConfig cronConfig, IServiceProvider services,
            string expressionName)
        {
            _services = services;
            _cronExpression = CronExpression.Parse(typeof(CronJobConfig).GetProperty(expressionName)!.GetValue(cronConfig)!.ToString());
            _timeZoneInfo = TimeZoneInfo.Local;
        }
        public virtual async Task StartAsync(CancellationToken cancellationToken) => await ScheduleJob(cancellationToken);

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }

        public void DoWorkNow()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Interval = 0.1;
                _timer.Start();
            }
        }

        public DateTimeOffset? NextOccurrence() => _cronExpression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);

        public DateTime? LastOccurance() => _lastRun;

        public string GetCronExpression() => _cronExpression.ToString();

        public bool IsRunning() => _isRunning;

        public virtual void Dispose() => _timer?.Dispose();

        protected abstract Task<string?> DoWorkAsync(IServiceScope scope, CancellationToken cancellationToken);

        private async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = NextOccurrence();
            if (next != null)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
                    await ScheduleJob(cancellationToken);

                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();   // reset and dispose timer
                    _timer = null;
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        var log = new CronLog()
                        {
                            Name = GetType().Name,
                            StartDateTime = DateTime.Now,
                        };
                        using (var scope = _services.CreateScope())
                        {
                            try
                            {
                                _isRunning = true;
                                log.Message = await DoWorkAsync(scope, cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                var logType = typeof(ILogger<>).MakeGenericType(GetType());
                                var logger = (ILogger)scope.ServiceProvider.GetRequiredService(logType);
                                logger.LogError(ex, ex.Message);
                                ;
                            }
                            finally
                            {
                                log.EndDateTime = DateTime.Now;
                                log.IsCanceled = cancellationToken.IsCancellationRequested;

                                var cronLogRepository = scope.ServiceProvider.GetRequiredService<ICronLogRepository>();
                                await cronLogRepository.AddAsync(log);
                                await cronLogRepository.SaveChangesAsync();

                                _isRunning = false;
                            }
                        }
                    }
                    if (!cancellationToken.IsCancellationRequested)
                        await ScheduleJob(cancellationToken);   // reschedule next

                };
                _timer.Start();
            }
            else
                await Task.CompletedTask;
        }
    }
}
