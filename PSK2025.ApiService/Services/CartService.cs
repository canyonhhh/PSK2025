using AutoMapper;
using Microsoft.Extensions.Logging;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Migrations;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ICartService> _logger;

        public CartService(ICartRepository cartRepository, IMapper mapper, ILogger<ICartService> logger)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CartItemDto>> GetCartAsync(Guid userId)
        {
            var cartItems = await _cartRepository.GetCartItemsAsync(userId);
            return _mapper.Map<List<CartItemDto>>(cartItems);
        }

        public async Task<ServiceError> AddItemToCartAsync(Guid userId, Guid itemId)
        {
            try
            {
                var existingItem = await _cartRepository.GetCartItemAsync(userId, itemId);

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                    await _cartRepository.UpdateAsync(existingItem);
                }
                else
                {
                    var newItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        ItemId = itemId,
                        Quantity = 1
                    };
                    await _cartRepository.AddItemAsync(newItem);
                }

                await _cartRepository.SaveChangesAsync();
                return ServiceError.None;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding item to cart: {ex.Message}");
                return ServiceError.DatabaseError;
            }
        }

        public async Task<ServiceError> UpdateCartItemAsync(Guid cartItemId, int quantity)
        {
            var existingItem = await _cartRepository.GetCartItemAsync(cartItemId, cartItemId);

            if (existingItem == null)
            {
                return ServiceError.NotFound;
            }

            existingItem.Quantity = quantity;
            await _cartRepository.UpdateAsync(existingItem);
            return ServiceError.None;
        }

        public async Task<ServiceError> DeleteCartItemAsync(Guid cartItemId, Guid userId)
        {
            var item = await _cartRepository.GetCartItemAsync(userId, cartItemId);

            if (item == null)
            {
                return ServiceError.NotFound;
            }

            var isDeleted = await _cartRepository.DeleteAsync(userId, cartItemId);

            if (!isDeleted)
            {
                return ServiceError.NotFound;
            }

            return ServiceError.None;
        }

    }
}
