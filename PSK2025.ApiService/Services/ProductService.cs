using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services
{
    public class ProductService(IProductRepository productRepository, IMapper mapper, ILogger<IProductService> logger) : IProductService
    {
        public async Task<PaginatedResult<ProductDto>> GetAllProductsAsync(int page = 1, int pageSize = 10)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var (products, totalCount) = await productRepository.GetAllAsync(page, pageSize);
            var productDtos = mapper.Map<List<ProductDto>>(products);

            return new PaginatedResult<ProductDto>
            {
                Items = productDtos,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task<(ProductDto? Product, ServiceError Error)> GetProductByIdAsync(string id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return (null, ServiceError.NotFound);
            }

            return (mapper.Map<ProductDto>(product), ServiceError.None);
        }

        public async Task<(ProductDto? Product, ServiceError Error)> CreateProductAsync(CreateProductDto model)
        {
            try
            {
                var product = mapper.Map<Product>(model);
                var createdProduct = await productRepository.CreateAsync(product);
                return (mapper.Map<ProductDto>(createdProduct), ServiceError.None);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error creating product: {ex.Message}");
                return (null, ServiceError.DatabaseError);
            }
        }

        private async Task<(ProductDto? Product, ServiceError Error, ProductDto? ConflictingEntity)> UpdateProductPropertyAsync(
            string id,
            Action<Product> updateAction)
        {
            var existingProduct = await productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return (null, ServiceError.NotFound, null);
            }

            try
            {
                updateAction(existingProduct);

                existingProduct.UpdatedAt = DateTime.UtcNow;

                var (updatedProduct, concurrencyConflict, conflictingEntity) = await productRepository.UpdateAsync(existingProduct);

                if (concurrencyConflict)
                {
                    return (null, ServiceError.ConcurrencyError,
                           conflictingEntity != null ? mapper.Map<ProductDto>(conflictingEntity) : null);
                }

                if (updatedProduct == null)
                {
                    return (null, ServiceError.DatabaseError, null);
                }

                return (mapper.Map<ProductDto>(updatedProduct), ServiceError.None, null);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error updating product with ID {id}: {ex.Message}");
                return (null, ServiceError.DatabaseError, null);
            }
        }

        public async Task<(ProductDto? Product, ServiceError Error, ProductDto? ConflictingEntity)> UpdateProductAsync(string id, UpdateProductDto model)
        {
            return await UpdateProductPropertyAsync(id, product => mapper.Map(model, product));
        }

        public async Task<(ProductDto? Product, ServiceError Error, ProductDto? ConflictingEntity)> UpdateProductAvailabilityAsync(string id, UpdateProductAvailabilityDto model)
        {
            return await UpdateProductPropertyAsync(id, product => product.IsAvailable = model.IsAvailable);
        }

        public async Task<ServiceError> DeleteProductAsync(string id)
        {
            var isDeleted = await productRepository.DeleteAsync(id);

            if (!isDeleted)
            {
                return ServiceError.NotFound;
            }

            return ServiceError.None;
        }
    }
}