namespace LteVideoPlayer.Api.Dtos
{
    public class RemoteData_SetSeekDto : RemoteData, IRefactorType
    {
        public float? SeekPercentPosition { get; set; }
    }
}
