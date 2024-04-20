namespace LteVideoPlayer.Api.Dtos
{
    public class RemoteData_SetSeekDto : RemoteDataDto, IRefactorType
    {
        public float? SeekPercentPosition { get; set; }
    }
}
