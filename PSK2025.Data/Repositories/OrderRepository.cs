using Microsoft.EntityFrameworkCore;
using PSK2025.Data.Contexts;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.Data.Repositories
{
    public class OrderRepository(AppDbContext dbContext) : IOrderRepository
    {
        public async Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(
            string? userId = null,
            OrderStatus? status = null,
            OrderSortBy sortBy = OrderSortBy.CreatedAt,
            bool ascending = false,
            int page = 1,
            int pageSize = 10)
        {
            var query = dbContext.Orders
                .Include(o => o.Items)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.UserId == userId);
            }

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            query = sortBy switch
            {
                OrderSortBy.CreatedAt => ascending
                    ? query.OrderBy(o => o.CreatedAt)
                    : query.OrderByDescending(o => o.CreatedAt),
                OrderSortBy.ExpectedCompletionTime => ascending
                    ? query.OrderBy(o => o.ExpectedCompletionTime)
                    : query.OrderByDescending(o => o.ExpectedCompletionTime),
                OrderSortBy.TotalPrice => ascending
                    ? query.OrderBy(o => o.Items.Sum(i => i.ProductPrice * i.Quantity))
                    : query.OrderByDescending(o => o.Items.Sum(i => i.ProductPrice * i.Quantity)),
                _ => ascending
                    ? query.OrderBy(o => o.CreatedAt)
                    : query.OrderByDescending(o => o.CreatedAt)
            };

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<Order?> GetByIdAsync(string id)
        {
            return await dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            order.CreatedAt = DateTime.UtcNow;
            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateAsync(Order order)
        {
            var existingOrder = await dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == order.Id);
            if (existingOrder == null)
            {
                return null;
            }
            existingOrder.Status = order.Status;
            existingOrder.CompletedAt = order.Status == OrderStatus.Completed ? DateTime.UtcNow : order.CompletedAt;
            existingOrder.ExpectedCompletionTime = order.ExpectedCompletionTime;
            await dbContext.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var order = await dbContext.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }
            dbContext.Orders.Remove(order);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}