using LteVideoPlayer.Api.Entities;

namespace LteVideoPlayer.Api.Dtos
{
    public class ThumbnailErrorDto : IRefactorType, IMapTo<ThumbnailError>, IMapFrom<ThumbnailError>
    {
        public int TimesFailed { get; set; }
        public string Error { get; set; }
        public FileDto File { get; set; } = new FileDto();
        public DateTime LastError { get; set; } = DateTime.Now;
    }
}
