using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;
using PSK2025.Models.Extensions;
using visus.Models.DTOs;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthService authService, IGetUserIdService getUserIdService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (error, token) = await authService.LoginAsync(model);

            if (error == ServiceError.None)
            {
                return Ok(new { token });
            }

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Login"));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var error = await authService.ForgotPasswordAsync(model.Email);

            if (error == ServiceError.None)
            {
                return Ok();
            }

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Password reset"));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var error = await authService.ResetPasswordAsync(model.UserId, model.Token, model.NewPassword);

            if (error == ServiceError.None)
            {
                return Ok();
            }

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Password reset"));
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = getUserIdService.GetUserIdFromToken();
            var error = await authService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (error == ServiceError.None)
            {
                return Ok();
            }

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Password change"));
        }
    }
}