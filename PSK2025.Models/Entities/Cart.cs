using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.Entities
{
    public class Cart
    {
        [Key]
        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual User? User { get; set; }
        public virtual List<CartItem> Items { get; set; } = [];
    }
}