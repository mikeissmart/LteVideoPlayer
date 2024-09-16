using LteVideoPlayer.Api.DataTypes;

namespace LteVideoPlayer.Api.Entities
{
    public class ThumbnailError
    {
        public int Id { get; set; }
        public int TimesFailed { get; set; }
        public string Error { get; set; }
        public FileEntity File { get; set; } = new FileEntity();
        public DateTime LastError { get; set; } = DateTime.Now;
    }
}
