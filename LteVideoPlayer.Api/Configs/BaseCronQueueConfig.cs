using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteVideoPlayer.Api.Configs
{
    public abstract class BaseCronQueueConfig
    {
        public bool AutoRun { get; set; } = true;
        public int MinutesBetweenRuns { get; set; } = 1;
    }
}
