namespace LteVideoPlayer.Api.Logging
{
    public class DbLoggerProvider : ILoggerProvider
    {
        private readonly DbLoggerProcessor _processor;

        public DbLoggerProvider(string connectionString) => _processor = new DbLoggerProcessor(connectionString);

        public ILogger CreateLogger(string categoryName) => new DbLogger(_processor);

        public void Dispose() => _processor.Dispose();
    }
}
