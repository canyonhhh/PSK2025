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
    public class ProductController(IProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var paginatedProducts = await productService.GetAllProductsAsync(page, pageSize);
            return Ok(paginatedProducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            var (product, error) = await productService.GetProductByIdAsync(id);

            if (error == ServiceError.None)
                return Ok(product);

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Product"));
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (product, error) = await productService.CreateProductAsync(model);

            if (error == ServiceError.None)
                return CreatedAtAction(nameof(GetProduct), new { id = product!.Id }, product);

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Product"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (product, error, conflictingEntity) = await productService.UpdateProductAsync(id, model);

            if (error == ServiceError.None)
                return Ok(product);

            if (error == ServiceError.ConcurrencyError)
            {
                return Conflict(new
                {
                    message = error.GetErrorMessage(),
                    currentState = conflictingEntity
                });
            }

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Product"));
        }

        [HttpPut("{id}/availability")]
        [Authorize(Roles = "Manager,Barista")]
        public async Task<IActionResult> UpdateProductAvailability(string id, [FromBody] UpdateProductAvailabilityDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (product, error, conflictingEntity) = await productService.UpdateProductAvailabilityAsync(id, model);

            if (error == ServiceError.None)
                return Ok(product);

            if (error == ServiceError.ConcurrencyError)
            {
                return Conflict(new
                {
                    message = error.GetErrorMessage(),
                    currentState = conflictingEntity
                });
            }

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Product"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var error = await productService.DeleteProductAsync(id);

            if (error == ServiceError.None)
                return NoContent();

            return StatusCode(
                error.GetStatusCode(),
                error.GetErrorMessage("Product"));
        }
    }
}