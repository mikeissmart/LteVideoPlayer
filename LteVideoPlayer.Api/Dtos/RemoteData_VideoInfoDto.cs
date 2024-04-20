namespace LteVideoPlayer.Api.Dtos
{
    public class RemoteData_VideoInfoDto : RemoteData, IRefactorType
    {
        public string VideoFile { get; set; }
        public float CurrentTimeSeconds { get; set; }
        public float MaxTimeSeconds { get; set; }
        public int Volume { get; set; }
        public bool IsPlaying { get; set; }
    }
}
