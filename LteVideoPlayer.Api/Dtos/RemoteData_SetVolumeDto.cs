namespace LteVideoPlayer.Api.Dtos
{
    public class RemoteData_SetVolumeDto : RemoteData, IRefactorType
    {
        public float Volume { get; set; }
    }
}
