using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(Guid userId)
        {
            var cartItems = await _cartService.GetCartAsync(userId);
            return Ok(cartItems);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddCartItemDto model)
        {
            var result = await _cartService.AddItemToCartAsync(model.UserId, model.ItemId);

            if (result == ServiceError.None)
            {
                return Ok(new { Message = "Item added to cart successfully." });
            }

            return BadRequest(new { Error = result.ToString() });
        }

        [HttpPut("update/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(Guid cartItemId, [FromBody] UpdateCartItemDto model)
        {
            var result = await _cartService.UpdateCartItemAsync(cartItemId, model.Quantity);

            if (result == ServiceError.None)
            {
                return Ok(new { Message = "Cart item updated successfully." });
            }

            return BadRequest(new { Error = result.ToString() });
        }
        [HttpDelete("delete/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartItemId, [FromQuery] Guid userId)
        {
            var result = await _cartService.DeleteCartItemAsync(cartItemId, userId);

            if (result == ServiceError.None)
            {
                return Ok(new { Message = "Cart item deleted successfully." });
            }

            return BadRequest(new { Error = result.ToString() });
        }



    }
}
