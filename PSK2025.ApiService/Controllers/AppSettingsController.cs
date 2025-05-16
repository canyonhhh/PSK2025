using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.Enums;
using PSK2025.Models.Extensions;
using System.Threading.Tasks;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Manager")]
    public class AppSettingsController : ControllerBase
    {
        private readonly IAppSettingsService _service;

        public AppSettingsController(IAppSettingsService service)
        {
            _service = service;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetOrderingStatus()
        {
            var (paused, error) = await _service.GetOrderingStatusAsync();

            if (error == ServiceError.None)
                return Ok(new { orderingPaused = paused });

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("ordering")
            );
        }

        [HttpPost("pause")]
        public async Task<IActionResult> PauseOrdering()
        {
            var error = await _service.PauseOrderingAsync();

            if (error == ServiceError.None)
                return NoContent();

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("ordering")
            );
        }

        [HttpPost("resume")]
        public async Task<IActionResult> ResumeOrdering()
        {
            var error = await _service.ResumeOrderingAsync();

            if (error == ServiceError.None)
                return NoContent();

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("ordering")
            );
        }
    }
}