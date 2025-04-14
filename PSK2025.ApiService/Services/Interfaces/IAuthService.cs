using PSK2025.Models.DTOs;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string Token, string[] Errors)> LoginAsync(LoginDto model);
        Task<(bool Succeeded, string[] Errors)> ForgotPasswordAsync(string email);
        Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<(bool Succeeded, string[] Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}