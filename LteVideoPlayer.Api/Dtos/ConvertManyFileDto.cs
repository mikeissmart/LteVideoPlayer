namespace LteVideoPlayer.Api.Dtos
{
    public class ConvertManyFileDto : IRefactorType
    {
        public List<ConvertFileDto> Converts { get; set; } = new List<ConvertFileDto>();
    }
}
