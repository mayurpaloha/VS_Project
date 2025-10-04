using Agro_Saffron.Data;
using Agro_Saffron.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Agro_Saffron.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .Take(8)
                .ToListAsync();

            return View(featuredProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
