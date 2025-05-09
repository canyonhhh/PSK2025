using PSK2025.Models.Entities;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<List<Cart>> GetAllCartsAsync();
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<CartItem> AddOrUpdateCartItemAsync(string userId, string productId, int quantity);
        Task<bool> RemoveCartItemAsync(string userId, string productId);
        Task ClearCartAsync(string userId);
        Task<CartItem?> GetCartItemAsync(string userId, string productId);
    }
}