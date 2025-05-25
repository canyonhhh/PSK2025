using PSK2025.Models.Entities;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<(List<Product> Products, int TotalCount)> GetAllAsync(string? name, int page = 1, int pageSize = 10);
        Task<Product?> GetByIdAsync(string id);
        Task<Product> CreateAsync(Product item);
        Task<(Product? Product, bool ConcurrencyConflict, Product? ConflictingEntity)> UpdateAsync(Product item);
        Task<bool> DeleteAsync(string id);
    }
}