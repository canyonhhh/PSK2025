using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class CreatePaymentIntentDto
    {
        [Required]
        public string Currency { get; set; } = "usd";

        [Required]
        public DateTime ExpectedCompletionTime { get; set; }
    }
}