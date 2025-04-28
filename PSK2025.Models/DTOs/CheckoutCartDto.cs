using System;
using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class CheckoutCartDto
    {
        [Required]
        public DateTime PickupTime { get; set; }
    }
}