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
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public DateTime? PickupTime { get; set; }
        public CartStatus Status { get; set; }
        public List<IEnumerable> Items { get; set; } = new();
    }
}
