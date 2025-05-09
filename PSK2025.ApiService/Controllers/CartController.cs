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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IGetUserIdService _getUserIdService;

        public CartController(ICartService cartService, IGetUserIdService getUserIdService)
        {
            _cartService = cartService;
            _getUserIdService = getUserIdService;
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCart()
        {
            var userId = _getUserIdService.GetUserIdFromToken();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _getUserIdService.GetUserIdFromToken();
            var error = await _cartService.AddItemToCartAsync(userId, model);

            if (error == ServiceError.None)
                return Ok();

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart Item"));
        }

        [HttpPut("items")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _getUserIdService.GetUserIdFromToken();
            var error = await _cartService.UpdateCartItemAsync(userId, model);

            if (error == ServiceError.None)
                return Ok();

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart Item"));
        }

        [HttpDelete("items/{productId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveItem(string productId)
        {
            var userId = _getUserIdService.GetUserIdFromToken();
            var error = await _cartService.RemoveCartItemAsync(userId, productId);

            if (error == ServiceError.None)
                return NoContent();

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart Item"));
        }

        [HttpDelete("items")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ClearItems()
        {
            var userId = _getUserIdService.GetUserIdFromToken();
            var error = await _cartService.ClearCartAsync(userId);

            if (error == ServiceError.None)
                return NoContent();

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart"));
        }
    }
}