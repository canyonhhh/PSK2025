using PSK2025.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public CartStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public DateTime? PickupTime { get; set; }

        public List<CartItem> Items { get; set; } = new();
    }
}