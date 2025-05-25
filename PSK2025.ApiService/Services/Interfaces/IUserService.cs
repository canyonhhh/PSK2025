using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedResult<UserDto>> GetAllUsersAsync(
            string? email = null,
            string? role = null,
            UserSortBy sortBy = UserSortBy.CreatedAt,
            bool ascending = false,
            int page = 1,
            int pageSize = 10);
        Task<(UserDto? User, ServiceError Error)> GetUserByIdAsync(string id);
        Task<(UserDto? User, ServiceError Error)> GetUserByEmailAsync(string email);
        Task<(UserDto? User, ServiceError Error)> RegisterUserAsync(RegisterDto model);
        Task<(UserDto? User, ServiceError Error)> UpdateUserAsync(string id, UpdateUserDto model);
        Task<ServiceError> DeleteUserAsync(string id);
        Task<(UserDto? User, ServiceError Error)> ChangeUserRoleAsync(string id, string newRole);
    }
}