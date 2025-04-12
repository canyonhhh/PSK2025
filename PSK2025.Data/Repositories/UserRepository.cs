using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;

namespace PSK2025.Data.Repositories
{
    public class UserRepository(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
        : IUserRepository
    {
        public async Task<User?> GetByIdAsync(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await userManager.Users.ToListAsync();
        }

        public async Task<(bool Succeeded, string[] Errors)> CreateAsync(User user, string password)
        {
            var result = await userManager.CreateAsync(user, password);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateAsync(User user)
        {
            var result = await userManager.UpdateAsync(user);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<(bool Succeeded, string[] Errors)> DeleteAsync(User user)
        {
            var result = await userManager.DeleteAsync(user);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task<(bool Succeeded, string[] Errors)> AddToRoleAsync(User user, string role)
        {
            await EnsureRoleExistsAsync(role);

            var result = await userManager.AddToRoleAsync(user, role);

            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<(bool Succeeded, string[] Errors)> RemoveFromRoleAsync(User user, string role)
        {
            var result = await userManager.RemoveFromRoleAsync(user, role);

            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            return await userManager.IsInRoleAsync(user, role);
        }

        public async Task<List<string>> GetUserRoleAsync(User user)
        {
            var roles = await userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        private async Task EnsureRoleExistsAsync(string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}