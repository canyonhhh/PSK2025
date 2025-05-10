using AutoMapper;
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

        public async Task<(ProductDto? Product, ServiceError Error)> UpdateProductAsync(string id, UpdateProductDto model)
        {
            var existingProduct = await productRepository.GetByIdAsync(id);

            if (existingProduct == null)
            {
                return (null, ServiceError.NotFound);
            }

            try
            {
                mapper.Map(model, existingProduct);
                var updatedProduct = await productRepository.UpdateAsync(existingProduct);

                if (updatedProduct == null)
                {
                    return (null, ServiceError.DatabaseError);
                }

                return (mapper.Map<ProductDto>(updatedProduct), ServiceError.None);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error updating product with ID {id}: {ex.Message}");
                return (null, ServiceError.DatabaseError);
            }
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