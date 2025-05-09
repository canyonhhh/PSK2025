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

        public async Task<PaginatedResult<OrderDto>> GetOrdersAsync(
            string? userId = null,
            OrderStatus? status = null,
            OrderSortBy sortBy = OrderSortBy.CreatedAt,
            bool ascending = false,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                page = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 1, 50);

                var (orders, totalCount) = await _orderRepository.GetOrdersAsync(
                    userId, status, sortBy, ascending, page, pageSize);

                var orderDtos = _mapper.Map<List<OrderDto>>(orders);

                return new PaginatedResult<OrderDto>
                {
                    Items = orderDtos,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return new PaginatedResult<OrderDto>
                {
                    Items = new List<OrderDto>(),
                    TotalCount = 0,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
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

                var orderItems = new List<OrderItem>();

                foreach (var cartItem in cart.Items)
                {
                    var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                    if (product == null)
                    {
                        continue;
                    }

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
                    CreatedAt = DateTime.UtcNow,
                    ExpectedCompletionTime = model.ExpectedCompletionTime,
                    Status = OrderStatus.Pending,
                    Items = orderItems
                };

                await _orderRepository.CreateAsync(order);

                await _cartRepository.ClearCartAsync(userId);

                return (_mapper.Map<OrderDto>(order), ServiceError.None);
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
    }
}