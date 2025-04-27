using PSK2025.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSK2025.Models.Entities
{
    public class Cart
    {
        public Guid Id { get; set; } 
        public Guid UserId { get; set; }
        public CartStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public List<CartItem> Items { get; set; } = new(); 
    }
}
