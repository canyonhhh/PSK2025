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

        public async Task<(bool Succeeded, UserDto? CreatedUser, string[] Errors)> RegisterUserAsync(RegisterDto model)
        {
            var user = mapper.Map<User>(model);

            var (result, createdUser) = await userRepository.CreateAsync(user, model.Password);

            var createdUserDto = mapper.Map<UserDto>(createdUser);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(createdUser, "Customer");
                return (true, createdUserDto, []);
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return (false, null, errors);
        }

        public async Task<(bool Succeeded, UserDto? UpdatedUser, string[] Errors)> UpdateUserAsync(string id, UpdateUserDto model)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, null, ["User not found"]);

            mapper.Map(model, user);
            var (result, updatedUser) = await userRepository.UpdateAsync(user);

            var updatedUserDto = mapper.Map<UserDto>(updatedUser);

            if (result.Succeeded)
            {
                return (true, updatedUserDto, []);
            }
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return (false, null, errors);
        }
        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, ["User not found"]);

            var result = await userRepository.DeleteAsync(user);

            if (result.Succeeded)
            {
                return (true, []);
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return (false, errors);
        }

        public async Task<(bool Succeeded, UserDto? UpdatedUser, string[] Errors)> ChangeUserRoleAsync(string id, string newRole)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, null, ["User not found"]);

            var currentRoles = await userRepository.GetRolesAsync(user);
            foreach (var roleName in currentRoles)
            {
                await userRepository.RemoveFromRoleAsync(user, roleName);
            }

            await userRepository.AddToRoleAsync(user, newRole);

            var (result, updatedUser) = await userRepository.UpdateAsync(user);

            var updatedUserDto = mapper.Map<UserDto>(updatedUser);

            if (result.Succeeded)
            {
                return (true, updatedUserDto, []);
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return (false, null, errors);
        }
    }
}