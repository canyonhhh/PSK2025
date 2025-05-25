using Microsoft.AspNetCore.Identity;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<(List<User> Users, int TotalCount)> GetAllAsync(
            string? email = null,
            string? role = null,
            UserSortBy sortBy = UserSortBy.CreatedAt,
            bool ascending = false,
            int page = 1,
            int pageSize = 10);
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<(IdentityResult, User)> CreateAsync(User user, string password);
        Task<(IdentityResult, User)> UpdateAsync(User user);
        Task<IdentityResult> DeleteAsync(User user);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<IdentityResult> RemoveFromRoleAsync(User user, string role);
        Task<IList<string>> GetRolesAsync(User user);
        Task<bool> IsInRoleAsync(User user, string role);
        Task<List<string>> GetUserRoleAsync(User user);
    }
}