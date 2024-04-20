namespace LteVideoPlayer.Api.Dtos
{
    public class RemoteData_SetVolumeDto : RemoteDataDto, IRefactorType
    {
        public float Volume { get; set; }
    }
}
