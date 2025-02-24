using AutoMapper;
using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Models.Dtos;
using LteVideoPlayer.Api.Models.Enums;

namespace LteVideoPlayer.Api.Services
{
    public interface IVideoConfigService : IService
    {
        VideoConfig GetVideoConfig(DirectoryEnum dirEnum);
        List<DirectoryInfoDto> GetVideoConfigs();
    }

    public class VideoConfigService : BaseService, IVideoConfigService
    {
        private readonly VideoConfigs _videoConfigs;

        public VideoConfigService(VideoConfigs videoConfigs)
        {
            _videoConfigs = videoConfigs;
        }

        public VideoConfig GetVideoConfig(DirectoryEnum dirEnum)
        {
            foreach (var videoConfig in _videoConfigs.Configs)
            {
                if (videoConfig.DirectoryEnum == dirEnum)
                    return videoConfig;
            }

            throw new NotImplementedException();
        }

        public List<DirectoryInfoDto> GetVideoConfigs()
        {
            return _videoConfigs.Configs
                .Select(x => new DirectoryInfoDto
                {
                    FriendlyName = x.FriendlyName,
                    CanConvertVideo = x.CanConvertVideo,
                    CanPlayVideo = x.CanPlayVideo,
                    CanThumbnailVideo = x.CanThumbnailVideo,
                    AdminViewOnly = x.AdminViewOnly,
                    DirEnum = x.DirectoryEnum,
                })
                .ToList();
        }
    }
}
