using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAsync(string id);
        Task<Order> CreateAsync(Order order);
        Task<Order?> UpdateAsync(Order order);
        Task<bool> DeleteAsync(string id);
        Task<List<Order>> GetUserOrdersSortedAsync(string userId, OrderSortBy sortBy, bool ascending = true);
        Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
    }
}