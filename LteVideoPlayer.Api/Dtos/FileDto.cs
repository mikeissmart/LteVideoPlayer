using LteVideoPlayer.Api.DataTypes;
using LteVideoPlayer.Api.Entities;

namespace LteVideoPlayer.Api.Dtos
{
    public class FileDto : IRefactorType, IMapTo<FileEntity>, IMapFrom<FileEntity>
    {
        public string FilePath { get; set; } = "";
        public string FileName { get; set; } = "";
        public bool FileExists { get; set; }
        public bool? ConvertQueued { get; set; }
        public string FilePathName
        {
            get => Path.Combine(FilePath, FileName);
        }
    }
}
