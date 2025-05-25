using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;
using PSK2025.Models.Extensions;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService, IAuthService authService, IMapper mapper, IGetUserIdService getUserIdService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string? email = null,
            [FromQuery] string? role = null,
            [FromQuery] UserSortBy sortBy = UserSortBy.CreatedAt,
            [FromQuery] bool ascending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await userService.GetAllUsersAsync(email, role, sortBy, ascending, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string id)
        {
            var currentUserId = getUserIdService.GetUserIdFromToken();
            if (id != currentUserId && !User.IsInRole("Manager"))
            {
                return Forbid();
            }

            var (user, error) = await userService.GetUserByIdAsync(id);

            if (error == ServiceError.None)
                return Ok(user);

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("User"));
        }

        [HttpGet("email/{email}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var (user, error) = await userService.GetUserByEmailAsync(email);

            if (error == ServiceError.None)
                return Ok(user);

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("User"));
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (user, error) = await userService.RegisterUserAsync(model);

            if (error != ServiceError.None)
            {
                return StatusCode(
                    error.GetStatusCode(),
                    error.GetErrorMessage("User"));
            }

            var loginDto = mapper.Map<LoginDto>(model);
            var (loginError, token) = await authService.LoginAsync(loginDto);

            if (loginError != ServiceError.None)
            {
                return StatusCode(
                    loginError.GetStatusCode(),
                    loginError.GetErrorMessage("Login"));
            }

            return CreatedAtAction(nameof(GetUser), new { id = user!.Id }, new { token });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = getUserIdService.GetUserIdFromToken();
            if (id != currentUserId && !User.IsInRole("Manager"))
            {
                return Forbid();
            }

            var (user, error) = await userService.UpdateUserAsync(id, model);

            if (error == ServiceError.None)
                return Ok(user);

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("User"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var error = await userService.DeleteUserAsync(id);

            if (error == ServiceError.None)
                return NoContent();

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("User"));
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ChangeUserRole(string id, [FromBody] ChangeUserRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (user, error) = await userService.ChangeUserRoleAsync(id, model.Role);

            if (error == ServiceError.None)
                return Ok(user);

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("User"));
        }
    }
}