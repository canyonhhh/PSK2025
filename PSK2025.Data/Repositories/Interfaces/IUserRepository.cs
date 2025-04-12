using PSK2025.Models.Entities;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<(bool Succeeded, string[] Errors)> CreateAsync(User user, string password);
        Task<(bool Succeeded, string[] Errors)> UpdateAsync(User user);
        Task<(bool Succeeded, string[] Errors)> DeleteAsync(User user);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<(bool Succeeded, string[] Errors)> AddToRoleAsync(User user, string role);
        Task<(bool Succeeded, string[] Errors)> RemoveFromRoleAsync(User user, string role);
        Task<IList<string>> GetRolesAsync(User user);
        Task<bool> IsInRoleAsync(User user, string role);
        Task<List<string>> GetUserRoleAsync(User user);
    }
}