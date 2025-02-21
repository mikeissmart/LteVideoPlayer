namespace LteVideoPlayer.Api.Configs
{
    public class CronJobConfig : IConfig
    {
        public string ConvertCronJob { get; set; } = "";
        public string ThumbnailCronJob { get; set; } = "";
    }
}
