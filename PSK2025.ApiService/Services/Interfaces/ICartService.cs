using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface ICartService
    {
        Task<(List<CartDto>, ServiceError)> GetAllCartsAsync();
        Task<CartDto> GetCartAsync(string userId);
        Task<ServiceError> AddItemToCartAsync(string userId, AddCartItemDto model);
        Task<ServiceError> UpdateCartItemAsync(string userId, UpdateCartItemDto model);
        Task<ServiceError> DeleteCartItemAsync(string userId, string itemId);
        Task<ServiceError> UpdateCartAsync(string userId, UpdateCartDto model);
    }
}