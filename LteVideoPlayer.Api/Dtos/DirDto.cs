namespace LteVideoPlayer.Api.Dtos
{
    public class DirDto : IRefactorType
    {
        public string Path { get; set; } = "";
        public string Name { get; set; } = "";
        public List<DirDto> SubDirs { get; set; } = new List<DirDto>();
        public List<string> Videos { get; set; } = new List<string>();
    }
}
