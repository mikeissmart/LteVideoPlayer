namespace LteVideoPlayer.Api.Dtos
{
    public abstract class RemoteData : IRefactorType
    {
        public string Profile { get; set; }
        public int Channel { get; set; }
    }
}
