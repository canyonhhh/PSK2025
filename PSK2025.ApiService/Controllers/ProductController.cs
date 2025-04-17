using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            var (product, error) = await productService.GetProductByIdAsync(id);

            return error switch
            {
                ServiceError.None => Ok(product),
                ServiceError.NotFound => NotFound(),
                _ => StatusCode(500, "An unexpected error occurred")
            };
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

            return error switch
            {
                ServiceError.None => CreatedAtAction(nameof(GetProduct), new { id = product!.Id }, product),
                ServiceError.DatabaseError => StatusCode(500, "Failed to create product"),
                _ => BadRequest("Invalid request")
            };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (product, error) = await productService.UpdateProductAsync(id, model);

            return error switch
            {
                ServiceError.None => Ok(product),
                ServiceError.NotFound => NotFound(),
                ServiceError.DatabaseError => StatusCode(500, "Failed to update product"),
                _ => BadRequest("Invalid request")
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var error = await productService.DeleteProductAsync(id);

            return error switch
            {
                ServiceError.None => NoContent(),
                ServiceError.NotFound => NotFound(),
                _ => StatusCode(500, "Failed to delete product")
            };
        }
    }
}