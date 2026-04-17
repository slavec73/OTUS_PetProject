using Microsoft.AspNetCore.Mvc;
using VacationPlanner.Interfaces;

namespace VacationPlanner.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IDbHealthService _dbHealth;
        private readonly ICacheService _cache;

        public TestController(IEmailService emailService, IDbHealthService dbHealth, ICacheService cacheService)
        {
            _emailService = emailService;
            _dbHealth = dbHealth;
            _cache = cacheService;
        }

        [HttpGet(Name = "SendTestMessage")]
        public async Task<IActionResult> SendTestMessage()
        {
            await _emailService.SendAsync(
            "test@example.com",
            "Test email",
            "Hello from API 🚀"
        );

            return Ok("Sent");
        }

        [HttpGet(Name = "CheckDb")]
        public async Task<IActionResult> CheckDb()
        {
            var ok = await _dbHealth.CanConnectAsync();

            return ok
                ? Ok("DB OK!")
                : StatusCode(500, "DB FAIL");
        }

        [HttpPost(Name = "InsertKey")]
        public async Task<IActionResult> Set(string key, [FromBody] string value)
        {
            await _cache.SetAsync(key, value, TimeSpan.FromMinutes(10));
            return Ok("Saved");
        }

        [HttpDelete(Name = "DeleteKey")]
        public async Task<IActionResult> Delete(string key)
        {
            await _cache.RemoveAsync(key);
            return Ok("Deleted");
        }
    }
}

