using LteVideoPlayer.Api.CronJob.Convert;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace LteVideoPlayer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConvertFileController : ControllerBase
    {
        private readonly IConvertFileService _convertFileService;

        public ConvertFileController(IConvertFileService convertVideoFileService)
        {
            _convertFileService = convertVideoFileService;
        }

        [HttpGet("GetAllConvertFiles")]
        public async Task<IActionResult> GetAllConvertFiles()
        {
            try
            {
                return Ok(await _convertFileService.GetAllConvertFilesAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetConvertFileByOriginalFile")]
        public async Task<IActionResult> GetConvertFileByOriginalFile([FromBody] FileDto file)
        {
            try
            {
                return Ok(await _convertFileService.GetConvertFileByOriginalFileAsync(file));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddConvert")]
        public async Task<IActionResult> AddConvert([FromBody] CreateConvertDto convert)
        {
            try
            {
                return Ok(await _convertFileService.AddConvertFileAsync(convert, ModelState));
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }
    }
}
