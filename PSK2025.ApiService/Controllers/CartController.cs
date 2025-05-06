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
    [Route("cart")]
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
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllCarts()
        {
            var (carts, error) = await _cartService.GetAllCartsAsync();

            if (error == ServiceError.None)
                return Ok(carts);

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Carts"));
        }

        [HttpGet("active")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetActiveCart()
        {
            var userId = _getUserIdService.GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("active/items")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddItemToActiveCart([FromBody] AddCartItemDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _getUserIdService.GetUserIdFromToken();
            var error = await _cartService.AddItemToCartAsync(userId, model);

            if (error == ServiceError.None)
                return Ok();

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart Item"));
        }

        [HttpPut("active")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateActiveCart([FromBody] UpdateCartDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _getUserIdService.GetUserIdFromToken();
            var error = await _cartService.UpdateCartAsync(userId, model);

            return error == ServiceError.None ? Ok() : StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart"));
        }

        [HttpPut("active/items/{itemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCartItem(string itemId, [FromBody] UpdateCartItemDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _getUserIdService.GetUserIdFromToken();
            var error = await _cartService.UpdateCartItemAsync(userId, model);

            if (error == ServiceError.None)
                return Ok();

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart Item"));
        }

        [HttpDelete("active/items/{itemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteCartItem(string itemId)
        {
            var userId = _getUserIdService.GetUserIdFromToken();
            var error = await _cartService.DeleteCartItemAsync(userId, itemId);

            if (error == ServiceError.None)
                return NoContent();

            return StatusCode(error.GetStatusCode(), error.GetErrorMessage("Cart Item"));
        }
    }

}