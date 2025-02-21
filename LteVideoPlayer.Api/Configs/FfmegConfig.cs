namespace LteVideoPlayer.Api.Configs
{
    public class FfmegConfig : IConfig
    {
        public string RootDir { get; set; } = "";
        public string FfmpegFile { get; set; } = "";
        public string FfprobeFile { get; set; } = "";
        public int FfmpegThreads { get; set; }

        public string RootFfmpegFile { get => Path.Combine(RootDir, FfmpegFile); }
        public string RootFfprobeFile { get => Path.Combine(RootDir, FfprobeFile); }
    }
}
