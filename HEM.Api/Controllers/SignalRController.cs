using HEM.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HEM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalRController(INotificationService notificationService) : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] string message)
        {
            await notificationService.SendNotificationToAllAsync(message);
            return Ok("Notification sent");
        }
    }
}
