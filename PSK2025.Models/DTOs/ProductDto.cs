namespace PSK2025.Models.DTOs
{
    public class ProductDto
    {
        public required string Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? PhotoUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}