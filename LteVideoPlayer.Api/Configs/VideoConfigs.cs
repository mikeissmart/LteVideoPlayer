using LteVideoPlayer.Api.Models.Enums;

namespace LteVideoPlayer.Api.Configs
{
    public class VideoConfigs : IConfig
    {
        public List<VideoConfig> Configs { get; set; } = new List<VideoConfig>();
    }

    public class VideoConfig
    {
        public string FriendlyName { get; set; } = "";
        public int DirEnumValue { get; set; }
        public string RootDir { get; set; } = "";
        public string VideosSubDir { get; set; } = "";
        public string ThumbnailSubDir { get; set; } = "";
        public string ConvertRootFullPath { get; set; } = "";
        public int ThumbnailMinSeekPercent { get; set; }
        public int ThumbnailMaxSeekPercent { get; set; }
        public int RetryThumbnailAfterDays { get; set; }
        public int MaxThumbnailRetrys { get; set; }
        public bool CanPlayVideo { get; set; }
        public bool CanConvertVideo { get; set; }
        public bool CanThumbnailVideo { get; set; }
        public bool AdminViewOnly { get; set; }

        public string RootVideoDir { get => Path.Combine(RootDir, VideosSubDir); }
        public string RootThumbnailDir { get => Path.Combine(RootDir, ThumbnailSubDir); }
        public DirectoryEnum DirectoryEnum { get => (DirectoryEnum)DirEnumValue; }
    }
}
