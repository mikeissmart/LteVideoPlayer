using LteVideoPlayer.Api.Configs;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Service;
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
        private readonly VideoConfig _videoConfig;

        public DirectoryController(IDirectoryService directoryService,
            ILogger<DirectoryController> logger,
            VideoConfig videoConfig)
        {
            _directoryService = directoryService;
            _logger = logger;
            _videoConfig = videoConfig;
        }

        [HttpGet("GetRootDirsAndFiles")]
        public IActionResult GetRootDirsAndFiles([FromQuery] bool isStaging)
        {
            try
            {
                return Ok(_directoryService.GetDirsAndFiles("", isStaging));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Directory", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetDirsAndFiles")]
        public IActionResult GetDirsAndFiles([FromQuery] string dirPathName, [FromQuery] bool isStaging)
        {
            try
            {
                return Ok(_directoryService.GetDirsAndFiles(dirPathName, isStaging));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Directory", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("GetNextFile")]
        public IActionResult GetNextFile([FromBody] FileDto file, [FromQuery] bool isStaging)
        {
            try
            {
                return Ok(_directoryService.GetNextFile(file, isStaging));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Directory", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("StreamFile")]
        public IActionResult StreamFile([FromQuery] string filePathName)
        {
            try
            {
                var rootFilePathName = _videoConfig.VideoPath + filePathName;
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
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
