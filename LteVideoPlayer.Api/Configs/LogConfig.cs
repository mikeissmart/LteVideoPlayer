namespace LteVideoPlayer.Api.Configs
{
    public class LogConfig : IConfig
    {
        public int AppLogKeepDays { get; set; } = 90;
        public int CronLogKeepDays { get; set; } = 90;
    }
}
