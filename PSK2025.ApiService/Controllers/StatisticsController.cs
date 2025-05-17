using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Manager")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _stats;

        public StatisticsController(IStatisticsService stats)
        {
            _stats = stats;
        }

        [HttpGet("ordered-items")]
        public async Task<IActionResult> GetItems(
            [FromQuery] int count = 5,
            [FromQuery] string direction = "descending")
        {
            bool ascending = direction.Equals("ascending", StringComparison.OrdinalIgnoreCase);
            var result = await _stats.GetOrderedItemsAsync(count, ascending);
            return Ok(result);
        }

        [HttpGet("orders-over-time")]
        public async Task<IActionResult> GetOrdersOverTime(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] TimeGrouping grouping = TimeGrouping.Daily)
        {
            var end = to ?? DateTime.UtcNow;
            var start = from ?? end.AddDays(-30);
            return Ok(await _stats.GetTotalOrdersOverTimeAsync(start, end, grouping));
        }

        [HttpGet("item/{productId}/over-time")]
        public async Task<IActionResult> GetItemOverTime(
            string productId,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] TimeGrouping grouping = TimeGrouping.Daily)
        {
            var end = to ?? DateTime.UtcNow;
            var start = from ?? end.AddDays(-30);
            return Ok(await _stats.GetItemOrdersOverTimeAsync(productId, start, end, grouping));
        }
    }
}
