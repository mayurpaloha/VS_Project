using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agro_Saffron.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        [Required]
        public string? ShoppingCartId { get; set; } // Session-based or user-based

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Product Product { get; set; }

        // Calculated property
        [NotMapped]
        public decimal TotalPrice => Product?.Price * Quantity ?? 0;
    }
}