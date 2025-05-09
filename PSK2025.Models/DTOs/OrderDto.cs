using PSK2025.Models.Enums;

namespace PSK2025.Models.DTOs
{
    public class OrderDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime ExpectedCompletionTime { get; set; }
        public OrderStatus Status { get; set; }
        public IEnumerable<OrderItemDto>? Items { get; set; }
    }
}