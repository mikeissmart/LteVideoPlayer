using LteVideoPlayer.Api.Models.Enums.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Models.Entities.Logging
{
    public class CronLog
    {
        public int Id { get; set; }
        [StringLength(50)]
        public required string Name { get; set; }
        public CronLogTypeEnum CronLogType { get; set; }
        public string? Message { get; set; }
        public bool IsCanceled { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}
