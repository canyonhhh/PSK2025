using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.Entities
{
    public class CartItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string CartId { get; set; } = string.Empty;
        [Required]
        public string ItemId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

}
