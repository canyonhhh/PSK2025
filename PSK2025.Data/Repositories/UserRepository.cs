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

        public async Task<(IdentityResult, User)> CreateAsync(User user, string password)
        {
            user.UserName = user.Email;
            return (await userManager.CreateAsync(user, password), user);
        }

        public async Task<(IdentityResult, User)> UpdateAsync(User user)
        {
            return (await userManager.UpdateAsync(user), user);
        }

        public async Task<IdentityResult> DeleteAsync(User user)
        {
            return await userManager.DeleteAsync(user);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            await EnsureRoleExistsAsync(role);

            return await userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(User user, string role)
        {
            return await userManager.RemoveFromRoleAsync(user, role);
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