using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface ICartService
    {
        Task<(List<CartDto>, ServiceError)> GetAllCartsAsync();
        Task<CartDto> GetCartAsync(Guid userId);
        Task<ServiceError> AddItemToCartAsync(Guid userId, Guid itemId, int quantity);
        Task<ServiceError> UpdateCartItemAsync(Guid userId, Guid itemId, int quantity);
        Task<ServiceError> DeleteCartItemAsync(Guid userId, Guid itemId);
        Task<ServiceError> UpdateCartAsync(Guid userId, DateTime pickupTime, CartStatus status);
    }
}
