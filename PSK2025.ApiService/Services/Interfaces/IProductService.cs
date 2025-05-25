using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedResult<ProductDto>> GetAllProductsAsync(string? name, int page = 1, int pageSize = 10);
        Task<(ProductDto? Product, ServiceError Error)> GetProductByIdAsync(string id);
        Task<(ProductDto? Product, ServiceError Error)> CreateProductAsync(CreateProductDto model);
        Task<(ProductDto? Product, ServiceError Error, ProductDto? ConflictingEntity)> UpdateProductAsync(string id, UpdateProductDto model);
        Task<(ProductDto? Product, ServiceError Error, ProductDto? ConflictingEntity)> UpdateProductAvailabilityAsync(string id, UpdateProductAvailabilityDto model);
        Task<ServiceError> DeleteProductAsync(string id);
    }
}