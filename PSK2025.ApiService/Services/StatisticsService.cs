using Microsoft.Extensions.Logging;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSK2025.ApiService.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ILogger<StatisticsService> _logger;

        public StatisticsService(IOrderRepository orderRepo, ILogger<StatisticsService> logger)
        {
            _orderRepo = orderRepo;
            _logger = logger;
        }

        public async Task<IList<ItemOrderCountDto>> GetOrderedItemsAsync(int count, bool ascending)
        {
            var all = await _orderRepo.GetItemOrderCountsAsync();
            return ascending
                ? all.OrderBy(x => x.TotalQuantity).Take(count).ToList()
                : all.OrderByDescending(x => x.TotalQuantity).Take(count).ToList();
        }

        public Task<IList<TimeSeriesPointDto>> GetTotalOrdersOverTimeAsync(DateTime from, DateTime to, TimeGrouping grouping)
            => _orderRepo.GetOrderCountsOverTimeAsync(from, to, grouping);

        public Task<IList<TimeSeriesPointDto>> GetItemOrdersOverTimeAsync(string productId, DateTime from, DateTime to, TimeGrouping grouping)
            => _orderRepo.GetItemOrderCountsOverTimeAsync(productId, from, to, grouping);
    }
}