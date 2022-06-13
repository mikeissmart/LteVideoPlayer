namespace LteVideoPlayer.Api.Dtos
{
    public class DirDto : IRefactorType
    {
        public string DirPath { get; set; } = "";
        public string DirName { get; set; } = "";
        public string DirPathName
        {
            get => Path.Combine(DirPath, DirName);
        }
    }
}
