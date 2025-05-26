using Microsoft.EntityFrameworkCore;
using PSK2025.Data.Contexts;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;

namespace PSK2025.Data.Repositories
{
    public class CartRepository(AppDbContext dbContext) : ICartRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<List<Cart>> GetAllCartsAsync()
        {
            return await _dbContext.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            var cart = await _dbContext.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Items = [],
                    User = await _dbContext.Users
                        .FirstOrDefaultAsync(u => u.Id == userId)
                };

                _dbContext.Carts.Add(cart);
                await _dbContext.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<CartItem> AddOrUpdateCartItemAsync(string userId, string productId, int quantity)
        {
            await GetCartByUserIdAsync(userId);

            var cartItem = await _dbContext.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };

                _dbContext.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
                _dbContext.CartItems.Update(cartItem);
            }

            await UpdateCartTimestampAsync(userId);

            await _dbContext.SaveChangesAsync();

            return await _dbContext.CartItems
                .Include(ci => ci.Product)
                .FirstAsync(ci => ci.Id == cartItem.Id);
        }

        public async Task<bool> RemoveCartItemAsync(string userId, string productId)
        {
            var cartItem = await _dbContext.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

            if (cartItem == null)
                return false;

            _dbContext.CartItems.Remove(cartItem);

            await UpdateCartTimestampAsync(userId);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task ClearCartAsync(string userId)
        {
            var cartItems = await _dbContext.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (cartItems.Count != 0)
            {
                _dbContext.CartItems.RemoveRange(cartItems);

                var cart = await _dbContext.Carts.FindAsync(userId);
                if (cart != null)
                {
                    cart.UpdatedAt = DateTime.UtcNow;
                    _dbContext.Carts.Update(cart);
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<CartItem?> GetCartItemAsync(string userId, string productId)
        {
            return await _dbContext.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        }

        private async Task UpdateCartTimestampAsync(string userId)
        {
            var cart = await _dbContext.Carts.FindAsync(userId);
            if (cart != null)
            {
                cart.UpdatedAt = DateTime.UtcNow;
                _dbContext.Carts.Update(cart);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
