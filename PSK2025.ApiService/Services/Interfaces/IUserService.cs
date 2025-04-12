using PSK2025.Models.DTOs;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(RegisterDto model);
        Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(string id, UpdateUserDto model);
        Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string id);
        Task<(bool Succeeded, string[] Errors)> ChangePasswordAsync(string id, string currentPassword, string newPassword);
        Task<(bool Succeeded, string[] Errors)> ChangeUserRoleAsync(string id, string newRole);
    }
}