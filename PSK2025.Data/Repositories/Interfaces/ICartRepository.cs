using PSK2025.Data.Migrations;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<List<Cart>> GetAllCartsAsync();
        Task<Cart?> GetCartAsync(string userId);
        Task CreateCartAsync(string cart);
        Task AddItemToCartAsync(string cartId, CartItem cartItem);
        Task UpdateCartAsync(Cart cart);
        Task<bool> RemoveItemFromCartAsync(string cartId, string itemId);
        Task SaveChangesAsync();
    }
}
