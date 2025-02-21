using LteVideoPlayer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos.Logging
{
    public class AppLogFilterDto : IRefactorType
    {
        public LogLevelEnum? LogLevel { get; set; }
        public int? EventId { get; set; }
        public string? EventName { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
    }
}
