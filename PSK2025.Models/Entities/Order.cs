using System.ComponentModel.DataAnnotations;
using PSK2025.Models.Enums;

namespace PSK2025.Models.Entities
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string CartId { get; set; } = string.Empty;
        
        public decimal TotalPrice { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? CompletedAt { get; set; }
        
        public DateTime ExpectedCompletionTime { get; set; }
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public virtual User? User { get; set; }
        
        public virtual Cart? Cart { get; set; }
        
        public List<OrderItem> Items { get; set; } = new();
    }
}