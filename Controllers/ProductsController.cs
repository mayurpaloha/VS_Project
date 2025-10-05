using Agro_Saffron.Data;
using Agro_Saffron.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agro_Saffron.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Products with search, filter, and pagination
        public async Task<IActionResult> Index(
            string searchString,
            int? categoryId,
            string sortOrder = "name",
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 12)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentCategory"] = categoryId;
            ViewData["CurrentMinPrice"] = minPrice;
            ViewData["CurrentMaxPrice"] = maxPrice;

            // Get all active products
            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Description.Contains(searchString) ||
                    p.Category.Name.Contains(searchString));
            }

            // Apply category filter
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            // Apply price range filter
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            // Apply sorting
            products = sortOrder?.ToLower() switch
            {
                "price" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "newest" => products.OrderByDescending(p => p.CreatedDate),
                "name_desc" => products.OrderByDescending(p => p.Name),
                _ => products.OrderBy(p => p.Name) // Default sort by name
            };

            // Get total count for pagination
            var totalItems = await products.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Apply pagination
            var pagedProducts = await products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get categories for filter dropdown
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();

            // Prepare view model
            var viewModel = new ProductIndexViewModel
            {
                Products = pagedProducts,
                Categories = categories,
                CurrentCategoryId = categoryId,
                SearchString = searchString,
                SortOrder = sortOrder,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            // Get related products (same category)
            var relatedProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == product.CategoryId &&
                           p.Id != product.Id &&
                           p.IsActive)
                .Take(4)
                .ToListAsync();

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts
            };

            return View(viewModel);
        }

        // GET: Products/Category/5
        public async Task<IActionResult> Category(int id, int page = 1)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (category == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id && p.IsActive)
                .OrderBy(p => p.Name)
                .Skip((page - 1) * 12)
                .Take(12)
                .ToListAsync();

            var totalProducts = await _context.Products
                .CountAsync(p => p.CategoryId == id && p.IsActive);

            var viewModel = new ProductCategoryViewModel
            {
                Category = category,
                Products = products,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / 12.0)
            };

            return View(viewModel);
        }

        // AJAX endpoint for quick product search
        [HttpGet]
        public async Task<IActionResult> QuickSearch(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            var products = await _context.Products
                .Where(p => p.Name.Contains(term) && p.IsActive)
                .Select(p => new {
                    id = p.Id,
                    name = p.Name,
                    price = p.Price,
                    image = p.ImageUrl,
                    category = p.Category.Name
                })
                .Take(10)
                .ToListAsync();

            return Json(products);
        }
    }
}