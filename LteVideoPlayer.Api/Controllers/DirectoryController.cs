using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Models.Dtos;
using LteVideoPlayer.Api.Models.Enums;
using LteVideoPlayer.Api.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace LteVideoPlayer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors]
    public class DirectoryController : ControllerBase
    {
        private readonly IDirectoryService _directoryService;
        private readonly ILogger<DirectoryController> _logger;
        private readonly IVideoConfigService _videoConfigService;

        public DirectoryController(IDirectoryService directoryService,
            ILogger<DirectoryController> logger,
            IVideoConfigService videoConfigService)
        {
            _directoryService = directoryService;
            _logger = logger;
            _videoConfigService = videoConfigService;
        }

        [HttpGet("GetDirectories")]
        public IActionResult GetDirectories()
        {
            try
            {
                return Ok(_videoConfigService.GetVideoConfigs());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Directory", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetRootDirsAndFiles")]
        public async Task<IActionResult> GetRootDirsAndFiles([FromQuery] DirectoryEnum dirEnum)
        {
            try
            {
                return Ok(await _directoryService.GetDirsAndFilesAsync(dirEnum, "", false));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDirsAndFiles")]
        public async Task<IActionResult> GetDirsAndFiles([FromQuery] DirectoryEnum dirEnum, [FromQuery] string path)
        {
            try
            {
                return Ok(await _directoryService.GetDirsAndFilesAsync(dirEnum, path, false));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetNextFile")]
        public IActionResult GetNextFile([FromQuery] DirectoryEnum dirEnum, [FromBody] FileDto file)
        {
            try
            {
                return Ok(_directoryService.GetNextFile(dirEnum, file));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("StreamFile")]
        public IActionResult StreamFile([FromQuery] DirectoryEnum dirEnum, [FromQuery] string fullPath)
        {
            try
            {
                var videoConfig = _videoConfigService.GetVideoConfig(dirEnum);
                if (!videoConfig.CanPlayVideo)
                    throw new Exception("Play video disabled for this directory");

                var rootFilePathName = Path.Combine(videoConfig.RootVideoDir, fullPath);
                if (!System.IO.File.Exists(rootFilePathName))
                    throw new FileNotFoundException(rootFilePathName);

                return new FileStreamResult(
                    new FileStream(rootFilePathName, FileMode.Open,
                        FileAccess.Read, FileShare.Read),
                    "application/octet-stream")
                {
                    EnableRangeProcessing = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
