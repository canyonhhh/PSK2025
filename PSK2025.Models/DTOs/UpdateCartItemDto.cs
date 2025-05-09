using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class UpdateCartItemDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}