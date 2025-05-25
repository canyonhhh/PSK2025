using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services
{
    public class UserService(
        IUserRepository userRepository,
        UserManager<User> userManager,
        IMapper mapper,
        ILogger<UserService> logger)
        : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UserService> _logger = logger;

        public async Task<PaginatedResult<UserDto>> GetAllUsersAsync(
            string? email = null,
            string? role = null,
            UserSortBy sortBy = UserSortBy.CreatedAt,
            bool ascending = false,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                page = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 1, 50);

                var (users, totalCount) = await _userRepository.GetAllAsync(
                    email, role, sortBy, ascending, page, pageSize);

                var userDtos = _mapper.Map<List<UserDto>>(users);

                return new PaginatedResult<UserDto>
                {
                    Items = userDtos,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return new PaginatedResult<UserDto>
                {
                    Items = new List<UserDto>(),
                    TotalCount = 0,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
        }

        public async Task<(UserDto? User, ServiceError Error)> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return (null, ServiceError.NotFound);
                }

                return (_mapper.Map<UserDto>(user), ServiceError.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
                return (null, ServiceError.DatabaseError);
            }
        }

        public async Task<(UserDto? User, ServiceError Error)> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return (null, ServiceError.NotFound);
                }

                return (_mapper.Map<UserDto>(user), ServiceError.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with email {Email}", email);
                return (null, ServiceError.DatabaseError);
            }
        }

        public async Task<(UserDto? User, ServiceError Error)> RegisterUserAsync(RegisterDto model)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return (null, ServiceError.AlreadyExists);
                }

                var user = _mapper.Map<User>(model);
                var (result, createdUser) = await _userRepository.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to create user {Email}: {Errors}",
                        model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return (null, ServiceError.InvalidData);
                }

                await _userManager.AddToRoleAsync(createdUser, "Customer");
                var createdUserDto = _mapper.Map<UserDto>(createdUser);

                return (createdUserDto, ServiceError.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email {Email}", model.Email);
                return (null, ServiceError.DatabaseError);
            }
        }

        public async Task<(UserDto? User, ServiceError Error)> UpdateUserAsync(string id, UpdateUserDto model)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return (null, ServiceError.NotFound);
                }

                _mapper.Map(model, user);
                var (result, updatedUser) = await _userRepository.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to update user {UserId}: {Errors}",
                        id, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return (null, ServiceError.InvalidData);
                }

                var updatedUserDto = _mapper.Map<UserDto>(updatedUser);
                return (updatedUserDto, ServiceError.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}", id);
                return (null, ServiceError.DatabaseError);
            }
        }

        public async Task<ServiceError> DeleteUserAsync(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ServiceError.NotFound;
                }

                var result = await _userRepository.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to delete user {UserId}: {Errors}",
                        id, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return ServiceError.DatabaseError;
                }

                return ServiceError.None;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
                return ServiceError.DatabaseError;
            }
        }

        public async Task<(UserDto? User, ServiceError Error)> ChangeUserRoleAsync(string id, string newRole)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return (null, ServiceError.NotFound);
                }

                var validRoles = Enum.GetNames<UserRole>();
                if (!validRoles.Contains(newRole))
                {
                    return (null, ServiceError.InvalidData);
                }

                var currentRoles = await _userRepository.GetRolesAsync(user);
                foreach (var roleName in currentRoles)
                {
                    await _userRepository.RemoveFromRoleAsync(user, roleName);
                }

                await _userRepository.AddToRoleAsync(user, newRole);

                var (result, updatedUser) = await _userRepository.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to change role for user {UserId}: {Errors}",
                        id, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return (null, ServiceError.DatabaseError);
                }

                var updatedUserDto = _mapper.Map<UserDto>(updatedUser);
                return (updatedUserDto, ServiceError.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing role for user with ID {UserId}", id);
                return (null, ServiceError.DatabaseError);
            }
        }
    }
}