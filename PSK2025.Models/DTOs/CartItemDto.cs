using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class CartItemDto
    {
        [Required]
        public string? ItemId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string? ProductName { get; set; }
        public decimal Price { get; set; }
    }
}