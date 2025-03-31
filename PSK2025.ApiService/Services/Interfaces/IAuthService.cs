using PSK2025.Models.DTOs;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(RegisterDTO model);
        Task<(bool Succeeded, string Token, string[] Errors)> LoginAsync(LoginDTO model);
        Task<UserDTO?> GetUserByEmailAsync(string email);
    }
}