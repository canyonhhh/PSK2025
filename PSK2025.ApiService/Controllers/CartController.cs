using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services;
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
        private readonly IGetUserIdService _getUserIdService;

        public CartController(ICartService cartService, IGetUserIdService getUserIdService)
        {
            _cartService = cartService;
            _getUserIdService = getUserIdService;
        }

        private Guid GetUserIdFromToken()
        {
            return _getUserIdService.GetUserIdFromToken();
        }

        [HttpGet("all")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllCarts()
        {
            var (carts, error) = await _cartService.GetAllCartsAsync();

            if (error == ServiceError.None)
                return Ok(carts);

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Carts"));
        }
        [HttpGet]
        public async Task<IActionResult> GetActiveCart()
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

        [HttpPost("updateCart")]
        public async Task<IActionResult> UpdateCart([FromBody] UpdateCartDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var error = await _cartService.UpdateCartAsync(GetUserIdFromToken(), model.PickupTime, model.Status);
            return error == ServiceError.None ? Ok() : StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart"));
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
