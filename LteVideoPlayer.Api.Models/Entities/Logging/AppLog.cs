using LteVideoPlayer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Entities.Logging
{
    public class AppLog
    {
        public int Id { get; set; }
        public LogLevelEnum LogLevel { get; set; }
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public string? Source { get; set; }
        public string? StackTrace { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
