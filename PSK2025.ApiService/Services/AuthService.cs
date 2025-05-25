using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PSK2025.ApiService.Services
{
    public class AuthService(
        IUserRepository userRepository,
        UserManager<User> userManager,
        IConfiguration configuration,
        ILogger<AuthService> logger)
        : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<AuthService> _logger = logger;

        public async Task<(ServiceError Error, string Token)> LoginAsync(LoginDto model)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(model.Email);
                if (user == null)
                {
                    return (ServiceError.Unauthorized, string.Empty);
                }

                var result = await _userRepository.CheckPasswordAsync(user, model.Password);
                if (!result)
                {
                    return (ServiceError.Unauthorized, string.Empty);
                }

                var token = await GenerateJwtToken(user);
                return (ServiceError.None, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", model.Email);
                return (ServiceError.DatabaseError, string.Empty);
            }
        }

        public Task<ServiceError> ForgotPasswordAsync(string email)
        {
            // TODO: Implement email sending logic
            throw new NotImplementedException("Email sending is not implemented yet.");
        }

        public async Task<ServiceError> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return ServiceError.NotFound;
                }

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                return result.Succeeded ? ServiceError.None : ServiceError.InvalidData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user {UserId}", userId);
                return ServiceError.DatabaseError;
            }
        }

        public async Task<ServiceError> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return ServiceError.NotFound;
                }

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                return result.Succeeded ? ServiceError.None : ServiceError.InvalidData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return ServiceError.DatabaseError;
            }
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ??
                                                                      throw new ArgumentException(
                                                                          "JWT:Secret is not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(double.Parse(_configuration["JWT:ExpirationInDays"] ??
                                                            throw new ArgumentException(
                                                                "JWT:ExpirationInDays is not configured")));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}