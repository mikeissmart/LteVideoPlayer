using LteVideoPlayer.Api.Entities;

namespace LteVideoPlayer.Api.Dtos
{
    public class CreateConvertDto : IRefactorType, IMapTo<ConvertFile>
    {
        public FileDto OriginalFile { get; set; } = new FileDto();
        public FileDto ConvertedFile { get; set; } = new FileDto();
        public int AudioStream { get; set; }
    }
}
