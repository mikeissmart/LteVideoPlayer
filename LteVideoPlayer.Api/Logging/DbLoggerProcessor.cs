using LteVideoPlayer.Api.Models.Entities.Logging;
using LteVideoPlayer.Api.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace LteVideoPlayer.Api.Logging
{
    public class DbLoggerProcessor : IDisposable
    {
        private const int MAXQUEUESIZE = 1024;

        private readonly LogDbContext _logDbContext;
        private readonly BlockingCollection<AppLog> _queue;
        private readonly Thread _thread;

        public DbLoggerProcessor(string connectionString)
        {
            var optionBuilder = new DbContextOptionsBuilder<LogDbContext>();
            optionBuilder.UseSqlServer(connectionString);
            _logDbContext = new LogDbContext(optionBuilder.Options);

            _thread = new Thread(ProcessQueue)
            {
                IsBackground = true,
                Name = "DbLoggerProcessor queue thread"
            };
            _thread.Start();
            _queue = new BlockingCollection<AppLog>(MAXQUEUESIZE);
        }

        public void Enqueue(AppLog log)
        {
            if (!_queue.IsAddingCompleted)
            {
                try
                {
                    _queue.Add(log);
                }
                catch (InvalidOperationException) { }
            }
            else
            {
                _logDbContext.AppLogs.Add(log);
                _logDbContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            _queue.CompleteAdding();

            try
            {
                _thread.Join(1500);
            }
            catch (ThreadStateException) { }
        }

        private void ProcessQueue()
        {
            try
            {
                foreach (var log in _queue.GetConsumingEnumerable())
                {
                    _logDbContext.AppLogs.Add(log);
                    _logDbContext.SaveChanges();
                }
            }
            catch
            {
                try
                {
                    _queue.CompleteAdding();
                }
                catch { }
            }
        }
    }
}
