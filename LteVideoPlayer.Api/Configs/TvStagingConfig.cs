namespace LteVideoPlayer.Api.Configs
{
    public class TvStagingConfig : IConfig, IVideoConfig
    {
        public string FriendlyName { get; set; } = "Tv Staging";
        public string RootDir { get; set; } = "";
        public string VideosSubDir { get; set; } = "";
        public string? ThumbnailSubDir { get; set; }
        public string? ConvertToConfigName { get; set; } = nameof(TvConfig);
        public int? ThumbnailMinSeekPercent { get; set; }
        public int? ThumbnailMaxSeekPercent { get; set; }
        public int? RetryThumbnailAfterDays { get; set; }
        public int? MaxThumbnailRetrys { get; set; }
        public bool CanPlayVideo { get; set; } = false;
        public bool CanConvertVideo { get; set; } = true;
        public bool CanThumbnailVideo { get; set; } = false;
        public bool AdminViewOnly { get; set; } = true;
        public IVideoConfig? ConvertToConfig { get; set; }
    }
}
