using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Service;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace LteVideoPlayer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors]
    public class FileHistoryController : ControllerBase
    {
        private readonly IFileHistoryService _fileHistoryService;

        public FileHistoryController(IFileHistoryService watchVideoFileService)
        {
            _fileHistoryService = watchVideoFileService;
        }

        [HttpGet("GetFileHistoriesByUserProfile")]
        public async Task<IActionResult> GetFileHistoriesByUserProfile([FromQuery] int userProfileId)
        {
            try
            {
                return Ok(await _fileHistoryService.GetFileHistoriesByUserProfileAsync(userProfileId));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("FileHistory", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("AddUpdateFileHistory")]
        public async Task<IActionResult> AddUpdateFileHistory([FromBody] FileHistoryDto fileHistory)
        {
            try
            {
                return Ok(await _fileHistoryService.AddUpdateFileHistoryAsync(fileHistory));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("FileHistory", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
