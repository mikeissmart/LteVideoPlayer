namespace LteVideoPlayer.Api.Dtos
{
    public class RemoteData_MoveSeekDto : RemoteData, IRefactorType
    {
        public int SeekPosition { get; set; }
    }
}
