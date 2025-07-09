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

        [HttpPost("login")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password are required.");
            }

            var employee = await aiTaskService.LoginDetails(userName, password);

            if (employee == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(employee);
        }
    }

}
