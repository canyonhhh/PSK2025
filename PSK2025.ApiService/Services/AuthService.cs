using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;

namespace PSK2025.ApiService.Services
{
    public class AuthService(
        IUserRepository userRepository,
        UserManager<User> userManager,
        IConfiguration configuration)
        : IAuthService
    {
        public async Task<(bool Succeeded, string Token, string[] Errors)> LoginAsync(LoginDto model)
        {
            var user = await userRepository.GetByEmailAsync(model.Email);

            if (user == null)
            {
                return (false, string.Empty, ["User does not exist"]);
            }

            var result = await userRepository.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return (false, string.Empty, ["Invalid password"]);
            }

            var token = await GenerateJwtToken(user);

            return (true, token, []);
        }

        public Task<(bool Succeeded, string[] Errors)> ForgotPasswordAsync(string email)
        {
            // TODO : Implement email sending logic
            throw new NotImplementedException("Email sending is not implemented yet.");

            /*
                        var user = await _userRepository.GetByEmailAsync(email);
                        if (user == null)
                        {
                            return (true, Array.Empty<string>());
                        }

                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);


                        return (true, Array.Empty<string>());
            */
        }

        public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(string userId, string token,
            string newPassword)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, ["User not found"]);
            }

            var result = await userManager.ResetPasswordAsync(user, token, newPassword);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<(bool Succeeded, string[] Errors)> ChangePasswordAsync(string userId, string currentPassword,
            string newPassword)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, ["User not found"]);
            }

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var roles = await userManager.GetRolesAsync(user);
    
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
            }

            if (!string.IsNullOrEmpty(user.LastName))
            {
                claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"] ??
                                                                      throw new ArgumentException(
                                                                          "JWT:Secret is not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(double.Parse(configuration["JWT:ExpirationInDays"] ??
                                                            throw new ArgumentException(
                                                                "JWT:ExpirationInDays is not configured")));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }    }
}
