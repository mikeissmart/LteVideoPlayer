namespace LteVideoPlayer.Api.Configs
{
    public class VideoConfig : IConfig
    {
        public string RootPath { get; set; }
        public string StagePath { get; set; }
        public string FfmpegFile { get; set; }
        public int RemoveConvertingFileDays { get; set; }
    }
}
