using LteVideoPlayer.Api.Models.Enums.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Dtos.Logging
{
    public class CronLogFilterDto : IRefactorType
    {
        public string? Name { get; set; }
        public CronLogTypeEnum? CronLogType { get; set; }
        public bool? IsCanceled { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
    }
}
