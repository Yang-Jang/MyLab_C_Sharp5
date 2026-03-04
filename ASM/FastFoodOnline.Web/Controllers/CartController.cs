using FastFoodOnline.Web.Data;
using FastFoodOnline.Web.Extensions;
using FastFoodOnline.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFoodOnline.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetUserId() => _userManager.GetUserId(User) ?? "";

        private async Task<Cart> GetOrCreateCartAsync()
        {
            var userId = GetUserId();

            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Food)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Combo)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        private static int ClampQuantity(int quantity)
        {
            if (quantity < 1) return 1;
            if (quantity > 50) return 50;
            return quantity;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = await GetOrCreateCartAsync();
            return View(cart);
        }

        // ===== 1. API GET CART COUNT (Cho JavaScript gọi) =====
        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> GetCartCount()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Ok(0);

            var count = await _context.CartItems
                .Where(c => c.Cart.UserId == userId)
                .SumAsync(c => c.Quantity);

            return Ok(count);
        }

        // ===== 2. ADD FOOD (AJAX JSON) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFood(int foodId, int quantity = 1)
        {
            quantity = ClampQuantity(quantity);
            var food = await _context.Foods.FirstOrDefaultAsync(f => f.Id == foodId);
            if (food == null) return NotFound(new { message = "Món không tồn tại" });

            var cart = await GetOrCreateCartAsync();
            var existingItem = cart.Items?.FirstOrDefault(i => i.FoodId == foodId);

            if (existingItem != null)
            {
                existingItem.Quantity = ClampQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.Id, 
                    FoodId = foodId, 
                    Quantity = quantity, 
                    UnitPrice = food.Price
                });
            }

            await _context.SaveChangesAsync();

            // Trả về JSON để JS xử lý mà không cần reload trang
            return Ok(new { success = true, message = "Đã thêm món vào giỏ!" });
        }

        // ===== 3. ADD COMBO (AJAX JSON) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCombo(int comboId, int quantity = 1)
        {
            quantity = ClampQuantity(quantity);
            var combo = await _context.Combos.FirstOrDefaultAsync(c => c.Id == comboId);
            if (combo == null) return NotFound(new { message = "Combo không tồn tại" });

            var cart = await GetOrCreateCartAsync();
            var existingItem = cart.Items?.FirstOrDefault(i => i.ComboId == comboId);

            if (existingItem != null)
            {
                existingItem.Quantity = ClampQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.Id, 
                    ComboId = comboId, 
                    Quantity = quantity, 
                    UnitPrice = combo.Price
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đã thêm combo vào giỏ!" });
        }

        // ===== UPDATE QUANTITY (Giữ nguyên Redirect cho trang Cart) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int itemId, int quantity)
        {
            var userId = GetUserId();
            quantity = ClampQuantity(quantity);

            var item = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart.UserId == userId);

            if (item == null)
            {
                this.Toast("Không tìm thấy sản phẩm trong giỏ.", "error");
                return RedirectToAction(nameof(Index));
            }

            item.Quantity = quantity;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===== REMOVE ITEM (Giữ nguyên Redirect cho trang Cart) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int itemId)
        {
            var userId = GetUserId();

            var item = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart.UserId == userId);

            if (item == null)
            {
                this.Toast("Không tìm thấy sản phẩm để xoá.", "error");
                return RedirectToAction(nameof(Index));
            }

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            this.Toast("Đã xoá sản phẩm khỏi giỏ.", "success");
            return RedirectToAction(nameof(Index));
        }

        // ===== CLEAR CART =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var cart = await GetOrCreateCartAsync();
            if (cart.Items != null && cart.Items.Any())
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
                this.Toast("Đã xoá toàn bộ giỏ hàng.", "success");
            }
            else
            {
                this.Toast("Giỏ hàng đang trống.", "info");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}