using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService, IAuthService authService, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, user, errors) = await userService.RegisterUserAsync(model);


            if (!succeeded)
            {
                return BadRequest(new { errors });
            }

            var loginDto = mapper.Map<LoginDto>(model);

            var (loginSucceeded, token, loginErrors) = await authService.LoginAsync(loginDto);

            if (!loginSucceeded)
            {
                return BadRequest(new { loginErrors });
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

            var (succeeded, user, errors) = await userService.UpdateUserAsync(id, model);

            if (succeeded)
            {
                return Ok(user);
            }

            if (errors.Contains("User not found"))
            {
                return NotFound();
            }

            return BadRequest(new { errors });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var (succeeded, errors) = await userService.DeleteUserAsync(id);

            if (succeeded)
            {
                return NoContent();
            }

            if (errors.Contains("User not found"))
            {
                return NotFound();
            }

            return BadRequest(new { errors });
        }

        [HttpPut("{id}/change-role")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ChangeUserRole(string id, [FromBody] string newRole)
        {
            var (succeeded, user, errors) = await userService.ChangeUserRoleAsync(id, newRole);

            if (succeeded)
            {
                return Ok(user);
            }

            if (errors.Contains("User not found"))
            {
                return NotFound();
            }

            return BadRequest(new { errors });
        }
    }
}