using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedResult<ProductDto>> GetAllProductsAsync(int page = 1, int pageSize = 10);
        Task<(ProductDto? Product, ServiceError Error)> GetProductByIdAsync(string id);
        Task<(ProductDto? Product, ServiceError Error)> CreateProductAsync(CreateProductDto model);
        Task<(ProductDto? Product, ServiceError Error)> UpdateProductAsync(string id, UpdateProductDto model);
        Task<ServiceError> DeleteProductAsync(string id);
        Task<(ProductDto? Product, ServiceError Error)> UpdateProductAvailabilityAsync(string id, UpdateProductAvailabilityDto model);
    }
}