using Microsoft.EntityFrameworkCore;
using Agro_Saffron.Data;
using Agro_Saffron.Models;

namespace Agro_Saffron.Services
{
    public interface IShoppingCartService
    {
        Task AddToCartAsync(int productId, int quantity = 1);
        Task RemoveFromCartAsync(int itemId);
        Task UpdateQuantityAsync(int itemId, int quantity);
        Task ClearCartAsync();
        Task<List<ShoppingCartItem>> GetCartItemsAsync();
        Task<int> GetCartItemsCountAsync();
        Task<decimal> GetCartTotalAsync();
        Task<ShoppingCartItem> GetCartItemAsync(int itemId);
    }

    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingCartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCartId()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartId = session.GetString("CartId");

            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                session.SetString("CartId", cartId);
            }

            return cartId;
        }

        public async Task AddToCartAsync(int productId, int quantity = 1)
        {
            var cartId = GetCartId();
            var product = await _context.Products.FindAsync(productId);

            if (product == null || !product.IsActive)
            {
                throw new ArgumentException("Product not found or not available");
            }

            if (quantity > product.StockQuantity)
            {
                throw new ArgumentException("Requested quantity exceeds available stock");
            }

            var existingItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(item => item.ShoppingCartId == cartId && item.ProductId == productId);

            if (existingItem != null)
            {
                // Update existing item
                existingItem.Quantity += quantity;
                if (existingItem.Quantity > product.StockQuantity)
                {
                    existingItem.Quantity = product.StockQuantity;
                }
            }
            else
            {
                // Add new item
                var cartItem = new ShoppingCartItem
                {
                    ShoppingCartId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.ShoppingCartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int itemId)
        {
            var cartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(item => item.Id == itemId && item.ShoppingCartId == GetCartId());

            if (cartItem != null)
            {
                _context.ShoppingCartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateQuantityAsync(int itemId, int quantity)
        {
            if (quantity < 1)
            {
                await RemoveFromCartAsync(itemId);
                return;
            }

            var cartItem = await _context.ShoppingCartItems
                .Include(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == itemId && item.ShoppingCartId == GetCartId());

            if (cartItem != null)
            {
                if (quantity > cartItem.Product.StockQuantity)
                {
                    quantity = cartItem.Product.StockQuantity;
                }
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync()
        {
            var cartId = GetCartId();
            var cartItems = await _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == cartId)
                .ToListAsync();

            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShoppingCartItem>> GetCartItemsAsync()
        {
            var cartId = GetCartId();
            return await _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == cartId)
                .Include(item => item.Product)
                .ThenInclude(p => p.Category)
                .OrderBy(item => item.CreatedDate)
                .ToListAsync();
        }

        public async Task<int> GetCartItemsCountAsync()
        {
            var cartId = GetCartId();
            return await _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == cartId)
                .SumAsync(item => item.Quantity);
        }

        public async Task<decimal> GetCartTotalAsync()
        {
            var cartId = GetCartId();
            var items = await _context.ShoppingCartItems
                .Where(item => item.ShoppingCartId == cartId)
                .Include(item => item.Product)
                .ToListAsync();

            return items.Sum(item => item.TotalPrice);
        }

        public async Task<ShoppingCartItem> GetCartItemAsync(int itemId)
        {
            return await _context.ShoppingCartItems
                .Include(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == itemId && item.ShoppingCartId == GetCartId());
        }
    }
}