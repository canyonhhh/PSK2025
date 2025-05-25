using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(ServiceError Error, string Token)> LoginAsync(LoginDto model);
        Task<ServiceError> ForgotPasswordAsync(string email);
        Task<ServiceError> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<ServiceError> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}