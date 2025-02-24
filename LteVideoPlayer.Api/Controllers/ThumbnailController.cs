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
    public class ThumbnailController : ControllerBase
    {
        private readonly IThumbnailService _thumbnailService;
        private readonly ILogger<ThumbnailController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ThumbnailController(IThumbnailService thumbnailService,
            ILogger<ThumbnailController> logger,
            IWebHostEnvironment environment)
        {
            _thumbnailService = thumbnailService;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet("GetAllThumbnailErrors")]
        public async Task<IActionResult> GetAllThumbnailErrorsAsync()
        {
            try
            {
                return Ok(await _thumbnailService.GetAllThumbnailErrorsAsync());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetFolderThunbmail")]
        public IActionResult GetFolderThunbmail([FromQuery] DirectoryEnum dirEnum, [FromQuery] string fullPath)
        {
            try
            {
                var thumbnail = _thumbnailService.GetFolderThumbnail(dirEnum, fullPath);
                if (thumbnail.Length == 0)
                    return File(System.IO.File.OpenRead(DefualtThumbnail()), "image/jpeg");
                else
                    return File(System.IO.File.OpenRead(thumbnail), "image/jpeg");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetFileThumbnail")]
        public IActionResult GetFileThumbnail([FromQuery] DirectoryEnum dirEnum, [FromQuery] string fullPath)
        {
            try
            {
                var thumbnail = _thumbnailService.GetFileThumbnail(dirEnum, fullPath);
                if (thumbnail.Length == 0)
                    return File(System.IO.File.OpenRead(DefualtThumbnail()), "image/jpeg");
                else
                    return File(System.IO.File.OpenRead(thumbnail), "image/jpeg");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("HasFileThumbnail")]
        public IActionResult HasFileThumbnail([FromQuery] DirectoryEnum dirEnum, [FromQuery] string fullPath)
        {
            try
            {
                var thumbnail = _thumbnailService.GetFileThumbnail(dirEnum, fullPath);
                return Ok(thumbnail != "");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("DeleteThumbnail")]
        public IActionResult DeleteThumbnail([FromQuery] DirectoryEnum dirEnum, [FromBody] StringDto data)
        {
            try
            {
                _thumbnailService.DeleteThumbnail(dirEnum, data.Data);
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("DeleteThumbnailError")]
        public async Task<IActionResult> DeleteThumbnailError([FromQuery] DirectoryEnum dirEnum, [FromBody] FileDto file)
        {
            try
            {
                await _thumbnailService.DeleteThumbnailErrorAsync(dirEnum, file);
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("DeleteManyThumbnailErrors")]
        public async Task<IActionResult> DeleteManyThumbnailErrors([FromQuery] DirectoryEnum dirEnum, [FromBody] FileDto[] files)
        {
            try
            {
                foreach (var file in files)
                    await _thumbnailService.DeleteThumbnailErrorAsync(dirEnum, file);

                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Thumbnail", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetCurrentThumbnail")]
        public IActionResult GetCurrentThumbnail()
        {
            return Ok(new StringDto { Data = _thumbnailService.GetCurrentThumbnail() });
        }

        private string DefualtThumbnail()
            => Path.Combine(_environment.WebRootPath, "images", "Default.jpeg");
    }
}
