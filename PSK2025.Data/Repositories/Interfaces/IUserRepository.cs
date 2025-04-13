using Microsoft.AspNetCore.Identity;
using PSK2025.Models.Entities;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
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