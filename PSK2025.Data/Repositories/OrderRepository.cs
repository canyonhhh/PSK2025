using Microsoft.EntityFrameworkCore;
using PSK2025.Data.Contexts;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;
using PSK2025.Models.Enums;

namespace PSK2025.Data.Repositories
{
    public class OrderRepository(AppDbContext dbContext) : IOrderRepository
    {
        public async Task<List<Order>> GetAllAsync()
        {
            return await dbContext.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByUserIdAsync(string userId)
        {
            return await dbContext.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
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

        public async Task<List<Order>> GetUserOrdersSortedAsync(string userId, OrderSortBy sortBy, bool ascending = true)
        {
            var query = dbContext.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .AsQueryable();

            query = sortBy switch
            {
                OrderSortBy.CreatedAt => ascending
                    ? query.OrderBy(o => o.CreatedAt)
                    : query.OrderByDescending(o => o.CreatedAt),
                OrderSortBy.ExpectedCompletionTime => ascending
                    ? query.OrderBy(o => o.ExpectedCompletionTime)
                    : query.OrderByDescending(o => o.ExpectedCompletionTime),
                OrderSortBy.TotalPrice => ascending
                    ? query.OrderBy(o => o.TotalPrice)
                    : query.OrderByDescending(o => o.TotalPrice),
                _ => ascending
                    ? query.OrderBy(o => o.CreatedAt)
                    : query.OrderByDescending(o => o.CreatedAt)
            };

            return await query.ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await dbContext.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}