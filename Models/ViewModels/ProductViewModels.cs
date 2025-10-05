using Agro_Saffron.Models;

namespace Agro_Saffron.Models.ViewModels
{
    public class ProductIndexViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public int? CurrentCategoryId { get; set; }
        public string? SearchString { get; set; }
        public string SortOrder { get; set; } = "name";
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        // Available sort options
        public List<string> SortOptions => new List<string>
        {
            "name",
            "name_desc",
            "price",
            "price_desc",
            "newest"
        };
    }

    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
    }

    public class ProductCategoryViewModel
    {
        public Category Category { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
    }

    public class ShoppingCartViewModel
    {
        public List<ShoppingCartItem> CartItems { get; set; } = new List<ShoppingCartItem>();
        public decimal CartTotal { get; set; }
        public int ItemsCount { get; set; }
    }
}