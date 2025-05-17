using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.DTOs;

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

        public async Task<IList<ItemOrderCountDto>> GetTopOrderedItemsAsync(int topN) =>
            (await _orderRepo.GetItemOrderCountsAsync())
                .OrderByDescending(x => x.TotalQuantity)
                .Take(topN)
                .ToList();

        public async Task<IList<ItemOrderCountDto>> GetLeastOrderedItemsAsync(int bottomN) =>
            (await _orderRepo.GetItemOrderCountsAsync())
                .OrderBy(x => x.TotalQuantity)
                .Take(bottomN)
                .ToList();

        public Task<IList<TimeSeriesPointDto>> GetTotalOrdersOverTimeAsync(DateTime from, DateTime to, Models.Enums.TimeGrouping grouping) =>
            _orderRepo.GetOrderCountsOverTimeAsync(from, to, grouping);

        public Task<IList<TimeSeriesPointDto>> GetItemOrdersOverTimeAsync(string productId, DateTime from, DateTime to, Models.Enums.TimeGrouping grouping) =>
            _orderRepo.GetItemOrderCountsOverTimeAsync(productId, from, to, grouping);
    }
}
