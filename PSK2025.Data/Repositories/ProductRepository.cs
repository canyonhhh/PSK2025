using Microsoft.EntityFrameworkCore;
using PSK2025.Data.Contexts;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;

namespace PSK2025.Data.Repositories
{
    public class ProductRepository(AppDbContext dbContext) : IProductRepository
    {
        public async Task<(List<Product> Products, int TotalCount)> GetAllAsync(string? name, int page = 1, int pageSize = 10)
        {
            var query = dbContext.Products.AsQueryable();

            var totalCount = await query.CountAsync();

            var products = await query
                .Where(p => string.IsNullOrEmpty(name) || p.Title.ToLower().Contains(name.ToLower()))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            return await dbContext.Products.FindAsync(id);
        }

        public async Task<Product> CreateAsync(Product item)
        {
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;

            dbContext.Products.Add(item);
            await dbContext.SaveChangesAsync();

            return item;
        }

        public async Task<(Product? Product, bool ConcurrencyConflict, Product? ConflictingEntity)> UpdateAsync(Product item)
        {
            try
            {
                var existingProduct = await dbContext.Products.FindAsync(item.Id);
                if (existingProduct == null)
                {
                    return (null, false, null);
                }

                dbContext.Entry(existingProduct).CurrentValues.SetValues(item);
                await dbContext.SaveChangesAsync();
                return (existingProduct, false, null);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var databaseValues = await entry.GetDatabaseValuesAsync();

                if (databaseValues == null)
                {
                    return (null, true, null);
                }

                var databaseEntity = (Product)databaseValues.ToObject();
                return (null, true, databaseEntity);
            }
            catch (Exception)
            {
                return (null, false, null);
            }
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