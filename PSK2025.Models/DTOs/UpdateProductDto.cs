using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class UpdateProductDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        public string? PhotoUrl { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsAvailable { get; set; }
    }
}