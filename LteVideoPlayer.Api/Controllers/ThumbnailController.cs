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
    public class ThumbnailController : ControllerBase
    {
        private readonly IThumbnailService _thumbnailService;
        private readonly ILogger<ThumbnailController> _logger;
        private readonly VideoConfig _videoConfig;

        public ThumbnailController(IThumbnailService thumbnailService,
            ILogger<ThumbnailController> logger,
            VideoConfig videoConfig)
        {
            _thumbnailService = thumbnailService;
            _logger = logger;
            _videoConfig = videoConfig;
        }

        [HttpGet("GetThumbnailErrors")]
        public async Task<IActionResult> GetThumbnailErrors()
        {
            try
            {
                return Ok(await _thumbnailService.GetThumbnailErrorsAsync());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetFolderThunbmail")]
        public IActionResult GetFolderThunbmail([FromQuery] string filePathName)
        {
            try
            {
                var thumbnail = _thumbnailService.GetFolderThumbnail(filePathName);
                if (thumbnail == "")
                    thumbnail = _videoConfig.DefaultThumbnailFile;

                var image = System.IO.File.OpenRead(thumbnail);
                return File(image, "image/jpeg");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetFileThumbnail")]
        public IActionResult GetFileThumbnail([FromQuery] string filePathName)
        {
            try
            {
                var thumbnail = _thumbnailService.GetFileThumbnail(filePathName);
                if (thumbnail == "")
                    thumbnail = _videoConfig.DefaultThumbnailFile;

                var image = System.IO.File.OpenRead(thumbnail);
                return File(image, "image/jpeg");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("HasFileThumbnail")]
        public IActionResult HasFileThumbnail([FromQuery] string filePathName)
        {
            try
            {
                var thumbnail = _thumbnailService.GetFileThumbnail(filePathName);
                return Ok(thumbnail != "");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("DeleteThumbnail")]
        public IActionResult DeleteThumbnail([FromBody] StringDto data)
        {
            try
            {
                _thumbnailService.DeleteThumbnail(data.Data);
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("DeleteThumbnailError")]
        public async Task<IActionResult> DeleteThumbnailError([FromBody] FileDto file)
        {
            try
            {
                await _thumbnailService.DeleteThumbnailErrorAsync(file);
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("DeleteManyThumbnailErrors")]
        public async Task<IActionResult> DeleteManyThumbnailErrors([FromBody] FileDto[] files)
        {
            try
            {
                foreach (var file in files)
                    await _thumbnailService.DeleteThumbnailErrorAsync(file);

                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetVideoMeta")]
        public async Task<IActionResult> GetVideoMeta([FromQuery] string filePathName, [FromQuery] bool isStaging)
        {
            try
            {
                var result = await _thumbnailService.GetVideoMetaAsync(filePathName, isStaging);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetWorkingThumbnail")]
        public IActionResult GetWorkingThumbnail()
        {
            return Ok(new StringDto { Data = _thumbnailService.GetWorkingThumbnail() });
        }
    }
}
