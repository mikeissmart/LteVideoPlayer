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
    public class ConvertFileController : ControllerBase
    {
        private readonly IConvertFileService _convertFileService;

        public ConvertFileController(IConvertFileService convertVideoFileService)
        {
            _convertFileService = convertVideoFileService;
        }

        [HttpGet("GetAllConvertFiles")]
        public async Task<IActionResult> GetAllConvertFiles([FromQuery] DirectoryEnum dirEnum)
        {
            try
            {
                return Ok(await _convertFileService.GetAllConvertFilesAsync(dirEnum));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetConvertFileByOriginalFile")]
        public async Task<IActionResult> GetConvertFileByOriginalFile([FromQuery] DirectoryEnum dirEnum, [FromBody] FileDto file)
        {
            try
            {
                return Ok(await _convertFileService.GetConvertFileByOriginalFileAsync(dirEnum, file));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddConvert")]
        public async Task<IActionResult> AddConvert([FromQuery] DirectoryEnum dirEnum, [FromBody] CreateConvertDto convert)
        {
            try
            {
                return Ok(await _convertFileService.AddConvertFileAsync(dirEnum, convert, ModelState));
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("AddManyConvert")]
        public async Task<IActionResult> AddManyConvert([FromQuery] DirectoryEnum dirEnum, [FromBody] CreateManyConvertDto convert)
        {
            try
            {
                return Ok(await _convertFileService.AddConvertManyFileAsync(dirEnum, convert, ModelState));
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetCurrentConvertFile")]
        public IActionResult GetCurrentConvertFile()
        {
            return Ok(_convertFileService.GetCurrentConvertFile());
        }

        [HttpGet("GetVideoMeta")]
        public async Task<IActionResult> GetVideoMeta([FromQuery] DirectoryEnum dirEnum, [FromQuery] string fullPath)
        {
            try
            {
                var result = await _convertFileService.GetVideoMetaAsync(dirEnum, fullPath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
