using LteVideoPlayer.Api.DataTypes;
using LteVideoPlayer.Api.Entities;

namespace LteVideoPlayer.Api.Dtos
{
    public class ConvertFileDto : IRefactorType, IMapFrom<ConvertFile>, IMapTo<ConvertFile>
    {
        public int Id { get; set; }
        public string? Output { get; set; }
        public bool Errored { get; set; }
        public FileDto OriginalFile { get; set; } = new FileDto();
        public FileDto ConvertedFile { get; set; } = new FileDto();
        public int AudioStream { get; set; } = 0;
        public DateTime CreatedDate { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? EndedDate { get; set; }
    }
}
