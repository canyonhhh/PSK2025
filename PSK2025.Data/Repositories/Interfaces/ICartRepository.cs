using PSK2025.Data.Migrations;
using PSK2025.Models.Entities;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task AddItemAsync(CartItem cartItem);
        Task<CartItem?> GetCartItemAsync(Guid userId, Guid itemId);
        Task<List<CartItem>> GetCartItemsAsync(Guid userId);
        Task<CartItem?> UpdateAsync(CartItem cartItem);
        Task<bool> DeleteAsync(Guid userId, Guid itemId);
        Task SaveChangesAsync();
    }
}
