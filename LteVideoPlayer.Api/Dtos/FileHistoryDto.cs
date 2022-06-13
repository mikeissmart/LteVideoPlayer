using LteVideoPlayer.Api.DataTypes;
using LteVideoPlayer.Api.Entities;

namespace LteVideoPlayer.Api.Dtos
{
    public class FileHistoryDto : IRefactorType, IMapFrom<FileHistory>, IMapTo<FileHistory>
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public float PercentWatched { get; set; }
        public FileDto FileEntity { get; set; } = new FileDto();
        public DateTime StartedDate { get; set; }
    }
}
