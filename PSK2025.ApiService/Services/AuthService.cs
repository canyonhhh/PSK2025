using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PSK2025.ApiService.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(RegisterDTO model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return (true, Array.Empty<string>());
            }

            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<(bool Succeeded, string Token, string[] Errors)> LoginAsync(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return (false, string.Empty, new[] { "User does not exist" });
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return (false, string.Empty, new[] { "Invalid password" });
            }

            var token = await GenerateJwtToken(user);

            return (true, token, Array.Empty<string>());
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
            };
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
            }

            if (!string.IsNullOrEmpty(user.LastName))
            {
                claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? "temporary secret"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(double.Parse(_configuration["JWT:ExpirationInDays"] ?? "1"));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"] ?? "temporary issuer",
                audience: _configuration["JWT:ValidAudience"] ?? "temporary audience",
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}