using Agro_Saffron.Data;
using Agro_Saffron.Models;
using Microsoft.EntityFrameworkCore;

namespace Agro_Saffron.Services
{
    public class ShoppingCartService
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

        public async Task AddToCart(int productId, int quantity = 1)
        {
            var cartId = GetCartId();
            var cartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(c => c.ShoppingCartId == cartId && c.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new ShoppingCartItem
                {
                    ShoppingCartId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ShoppingCartItem>> GetCartItems()
        {
            var cartId = GetCartId();
            return await _context.ShoppingCartItems
                .Where(c => c.ShoppingCartId == cartId)
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<decimal> GetCartTotal()
        {
            var cartId = GetCartId();
            var items = await _context.ShoppingCartItems
                .Where(c => c.ShoppingCartId == cartId)
                .Include(c => c.Product)
                .ToListAsync();

            return items.Sum(item => item.Quantity * item.Product.Price);
        }

        public async Task RemoveFromCart(int productId)
        {
            var cartId = GetCartId();
            var cartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(c => c.ShoppingCartId == cartId && c.ProductId == productId);

            if (cartItem != null)
            {
                _context.ShoppingCartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCart()
        {
            var cartId = GetCartId();
            var cartItems = await _context.ShoppingCartItems
                .Where(c => c.ShoppingCartId == cartId)
                .ToListAsync();

            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}