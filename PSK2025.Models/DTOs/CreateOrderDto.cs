using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public DateTime ExpectedCompletionTime { get; set; }
    }
}