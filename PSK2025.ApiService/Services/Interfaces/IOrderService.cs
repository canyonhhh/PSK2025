using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<List<OrderDto>> GetUserOrdersAsync(string userId);
        Task<(OrderDto? Order, ServiceError Error)> GetOrderByIdAsync(string id);
        Task<(OrderDto? Order, ServiceError Error)> CreateOrderAsync(string userId, CreateOrderDto model);
        Task<(OrderDto? Order, ServiceError Error)> UpdateOrderStatusAsync(string id, UpdateOrderStatusDto model);
        Task<ServiceError> DeleteOrderAsync(string id);
        Task<List<OrderDto>> GetUserOrdersSortedAsync(string userId, OrderSortBy sortBy, bool ascending = true);
    }
}