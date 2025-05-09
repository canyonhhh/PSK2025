using PSK2025.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSK2025.Models.Entities
{
    public class Order
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } = string.Empty;

        [NotMapped]
        public decimal TotalPrice => Items?.Sum(item => item.ProductPrice * item.Quantity) ?? 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public DateTime ExpectedCompletionTime { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual List<OrderItem> Items { get; set; } = [];
    }
}