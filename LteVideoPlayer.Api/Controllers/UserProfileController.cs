using LteVideoPlayer.Api.Models.Dtos;
using LteVideoPlayer.Api.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace LteVideoPlayer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet("GetAllUserProfiles")]
        public async Task<IActionResult> GetAllUserProfiles()
        {
            try
            {
                return Ok(await _userProfileService.GetAllUserProfilesAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUserProfileById")]
        public async Task<IActionResult> GetUserProfileById([FromQuery] int userProfileId)
        {
            try
            {
                return Ok(await _userProfileService.GetUserProfileByIdAsync(userProfileId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateUserProfile")]
        public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileDto userProfile)
        {
            try
            {
                return Ok(await _userProfileService.CreateUserProfileAsync(userProfile, ModelState));
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileDto userProfile)
        {
            try
            {
                return Ok(await _userProfileService.UpdateUserProfileAsync(userProfile, ModelState));
            }
            catch
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("DeleteUserProfile")]
        public async Task<IActionResult> DeleteUserProfile([FromBody] int userProfileId)
        {
            try
            {
                await _userProfileService.DeleteUserProfileAsync(userProfileId);
                return Ok(true);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserProfile", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
