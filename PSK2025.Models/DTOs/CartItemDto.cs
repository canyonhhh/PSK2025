using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class CartItemDto
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string? ProductPhotoUrl { get; set; }
    }
}