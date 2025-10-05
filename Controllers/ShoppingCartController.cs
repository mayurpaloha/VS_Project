using Microsoft.AspNetCore.Mvc;
using Agro_Saffron.Services;
using Agro_Saffron.Models;
using Agro_Saffron.Models.ViewModels;

namespace Agro_Saffron.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILogger<ShoppingCartController> _logger;

        public ShoppingCartController(IShoppingCartService shoppingCartService, ILogger<ShoppingCartController> logger)
        {
            _shoppingCartService = shoppingCartService;
            _logger = logger;
        }

        // GET: ShoppingCart
        public async Task<IActionResult> Index()
        {
            var cartItems = await _shoppingCartService.GetCartItemsAsync();
            var cartTotal = await _shoppingCartService.GetCartTotalAsync();
            var itemCount = await _shoppingCartService.GetCartItemsCountAsync();

            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cartItems,
                CartTotal = cartTotal,
                ItemsCount = itemCount
            };

            return View(viewModel);
        }

        // POST: ShoppingCart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                await _shoppingCartService.AddToCartAsync(request.ProductId, request.Quantity);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // AJAX request - return JSON
                    var cartCount = await _shoppingCartService.GetCartItemsCountAsync();
                    return Json(new
                    {
                        success = true,
                        message = "Product added to cart!",
                        cartCount = cartCount
                    });
                }

                // Regular request - redirect back
                TempData["SuccessMessage"] = "Product added to cart!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to cart");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = false,
                        message = ex.Message
                    });
                }

                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: ShoppingCart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int itemId, int quantity)
        {
            try
            {
                await _shoppingCartService.UpdateQuantityAsync(itemId, quantity);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var cartItem = await _shoppingCartService.GetCartItemAsync(itemId);
                    var cartTotal = await _shoppingCartService.GetCartTotalAsync();
                    var cartCount = await _shoppingCartService.GetCartItemsCountAsync();

                    return Json(new
                    {
                        success = true,
                        itemTotal = cartItem?.TotalPrice ?? 0,
                        cartTotal = cartTotal,
                        cartCount = cartCount
                    });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart quantity");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = false,
                        message = ex.Message
                    });
                }

                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: ShoppingCart/RemoveFromCart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            try
            {
                await _shoppingCartService.RemoveFromCartAsync(itemId);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var cartTotal = await _shoppingCartService.GetCartTotalAsync();
                    var cartCount = await _shoppingCartService.GetCartItemsCountAsync();

                    return Json(new
                    {
                        success = true,
                        message = "Item removed from cart",
                        cartTotal = cartTotal,
                        cartCount = cartCount
                    });
                }

                TempData["SuccessMessage"] = "Item removed from cart";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = false,
                        message = ex.Message
                    });
                }

                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: ShoppingCart/ClearCart
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                await _shoppingCartService.ClearCartAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = true,
                        message = "Cart cleared successfully"
                    });
                }

                TempData["SuccessMessage"] = "Cart cleared successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = false,
                        message = ex.Message
                    });
                }

                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: ShoppingCart/GetCartCount
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var count = await _shoppingCartService.GetCartItemsCountAsync();
            return Json(new { count = count });
        }

        public class AddToCartRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; } = 1;
        }
    }
}