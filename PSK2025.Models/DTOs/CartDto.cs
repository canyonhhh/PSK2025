using System;
using System.Collections.Generic;

namespace PSK2025.Models.DTOs
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public DateTime? PickupTime { get; set; }

        public List<CartItemDto> Items { get; set; } = new();
    }
}
