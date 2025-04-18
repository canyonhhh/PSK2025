using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using visus.Models.DTOs;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, token, errors) = await authService.LoginAsync(model);

            if (succeeded)
            {
                return Ok(new { token });
            }

            return Unauthorized(new { errors });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await authService.ForgotPasswordAsync(model.Email);

            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errors) = await authService.ResetPasswordAsync(model.UserId, model.Token, model.NewPassword);

            if (succeeded)
            {
                return Ok();
            }

            return BadRequest(new { errors });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var (succeeded, errors) = await authService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (succeeded)
            {
                return Ok();
            }

            return BadRequest(new { errors });
        }
    }
}