using Agro_Saffron.Models;
using System.ComponentModel.DataAnnotations;

namespace Agro_Saffron.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property
        public List<Product> Products { get; set; } = new List<Product>();
    }
}