using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PaginatedResult<OrderDto>> GetOrdersAsync(
            string? userId = null,
            OrderStatus? status = null,
            OrderSortBy sortBy = OrderSortBy.CreatedAt,
            bool ascending = false,
            int page = 1,
            int pageSize = 10);
        Task<(OrderDto? Order, ServiceError Error)> GetOrderByIdAsync(string id);
        Task<(OrderDto? Order, ServiceError Error)> CreateOrderAsync(string userId, CreateOrderDto model);
        Task<(OrderDto? Order, ServiceError Error)> UpdateOrderStatusAsync(string id, UpdateOrderStatusDto model);
        Task<ServiceError> DeleteOrderAsync(string id);
    }
}