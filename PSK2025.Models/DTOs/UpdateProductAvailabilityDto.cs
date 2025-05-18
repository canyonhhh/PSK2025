using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class UpdateProductAvailabilityDto
    {
        [Required]
        public bool IsAvailable { get; set; }
    }
}