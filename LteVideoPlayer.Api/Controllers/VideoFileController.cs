using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace LteVideoPlayer.Api.Controllers
{
    /*[ApiController]
    [Route("api/[controller]")]
    public class VideoFileController : ControllerBase
    {
        private readonly IVideoFileService _videoFileService;
        private readonly VideoConfig _videoConfig;

        public VideoFileController(IVideoFileService videoFileService,
            VideoConfig videoConfig)
        {
            _videoFileService = videoFileService;
            _videoConfig = videoConfig;
        }

        [HttpGet("GetAllVideoDirsAndFiles")]
        public async Task<IActionResult> GetAllVideoDirsAndFiles()
        {
            try
            {
                return Ok(await _videoFileService.GetAllVideoDirsAndFilesAsync());
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("GetVideoFileById")]
        public async Task<IActionResult> GetVideoFileById([FromQuery] int videoFileId)
        {
            try
            {
                return Ok(await _videoFileService.GetVideoFilesByIdAsync(videoFileId));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("StreamVideoFilesById")]
        public async Task<FileResult> StreamVideoFilesById([FromQuery] int videoFileId)
        {
            var videoFile = await _videoFileService.GetVideoFilesByIdAsync(videoFileId);
            if (videoFile == null)
                throw new ArgumentException("VideoFile not found " + videoFileId);
            var rootVideoPathName = _videoConfig.RootPath + videoFile.VideoPathName;
            if (!System.IO.File.Exists(rootVideoPathName))
                throw new FileNotFoundException(rootVideoPathName);

            return new FileStreamResult(
                new FileStream(rootVideoPathName, FileMode.Open,
                    FileAccess.Read, FileShare.Read),
                "application/octet-stream")
            {
                EnableRangeProcessing = true
            };
        }

        [HttpPost("UpdateVideoFile")]
        public async Task<IActionResult> UpdateVideoFile([FromBody] FileDto videoFile)
        {
            try
            {
                await _videoFileService.UpdateVideoFileAsync(videoFile);
                return Ok(true);
            }
            catch
            {
                return BadRequest();
            }
        }
    }*/
}
