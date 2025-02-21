using AutoMapper;
using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Models.Dtos;
using LteVideoPlayer.Api.Models.Enums;

namespace LteVideoPlayer.Api.Services
{
    public interface IVideoConfigService : IService
    {
        IVideoConfig GetVideoConfig(DirectoryEnum dirEnum);
        List<DirectoryInfoDto> GetVideoConfigs();
    }

    public class VideoConfigService : BaseService, IVideoConfigService
    {
        private readonly Dictionary<DirectoryEnum, IVideoConfig> _videoConfigs;

        public VideoConfigService(TvConfig tvConfig, TvStagingConfig tvStagingConfig, CameraConfig cameraConfig)
        {
            _videoConfigs = new Dictionary<DirectoryEnum, IVideoConfig>
            {
                { DirectoryEnum.Tv, tvConfig },
                { DirectoryEnum.Tv_Staging, tvStagingConfig },
                { DirectoryEnum.Camera, cameraConfig }
            };
        }

        public IVideoConfig GetVideoConfig(DirectoryEnum dirEnum)
        {
            if (_videoConfigs.TryGetValue(dirEnum, out var videoConfig))
                return videoConfig;

            throw new NotImplementedException();
        }

        public List<DirectoryInfoDto> GetVideoConfigs()
        {
            return _videoConfigs
                .Select(x => new DirectoryInfoDto
                {
                    FriendlyName = x.Value.FriendlyName,
                    AdminViewOnly = x.Value.AdminViewOnly,
                    DirEnum = x.Key,
                })
                .ToList();
        }
    }
}
