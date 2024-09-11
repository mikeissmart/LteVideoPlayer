namespace LteVideoPlayer.Api.Dtos
{
    public class CreateManyConvertDto : IRefactorType
    {
        public List<CreateConvertDto> Converts { get; set; }
    }
}
