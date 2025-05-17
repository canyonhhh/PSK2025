using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<IList<ItemOrderCountDto>> GetTopOrderedItemsAsync(int topN);
        Task<IList<ItemOrderCountDto>> GetLeastOrderedItemsAsync(int bottomN);
        Task<IList<TimeSeriesPointDto>> GetTotalOrdersOverTimeAsync(DateTime from, DateTime to, TimeGrouping grouping);
        Task<IList<TimeSeriesPointDto>> GetItemOrdersOverTimeAsync(string productId, DateTime from, DateTime to, TimeGrouping grouping);
    }
}
