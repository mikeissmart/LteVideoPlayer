namespace LteVideoPlayer.Api.Configs
{
    public class VideoConfig : IConfig
    {
        public string RootPath { get; set; }
        public string VideoName { get; set; }
        public string StageName { get; set; }
        public string FfmpegName { get; set; }
        public int FfmpegThreads { get; set; }

        public string VideoPath
        {
            get
            {
                return RootPath + VideoName;
            }
        }
        public string StagePath
        {
            get
            {
                return RootPath + StageName;
            }
        }
        public string FfmpegFile
        {
            get
            {
                return RootPath + FfmpegName;
            }
        }
        public int RemoveConvertingFileDays { get; set; }
    }
}
