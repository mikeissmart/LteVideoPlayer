using LteVideoPlayer.Api.Models.Entities.Logging;
using LteVideoPlayer.Api.Models.Enums;

namespace LteVideoPlayer.Api.Logging
{
    public class DbLogger : ILogger
    {
        private readonly DbLoggerProcessor _processor;

        public DbLogger(DbLoggerProcessor processor) => _processor = processor;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var log = new AppLog
            {
                LogLevel = (LogLevelEnum)logLevel,
                EventId = eventId.Id,
                EventName = eventId.Name,
                Message = formatter.Invoke(state, exception),
                CreatedDateTime = DateTime.Now
            };

            if (exception != null)
            {
                log.Source = exception.Source;
                log.StackTrace = exception.StackTrace;
            }

            _processor.Enqueue(log);
        }
    }
}
