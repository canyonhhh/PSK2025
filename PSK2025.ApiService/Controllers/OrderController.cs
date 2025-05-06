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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IGetUserIdService _getUserIdService;

        public OrderController(IOrderService orderService, IGetUserIdService getUserIdService)
        {
            _orderService = orderService;
            _getUserIdService = getUserIdService;
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("user")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUserOrders(
            [FromQuery] OrderSortBy sortBy = OrderSortBy.CreatedAt, 
            [FromQuery] bool ascending = false)
        {
            var userId = _getUserIdService.GetUserIdFromToken();
            var orders = await _orderService.GetUserOrdersSortedAsync(userId, sortBy, ascending);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrder(string id)
        {
            var userId = _getUserIdService.GetUserIdFromToken();
            var (order, error) = await _orderService.GetOrderByIdAsync(id);

            if (error == ServiceError.None)
            {
                // Ensure users can only see their own orders unless they're managers
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