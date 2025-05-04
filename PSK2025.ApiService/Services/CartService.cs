using AutoMapper;
using Microsoft.Extensions.Logging;
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
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }
        public async Task<(List<CartDto>, ServiceError)> GetAllCartsAsync()
        {
            try
            {
                var carts = await _cartRepository.GetAllCartsAsync();
                var cartsDto = _mapper.Map<List<CartDto>>(carts);
                return (cartsDto, ServiceError.None);
            }
            catch (Exception ex)
            {
                return (new List<CartDto>(), ServiceError.DatabaseError);
            }
        }

        public async Task<CartDto> GetCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartAsync(userId);

            if (cart == null)
            {
                return null; 
            }

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<ServiceError> AddItemToCartAsync(Guid userId, Guid itemId, int quantity)
        {
            try
            {
                var cart = await _cartRepository.GetCartAsync(userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Status = CartStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        Items = new List<CartItem>()
                    };

                    await _cartRepository.CreateCartAsync(cart);
                }
                cart = await _cartRepository.GetCartAsync(userId);
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ItemId = itemId,
                    Quantity = quantity
                };

                await _cartRepository.AddItemToCartAsync(cart.Id, cartItem);
                return ServiceError.None;
            }
            catch (Exception ex)
            {
                return ServiceError.DatabaseError;
            }
        }

        public async Task<ServiceError> UpdateCartItemAsync(Guid userId, Guid itemId, int quantity)
        {
            try
            {
                var cart = await _cartRepository.GetCartAsync(userId);

                if (cart == null || cart.Items == null)
                {
                    return ServiceError.NotFound;
                }

                var cartItem = cart.Items.FirstOrDefault(i => i.ItemId == itemId);

                if (cartItem == null)
                {
                    return ServiceError.NotFound;
                }
                if (quantity <= 0)
                {
                    return ServiceError.InvalidData;
                }

                cartItem.Quantity = quantity;
                cart.UpdatedAt = DateTime.UtcNow;

                await _cartRepository.UpdateCartAsync(cart);
                return ServiceError.None;
            }
            catch (Exception ex)
            {
                return ServiceError.DatabaseError;
            }
        }
        public async Task<ServiceError> UpdateCartAsync(Guid userId, DateTime pickupTime, CartStatus status)
        {
            var cart = await _cartRepository.GetCartAsync(userId);

            if (cart == null) return ServiceError.NotFound;

            cart.PickupTime = pickupTime;
            cart.Status = status;
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateCartAsync(cart);
            return ServiceError.None;
        }
        public async Task<ServiceError> DeleteCartItemAsync(Guid userId, Guid itemId)
        {
            try
            {
                var cart = await _cartRepository.GetCartAsync(userId);

                if (cart == null)
                {
                    return ServiceError.NotFound;
                }

                var isDeleted = await _cartRepository.RemoveItemFromCartAsync(cart.Id, itemId);

                if (!isDeleted)
                {
                    return ServiceError.NotFound;
                }

                return ServiceError.None;
            }
            catch (Exception ex)
            {
                return ServiceError.DatabaseError;
            }
        }
    }
}
