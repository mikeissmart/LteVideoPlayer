namespace LteVideoPlayer.Api.Configs
{
    public class TvConfig : IConfig, IVideoConfig
    {
        public string FriendlyName { get; set; } = "Tv";
        public string RootDir { get; set; } = "";
        public string VideosSubDir { get; set; } = "";
        public string? ThumbnailSubDir { get; set; }
        public string? ConvertToConfigName { get; set; }
        public int? ThumbnailMinSeekPercent { get; set; } = 10;
        public int? ThumbnailMaxSeekPercent { get; set; } = 90;
        public int? RetryThumbnailAfterDays { get; set; } = 1;
        public int? MaxThumbnailRetrys { get; set; } = 10;
        public bool CanPlayVideo { get; set; } = true;
        public bool CanConvertVideo { get; set; } = false;
        public bool CanThumbnailVideo { get; set; } = true;
        public bool AdminViewOnly { get; set; } = false;
        public IVideoConfig? ConvertToConfig { get; set; }
    }
}
