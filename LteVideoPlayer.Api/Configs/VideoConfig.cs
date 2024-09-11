namespace LteVideoPlayer.Api.Configs
{
    public class VideoConfig : IConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RootPath { get; set; }
        public string VideoName { get; set; }
        public string StageName { get; set; }
        public string ThumbnailName { get; set; }
        public string FfmpegName { get; set; }
        public int FfmpegThreads { get; set; }
        public string FfprobeName { get; set; }
        public int ThumbnailMinPercent { get; set; }
        public int ThumbnailMaxPercent { get; set; }
        public string DefaultThumbnail { get; set; }

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
        public string ThumbnailPath
        {
            get
            {
                return RootPath + ThumbnailName;
            }
        }
        public string FfmpegFile
        {
            get
            {
                return RootPath + FfmpegName;
            }
        }
        public string FfprobeFile
        {
            get
            {
                return RootPath + FfprobeName;
            }
        }
        public string DefaultThumbnailFile
        {
            get
            {
                return ThumbnailPath + DefaultThumbnail;
            }
        }
    }
}
