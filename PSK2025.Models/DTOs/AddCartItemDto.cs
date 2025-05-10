using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class AddCartItemDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}