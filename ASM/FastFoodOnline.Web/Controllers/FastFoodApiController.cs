using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FastFoodOnline.Web.Data;    
using FastFoodOnline.Web.Models;  
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastFoodOnline.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FastFoodApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FastFoodApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/FastFoodApi
        // Lấy danh sách món ăn (Kèm tên danh mục)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetFoods()
        {
            // Dùng Select để tránh lỗi vòng lặp JSON (Circular Reference) và chỉ lấy field cần thiết
            var foods = await _context.Foods
                .Include(f => f.Category)
                .Select(f => new {
                    f.Id,
                    f.Name,
                    f.Price,
                    f.ImageUrl,
                    f.Description,
                    f.CategoryId,
                    CategoryName = f.Category != null ? f.Category.Name : "N/A", // Lấy tên danh mục
                    f.IsActive
                })
                .ToListAsync();

            return Ok(foods);
        }

        // 2. GET: api/FastFoodApi/5
        // Lấy chi tiết 1 món ăn
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
            {
                return NotFound(new { message = "Không tìm thấy món ăn này" });
            }

            return Ok(food);
        }

        // 3. POST: api/FastFoodApi
        // Thêm mới món ăn
        [HttpPost]
        public async Task<ActionResult<Food>> PostFood(Food food)
        {
            // Kiểm tra Category có tồn tại không
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == food.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "CategoryId không hợp lệ" });
            }

            food.CreatedAt = DateTime.Now; // Gán ngày tạo
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFood", new { id = food.Id }, food);
        }

        // 4. PUT: api/FastFoodApi/5
        // Cập nhật món ăn
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFood(int id, Food food)
        {
            if (id != food.Id)
            {
                return BadRequest(new { message = "ID không khớp" });
            }

            // Đánh dấu object này đã bị sửa đổi
            _context.Entry(food).State = EntityState.Modified;

            // Giữ nguyên ngày tạo cũ (nếu cần thiết) hoặc logic nghiệp vụ khác
            // Ở đây ta đơn giản là save
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Cập nhật thành công!" });
        }

        // 5. DELETE: api/FastFoodApi/5
        // Xóa món ăn
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }

            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa thành công!" });
        }

        private bool FoodExists(int id)
        {
            return _context.Foods.Any(e => e.Id == id);
        }
    }
}