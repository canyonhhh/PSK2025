using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItemDto>> GetCartAsync(Guid userId);
        Task<ServiceError> AddItemToCartAsync(Guid userId, Guid itemId);
        Task<ServiceError> UpdateCartItemAsync(Guid cartItemId, int quantity);

        Task<ServiceError> DeleteCartItemAsync(Guid cartItemId, Guid userId);
    }
}
