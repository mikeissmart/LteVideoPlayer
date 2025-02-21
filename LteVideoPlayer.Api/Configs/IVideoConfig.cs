namespace LteVideoPlayer.Api.Configs
{
    public interface IVideoConfig
    {
        string FriendlyName { get; set; }
        string RootDir { get; set; }
        string VideosSubDir { get; set; }
        string? ThumbnailSubDir { get; set; }
        string? ConvertToConfigName { get; set; }
        int? ThumbnailMinSeekPercent { get; set; }
        int? ThumbnailMaxSeekPercent { get; set; }
        int? RetryThumbnailAfterDays { get; set; }
        int? MaxThumbnailRetrys { get; set; }
        bool CanPlayVideo { get; set; }
        bool CanConvertVideo { get; set; }
        bool CanThumbnailVideo { get; set; }
        bool AdminViewOnly { get; set; }
        IVideoConfig? ConvertToConfig { get; set; }

        string RootVideoDir { get => Path.Combine(RootDir, VideosSubDir); }
        string? RootThumbnailDir { get => ThumbnailSubDir == null
                ? throw new ArgumentNullException(nameof(ThumbnailSubDir))
                : Path.Combine(RootDir, ThumbnailSubDir); }
    }
}
