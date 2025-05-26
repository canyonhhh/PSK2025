using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class ConfirmPaymentDto
    {
        [Required]
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}