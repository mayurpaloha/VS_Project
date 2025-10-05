using Agro_Saffron.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agro_Saffron.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }

        [Range(0, 1000)]
        public int StockQuantity { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = "/images/Saffron_1.jpg";

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Navigation properties
        public Category Category { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // New properties for enhanced catalog
        [StringLength(50)]
        public string? Size { get; set; } // e.g., "Small", "Medium", "Large"

        //[StringLength(50)]
        //public string CareLevel { get; set; } // e.g., "Easy", "Moderate", "Difficult"

        //[Display(Name = "Light Requirements")]
        //[StringLength(100)]
        //public string LightRequirements { get; set; }

        //[Display(Name = "Watering Needs")]
        //[StringLength(100)]
        //public string WateringNeeds { get; set; }

        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Discount Percentage")]
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }

        // Calculated property for discounted price
        [NotMapped]
        public decimal DiscountedPrice => Price * (1 - DiscountPercentage / 100);

        [NotMapped]
        public bool OnSale => DiscountPercentage > 0;
    }
}
