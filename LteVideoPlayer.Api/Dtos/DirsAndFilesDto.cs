namespace LteVideoPlayer.Api.Dtos
{
    public class DirsAndFilesDto : IRefactorType
    {
        public List<DirDto> Dirs { get; set; }
        public List<FileDto> Files { get; set; }
    }
}
