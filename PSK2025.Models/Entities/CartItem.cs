using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.Entities
{
    public class CartItem
    {
        public string? Id { get; set; }
        [Required]
        public string? CartId { get; set; }
        [Required]
        public string? ItemId { get; set; }
        public int Quantity { get; set; }
    }

}
