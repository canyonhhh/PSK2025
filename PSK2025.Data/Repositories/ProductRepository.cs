using Microsoft.EntityFrameworkCore;
using PSK2025.Data.Contexts;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;

namespace PSK2025.Data.Repositories
{
    public class ProductRepository(AppDbContext dbContext) : IProductRepository
    {
        public async Task<List<Product>> GetAllAsync()
        {
            return await dbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            return await dbContext.Products.FindAsync(id);
        }

        public async Task<Product> CreateAsync(Product item)
        {
            item.CreatedAt = DateTime.UtcNow;

            dbContext.Products.Add(item);
            await dbContext.SaveChangesAsync();

            return item;
        }

        public async Task<Product?> UpdateAsync(Product item)
        {
            Product? existingProduct = await dbContext.Products.FindAsync(item.Id);

            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Title = item.Title;
            existingProduct.Price = item.Price;
            existingProduct.PhotoUrl = item.PhotoUrl;
            existingProduct.Description = item.Description;
            existingProduct.IsAvailable = item.IsAvailable;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var item = await dbContext.Products.FindAsync(id);

            if (item == null)
            {
                return false;
            }

            dbContext.Products.Remove(item);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}