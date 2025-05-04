using Microsoft.EntityFrameworkCore;
using PSK2025.Data.Contexts;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _dbContext;

    public CartRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<Cart>> GetAllCartsAsync()
    {
        return await _dbContext.Carts
            .Include(c => c.Items)
            .ToListAsync();
    }
    public async Task<Cart?> GetCartAsync(Guid userId)
    {
        return await _dbContext.Carts
            .Include(c => c.Items) 
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task CreateCartAsync(Cart cart)
    {
        await _dbContext.Carts.AddAsync(cart);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddItemToCartAsync(Guid cartId, CartItem cartItem)
    {
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId);

        if (cart == null)
        {
            throw new InvalidOperationException("Cart not found.");
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ItemId == cartItem.ItemId);
        if(cartItem.Quantity > 0)
        {
            if (existingItem != null)
            {
                existingItem.Quantity = cartItem.Quantity; 
            }
            else
            {
                cartItem.Id = Guid.Empty;
                cart.Items.Add(cartItem);
            }
        }
        
        cart.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCartAsync(Cart cart)
    {
        _dbContext.Carts.Update(cart);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> RemoveItemFromCartAsync(Guid cartId, Guid itemId)
    {
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId);

        if (cart == null)
        {
            return false;
        }

        var item = cart.Items.FirstOrDefault(i => i.ItemId == itemId);
        if (item == null)
        {
            return false;
        }

        cart.Items.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return true;
    }


    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
