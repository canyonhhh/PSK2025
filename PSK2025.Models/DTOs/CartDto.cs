using System;
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
        public List<CartItemDto> Items { get; set; } = new();
    }
}
