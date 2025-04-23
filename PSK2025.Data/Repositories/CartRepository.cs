using Microsoft.EntityFrameworkCore;
using PSK2025.Data.Contexts;
using PSK2025.Data.Migrations;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;

namespace PSK2025.Data.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _dbContext;

        public CartRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddItemAsync(CartItem cartItem)
        {
            await _dbContext.CartItems.AddAsync(cartItem);
        }

        public async Task<CartItem?> GetCartItemAsync(Guid userId, Guid itemId)
        {
            return await _dbContext.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ItemId == itemId);
        }

        public async Task<List<CartItem>> GetCartItemsAsync(Guid userId)
        {
            return await _dbContext.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }


        public async Task<CartItem?> UpdateAsync(CartItem cartItem)
        {
            var existingItem = await _dbContext.CartItems.FindAsync(cartItem.Id);

            if (existingItem == null)
            {
                return null;
            }

            existingItem.Quantity = cartItem.Quantity;
            await _dbContext.SaveChangesAsync();

            return existingItem;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid itemId)
        {
            var item = await _dbContext.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ItemId == itemId);

            if (item == null)
            {
                return false;
            }

            _dbContext.CartItems.Remove(item);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
