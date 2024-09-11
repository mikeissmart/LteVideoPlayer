namespace LteVideoPlayer.Api.Dtos
{
    public class ConvertFileQueueDto : IRefactorType
    {
        public int Index { get; set; }
        public bool Skip { get; set; }
        public bool ConvertQueued { get; set; }
        public string ConvertName { get; set; }
        public string AppendConvertName { get; set; }
        public FileDto File { get; set; }
    }
}
