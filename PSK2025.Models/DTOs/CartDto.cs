using PSK2025.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSK2025.Models.DTOs
{
    public class CartDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public DateTime? PickupTime { get; set; }
        public IEnumerable<CartItemDto>? Items { get; set; }
    }
}