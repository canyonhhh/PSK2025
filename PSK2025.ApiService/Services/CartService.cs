using AutoMapper;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<(List<CartDto>, ServiceError)> GetAllCartsAsync()
        {
            try
            {
                var carts = await _cartRepository.GetAllCartsAsync();
                var cartDtos = _mapper.Map<List<CartDto>>(carts);
                return (cartDtos, ServiceError.None);
            }
            catch (Exception)
            {
                return (new List<CartDto>(), ServiceError.DatabaseError);
            }
        }

        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<ServiceError> AddItemToCartAsync(string userId, AddCartItemDto model)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(model.ProductId);
                if (product == null)
                    return ServiceError.NotFound;

                if (!product.IsAvailable)
                    return ServiceError.InvalidData;

                await _cartRepository.AddOrUpdateCartItemAsync(userId, model.ProductId, model.Quantity);

                return ServiceError.None;
            }
            catch (Exception)
            {
                return ServiceError.DatabaseError;
            }
        }

        public async Task<ServiceError> UpdateCartItemAsync(string userId, UpdateCartItemDto model)
        {
            try
            {
                if (model.Quantity <= 0)
                    return ServiceError.InvalidData;

                var product = await _productRepository.GetByIdAsync(model.ProductId);
                if (product == null)
                    return ServiceError.NotFound;

                await _cartRepository.AddOrUpdateCartItemAsync(userId, model.ProductId, model.Quantity);

                return ServiceError.None;
            }
            catch (Exception)
            {
                return ServiceError.DatabaseError;
            }
        }

        public async Task<ServiceError> RemoveCartItemAsync(string userId, string productId)
        {
            try
            {
                var removed = await _cartRepository.RemoveCartItemAsync(userId, productId);
                return removed ? ServiceError.None : ServiceError.NotFound;
            }
            catch (Exception)
            {
                return ServiceError.DatabaseError;
            }
        }

        public async Task<ServiceError> ClearCartAsync(string userId)
        {
            try
            {
                await _cartRepository.ClearCartAsync(userId);
                return ServiceError.None;
            }
            catch (Exception)
            {
                return ServiceError.DatabaseError;
            }
        }
    }
}