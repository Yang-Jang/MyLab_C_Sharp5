using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bai1.Data;   
using Bai1.Models;

namespace Bai1.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDbContext _context;

        public ProductsController(ProductDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/Products  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var list = await _context.Products.ToListAsync();

            return Ok(new 
            { 
                success = true, 
                message = "Lấy danh sách sản phẩm thành công", 
                data = list 
            });        
        }

        // 2. GET: api/Products/
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            return Ok(new 
                { 
                    success = true, 
                    message = "Tìm thấy sản phẩm", 
                    data = product 
                });        
        }

        // 3. POST: api/Products (Thêm mới)
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                success = true, 
                message = "Thêm sản phẩm mới thành công!", 
                data = product 
            });        
        }

        // 4. PUT: api/Products/5 (Cập nhật)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest(new { success = false, message = "ID sản phẩm không khớp" });
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound(new { success = false, message = "Sản phẩm không tồn tại để sửa" });
                }
                else throw;               
                }
                return Ok(new 
            { 
                Success = true, 
                message = "Cập nhật sản phẩm thành công!" 
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { success = false, message = "Sản phẩm không tồn tại để xóa" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                success = true, 
                message = "Xóa sản phẩm thành công!" 
            });
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}