using AutoMapper;
using Microsoft.Extensions.Logging;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories;
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

        public CartService(ICartRepository cartRepository, IMapper mapper, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<(List<CartDto>, ServiceError)> GetAllCartsAsync()
        {
            try
            {
                var carts = await _cartRepository.GetAllCartsAsync();
                var cartsDto = _mapper.Map<List<CartDto>>(carts);
                return (cartsDto, ServiceError.None);
            }
            catch (Exception)
            {
                return (new List<CartDto>(), ServiceError.DatabaseError);
            }
        }

        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartAsync(userId);
            if (cart == null)
            {
                await _cartRepository.CreateCartAsync(userId);

                cart = await _cartRepository.GetCartAsync(userId)!;
            }

            cart.Items = cart.Items ?? new List<CartItem>();

            var cartDto = _mapper.Map<CartDto>(cart);

            foreach (var cartItem in cartDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ItemId);
                if (product != null)
                {
                    cartItem.ProductName = product.Title;  
                    cartItem.Price = product.Price;
                }
            }

            return cartDto;
        }

        public async Task<ServiceError> AddItemToCartAsync(string userId, AddCartItemDto model)
        {
            try
            {
                var cart = await _cartRepository.GetCartAsync(userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        Status = CartStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        Items = new List<CartItem>()
                    };
                    await _cartRepository.CreateCartAsync(cart.Id);
                }

                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ItemId = model.ItemId,
                    Quantity = model.Quantity
                };

                await _cartRepository.AddItemToCartAsync(cart.Id, cartItem);
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
                var cart = await _cartRepository.GetCartAsync(userId);
                if (cart == null || cart.Items == null)
                {
                    return ServiceError.NotFound;
                }

                var cartItem = cart.Items.FirstOrDefault(i => i.ItemId == model.ItemId);

                if (cartItem == null)
                {
                    return ServiceError.NotFound;
                }
                if (model.Quantity <= 0)
                {
                    return ServiceError.InvalidData;
                }

                cartItem.Quantity = model.Quantity;
                cart.UpdatedAt = DateTime.UtcNow;

                await _cartRepository.UpdateCartAsync(cart);
                return ServiceError.None;
            }
            catch (Exception)
            {
                return ServiceError.DatabaseError;
            }
        }

        public async Task<ServiceError> UpdateCartAsync(string userId, UpdateCartDto model)
        {
            try
            {
                var cart = await _cartRepository.GetCartAsync(userId);

                if (cart == null)
                {
                    return ServiceError.NotFound;
                }

                cart.PickupTime = model.PickupTime;
                cart.Status = model.Status;
                cart.UpdatedAt = DateTime.UtcNow;

                await _cartRepository.UpdateCartAsync(cart);
                return ServiceError.None;
            }
            catch (Exception)
            {
                return ServiceError.DatabaseError;
            }
        }

        public async Task<ServiceError> DeleteCartItemAsync(string userId, string itemId)
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
            catch (Exception)
            {
                return ServiceError.DatabaseError;
            }
        }
    }
}
