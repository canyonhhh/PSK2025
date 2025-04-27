using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;
using PSK2025.Models.Extensions;
using System.Security.Claims;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }
            return Guid.Parse(userIdClaim);
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserIdFromToken();
            var cart = await _cartService.GetCartAsync(userId);

            if (cart == null)
                return StatusCode(ServiceError.NotFound.GetStatusCode(), ServiceError.NotFound.GetErrorMessage("Cart"));

            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddCartItemDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserIdFromToken();
            var error = await _cartService.AddItemToCartAsync(userId, model.ItemId, model.Quantity);

            if (error == ServiceError.None)
                return Ok();

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart Item"));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromQuery] Guid itemId, [FromBody] UpdateCartItemDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserIdFromToken();
            var error = await _cartService.UpdateCartItemAsync(userId, itemId, model.Quantity);

            if (error == ServiceError.None)
                return Ok();

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart Item"));
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCartItem([FromQuery] Guid itemId)
        {
            var userId = GetUserIdFromToken();
            var error = await _cartService.DeleteCartItemAsync(userId, itemId);

            if (error == ServiceError.None)
                return NoContent();

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart Item"));
        }
    }
}
