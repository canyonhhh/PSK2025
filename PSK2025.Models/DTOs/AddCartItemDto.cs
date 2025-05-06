using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class AddCartItemDto
    {
        [Required]
        public string ItemId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }

}