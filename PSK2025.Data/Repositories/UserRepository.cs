using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.Data.Repositories
{
    public class UserRepository(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
        : IUserRepository
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        public async Task<(List<User> Users, int TotalCount)> GetAllAsync(
            string? email = null,
            string? role = null,
            UserSortBy sortBy = UserSortBy.CreatedAt,
            bool ascending = false,
            int page = 1,
            int pageSize = 10)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email != null && u.Email.ToLower().Contains(email.ToLower()));
            }

            if (!string.IsNullOrEmpty(role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                var userIds = usersInRole.Select(u => u.Id).ToList();
                query = query.Where(u => userIds.Contains(u.Id));
            }

            var totalCount = await query.CountAsync();

            query = sortBy switch
            {
                UserSortBy.CreatedAt => ascending
                    ? query.OrderBy(u => u.CreatedAt)
                    : query.OrderByDescending(u => u.CreatedAt),
                UserSortBy.Email => ascending
                    ? query.OrderBy(u => u.Email)
                    : query.OrderByDescending(u => u.Email),
                UserSortBy.FirstName => ascending
                    ? query.OrderBy(u => u.FirstName)
                    : query.OrderByDescending(u => u.FirstName),
                UserSortBy.LastName => ascending
                    ? query.OrderBy(u => u.LastName)
                    : query.OrderByDescending(u => u.LastName),
                _ => ascending
                    ? query.OrderBy(u => u.CreatedAt)
                    : query.OrderByDescending(u => u.CreatedAt)
            };

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<(IdentityResult, User)> CreateAsync(User user, string password)
        {
            user.UserName = user.Email;
            user.CreatedAt = DateTime.UtcNow;
            return (await _userManager.CreateAsync(user, password), user);
        }

        public async Task<(IdentityResult, User)> UpdateAsync(User user)
        {
            return (await _userManager.UpdateAsync(user), user);
        }

        public async Task<IdentityResult> DeleteAsync(User user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            await EnsureRoleExistsAsync(role);
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(User user, string role)
        {
            return await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<List<string>> GetUserRoleAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        private async Task EnsureRoleExistsAsync(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}