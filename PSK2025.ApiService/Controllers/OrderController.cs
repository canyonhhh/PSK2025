using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;
using PSK2025.Models.Extensions;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OrderController(IOrderService orderService, IGetUserIdService getUserIdService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly IGetUserIdService _getUserIdService = getUserIdService;


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders(
            [FromQuery] string? userId = null,
            [FromQuery] OrderStatus? status = null,
            [FromQuery] OrderSortBy sortBy = OrderSortBy.CreatedAt,
            [FromQuery] bool ascending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (userId != null && User.IsInRole("Customer") &&
                userId != _getUserIdService.GetUserIdFromToken())
            {
                return Forbid();
            }

            if (userId == null && User.IsInRole("Customer"))
            {
                userId = _getUserIdService.GetUserIdFromToken();
            }

            var result = await _orderService.GetOrdersAsync(
                userId,
                status,
                sortBy,
                ascending,
                page,
                pageSize);

            return Ok(result);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrder(string id)
        {
            var userId = _getUserIdService.GetUserIdFromToken();
            var (order, error) = await _orderService.GetOrderByIdAsync(id);

            if (error == ServiceError.None)
            {
                if (order!.UserId != userId && !User.IsInRole("Manager"))
                {
                    return Forbid();
                }

                return Ok(order);
            }

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Order"));
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _getUserIdService.GetUserIdFromToken();
            var (order, error) = await _orderService.CreateOrderAsync(userId, model);

            if (error == ServiceError.None)
                return CreatedAtAction(nameof(GetOrder), new { id = order!.Id }, order);

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Order"));
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Manager,Barista")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (order, error) = await _orderService.UpdateOrderStatusAsync(id, model);

            if (error == ServiceError.None)
                return Ok(order);

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Order"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var error = await _orderService.DeleteOrderAsync(id);

            if (error == ServiceError.None)
                return NoContent();

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Order"));
        }
    }
}