namespace PSK2025.Models.DTOs
{
    public class CartDto
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CartItemDto> Items { get; set; } = [];
    }
}