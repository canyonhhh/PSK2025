using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        [Required]
        public Guid CartId { get; set; }
        [Required]
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }

}
