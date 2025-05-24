using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.Data.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(
            string? userId = null,
            OrderStatus? status = null,
            OrderSortBy sortBy = OrderSortBy.CreatedAt,
            bool ascending = false,
            int page = 1,
            int pageSize = 10);
        Task<Order?> GetByIdAsync(string id);
        Task<Order> CreateAsync(Order order);
        Task<Order?> UpdateAsync(Order order);
        Task<bool> DeleteAsync(string id);

        Task<IList<ItemOrderCountDto>> GetItemOrderCountsAsync();
        Task<IList<TimeSeriesPointDto>> GetOrderCountsOverTimeAsync(DateTime from, DateTime to, TimeGrouping grouping);
        Task<IList<TimeSeriesPointDto>> GetItemOrderCountsOverTimeAsync(string productId, DateTime from, DateTime to, TimeGrouping grouping);
    }
}