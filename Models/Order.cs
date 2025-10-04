using System.ComponentModel.DataAnnotations;

namespace Agro_Saffron.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0.01, 100000)]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string ShippingAddress { get; set; }

        [StringLength(255)]
        public string CustomerName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string CustomerEmail { get; set; }

        // Navigation properties
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}