using AutoMapper;
using Microsoft.Extensions.Logging;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<(OrderDto? Order, ServiceError Error)> GetOrderByIdAsync(string id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return (null, ServiceError.NotFound);
            }

            return (_mapper.Map<OrderDto>(order), ServiceError.None);
        }

        public async Task<(OrderDto? Order, ServiceError Error)> CreateOrderAsync(string userId, CreateOrderDto model)
        {
            try
            {
                // Get the cart
                var cart = await _cartRepository.GetCartAsync(model.CartId);
                
                if (cart == null || cart.Items == null || !cart.Items.Any())
                {
                    _logger.LogWarning("Cannot create order: Cart {CartId} not found or empty", model.CartId);
                    return (null, ServiceError.InvalidData);
                }

                if (cart.UserId != userId)
                {
                    _logger.LogWarning("Cannot create order: User {UserId} does not own cart {CartId}", userId, model.CartId);
                    return (null, ServiceError.Forbidden);
                }

                decimal totalPrice = 0;
                var order = new Order
                {
                    UserId = userId,
                    CartId = model.CartId,
                    ExpectedCompletionTime = model.ExpectedCompletionTime,
                    Items = new List<OrderItem>()
                };

                // Create order items from cart items
                foreach (var cartItem in cart.Items)
                {
                    var product = await _productRepository.GetByIdAsync(cartItem.ItemId);
                    
                    if (product == null)
                    {
                        _logger.LogWarning("Product with ID {ProductId} not found", cartItem.ItemId);
                        continue;
                    }

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        ProductName = product.Title,
                        ProductPrice = product.Price,
                        Quantity = cartItem.Quantity
                    };

                    order.Items.Add(orderItem);
                    totalPrice += product.Price * cartItem.Quantity;
                }

                if (!order.Items.Any())
                {
                    _logger.LogWarning("Cannot create order: No valid items found in cart {CartId}", model.CartId);
                    return (null, ServiceError.InvalidData);
                }

                order.TotalPrice = totalPrice;

                var createdOrder = await _orderRepository.CreateAsync(order);
                
                // Clear the cart after creating the order
                await _cartRepository.DeleteCartAsync(userId);

                return (_mapper.Map<OrderDto>(createdOrder), ServiceError.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user {UserId}", userId);
                return (null, ServiceError.DatabaseError);
            }
        }

        public async Task<(OrderDto? Order, ServiceError Error)> UpdateOrderStatusAsync(string id, UpdateOrderStatusDto model)
        {
            try
            {
                var existingOrder = await _orderRepository.GetByIdAsync(id);

                if (existingOrder == null)
                {
                    return (null, ServiceError.NotFound);
                }

                existingOrder.Status = model.Status;
                
                if (model.Status == OrderStatus.Completed)
                {
                    existingOrder.CompletedAt = DateTime.UtcNow;
                }

                var updatedOrder = await _orderRepository.UpdateAsync(existingOrder);

                if (updatedOrder == null)
                {
                    return (null, ServiceError.DatabaseError);
                }

                return (_mapper.Map<OrderDto>(updatedOrder), ServiceError.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status with ID {OrderId}", id);
                return (null, ServiceError.DatabaseError);
            }
        }

        public async Task<ServiceError> DeleteOrderAsync(string id)
        {
            try
            {
                var isDeleted = await _orderRepository.DeleteAsync(id);

                if (!isDeleted)
                {
                    return ServiceError.NotFound;
                }

                return ServiceError.None;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order with ID {OrderId}", id);
                return ServiceError.DatabaseError;
            }
        }

        public async Task<List<OrderDto>> GetUserOrdersSortedAsync(string userId, OrderSortBy sortBy, bool ascending = true)
        {
            try
            {
                var orders = await _orderRepository.GetUserOrdersSortedAsync(userId, sortBy, ascending);
                return _mapper.Map<List<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sorted orders for user {UserId}", userId);
                return new List<OrderDto>();
            }
        }
    }
}