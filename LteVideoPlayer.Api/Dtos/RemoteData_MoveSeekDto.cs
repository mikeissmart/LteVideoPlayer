namespace LteVideoPlayer.Api.Dtos
{
    public class RemoteData_MoveSeekDto : RemoteDataDto, IRefactorType
    {
        public int SeekPosition { get; set; }
    }
}
