using LteVideoPlayer.Api.CronJob.Convert;
using LteVideoPlayer.Api.Dtos;
using LteVideoPlayer.Api.Service;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace LteVideoPlayer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors]
    public class ShareConnectController : ControllerBase
    {
        private readonly ShareConnect _shareConnect;

        public ShareConnectController(ShareConnect shareConnect)
        {
            _shareConnect = shareConnect;
        }

        [HttpGet("ShareConnectStatus")]
        public IActionResult ShareConnectStatus()
        {
            try
            {
                return Ok(new StringDto { Data = _shareConnect.ConnectOutput });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
