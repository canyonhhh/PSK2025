using PSK2025.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class UpdateCartDto
    {
        [Required]
        public DateTime PickupTime { get; set; }

        [Required]
        public CartStatus Status { get; set; }
    }
}