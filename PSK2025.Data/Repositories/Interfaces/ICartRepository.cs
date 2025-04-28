using PSK2025.Data.Migrations;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<List<Cart>> GetAllCartsAsync();
        Task<Cart?> GetCartAsync(Guid userId);
        Task CreateCartAsync(Cart cart);
        Task AddItemToCartAsync(Guid cartId, CartItem cartItem);
        Task UpdateCartAsync(Cart cart);
        Task<bool> RemoveItemFromCartAsync(Guid cartId, Guid itemId);
        Task SaveChangesAsync();
        Task<(Cart cart, ServiceError error)> GetOrCreateCartAsync(Guid userId);
    }
}
