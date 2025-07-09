using HEM.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HEM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiTaskController(IAiTaskService aiTaskService): Controller
    {
        [HttpPost("send")]
        public async Task<IActionResult> TaskSplitting(string user, string input)
        {
            var response = await aiTaskService.TaskSplitting(user, input);
            return Ok(response);
        }
    }
}
