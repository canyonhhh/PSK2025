using AutoMapper;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services
{
    public class OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<OrderService> logger) : IOrderService
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly ICartRepository _cartRepository = cartRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<OrderService> _logger = logger;

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
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);

                if (cart == null || cart.Items.Count == 0)
                {
                    return (null, ServiceError.InvalidData);
                }

                decimal totalPrice = 0;
                var orderItems = new List<OrderItem>();

                foreach (var cartItem in cart.Items)
                {
                    var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                    if (product == null)
                    {
                        continue;
                    }

                    totalPrice += product.Price * cartItem.Quantity;

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Title,
                        ProductPrice = product.Price,
                        Quantity = cartItem.Quantity
                    };

                    orderItems.Add(orderItem);
                }

                if (orderItems.Count == 0)
                {
                    return (null, ServiceError.InvalidData);
                }

                var order = new Order
                {
                    UserId = userId,
                    TotalPrice = totalPrice,
                    CreatedAt = DateTime.UtcNow,
                    ExpectedCompletionTime = model.ExpectedCompletionTime,
                    Status = OrderStatus.Pending,
                    Items = orderItems
                };

                await _orderRepository.CreateAsync(order);

                await _cartRepository.ClearCartAsync(userId);

                return (_mapper.Map<OrderDto>(order), ServiceError.None);
            }
            catch (Exception)
            {
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
                return [];
            }
        }

        public async Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByStatusAsync(status);
                return _mapper.Map<List<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders with status {Status}", status);
                return [];
            }
        }
    }
}