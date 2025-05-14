using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.Enums;
using System.Threading.Tasks;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("api/admin/settings")]
    [Authorize(Roles = "Manager")]
    public class AppSettingsController : ControllerBase
    {
        private readonly IAppSettingsService _service;

        public AppSettingsController(IAppSettingsService service)
        {
            _service = service;
        }

        [HttpGet("ordering-status")]
        public async Task<IActionResult> GetStatus()
        {
            var (paused, error) = await _service.GetOrderingStatusAsync();
            if (error != ServiceError.None)
                return StatusCode(500, new { message = "Database error" });
            return Ok(new { orderingPaused = paused });
        }

        [HttpPost("pause-ordering")]
        public async Task<IActionResult> Pause()
        {
            if (await _service.PauseOrderingAsync() != ServiceError.None)
                return StatusCode(500, new { message = "Could not pause ordering." });
            return NoContent();
        }

        [HttpPost("resume-ordering")]
        public async Task<IActionResult> Resume()
        {
            if (await _service.ResumeOrderingAsync() != ServiceError.None)
                return StatusCode(500, new { message = "Could not resume ordering." });
            return NoContent();
        }
    }
}
