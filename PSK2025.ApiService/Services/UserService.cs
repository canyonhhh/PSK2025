using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;

namespace PSK2025.ApiService.Services
{
    public class UserService(
        IUserRepository userRepository,
        UserManager<User> userManager,
        IMapper mapper)
        : IUserService
    {
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await userRepository.GetAllAsync();
            return mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await userRepository.GetByIdAsync(id);
            return user == null ? null : mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await userRepository.GetByEmailAsync(email);
            return user == null ? null : mapper.Map<UserDto>(user);
        }

        public async Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(RegisterDto model)
        {
            var user = mapper.Map<User>(model);
            
            var (succeeded, errors) = await userRepository.CreateAsync(user, model.Password);

            if (succeeded)
            {
                await userRepository.AddToRoleAsync(user, "customer");
            }

            return (succeeded, errors);
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(string id, UpdateUserDto model)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, ["User not found"]);

            mapper.Map(model, user);
            return await userRepository.UpdateAsync(user);
        }
        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, ["User not found"]);

            return await userRepository.DeleteAsync(user);
        }

        public async Task<(bool Succeeded, string[] Errors)> ChangePasswordAsync(string id, string currentPassword, string newPassword)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, ["User not found"]);

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<(bool Succeeded, string[] Errors)> ChangeUserRoleAsync(string id, string newRole)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, ["User not found"]);

            var currentRoles = await userRepository.GetRolesAsync(user);
            foreach (var roleName in currentRoles)
            {
                await userRepository.RemoveFromRoleAsync(user, roleName);
            }

            var result = await userRepository.AddToRoleAsync(user, newRole);

            if (result.Succeeded)
            {
                await userRepository.UpdateAsync(user);
            }

            return result;
        }
    }
}