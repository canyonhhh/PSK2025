using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<(bool Succeeded, UserDto? CreatedUser, string[] Errors)> RegisterUserAsync(RegisterDto model);
        Task<(bool Succeeded, UserDto? UpdatedUser, string[] Errors)> UpdateUserAsync(string id, UpdateUserDto model);
        Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string id);
        Task<(bool Succeeded, UserDto? UpdatedUser, string[] Errors)> ChangeUserRoleAsync(string id, string newRole);
    }
}