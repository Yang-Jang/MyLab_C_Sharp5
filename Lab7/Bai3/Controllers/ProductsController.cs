using Microsoft.AspNetCore.Mvc;
using Bai3.Models;

namespace Bai3.Controllers
{
    public class ProductsController : ControllerBase
    {
        private static List<Product> products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 1500, Description = "Bền bỉ." },
            new Product { Id = 2, Name = "Smartphone", Price = 800, Description = "Mỏng manh." }
        };

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(products);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var p = products.FirstOrDefault(x => x.Id == id);
            if (p == null) return NotFound($"Không tìm thấy sản phẩm có ID: {id}");
            return Ok(p);
        }

        [HttpGet]
        [Route("api/products/search")]
        public IActionResult Search(string name)
        {
            var result = products.Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(result);
        }

        [HttpPost]
        [Route("api/products")]
        public IActionResult Create([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Sản phẩm không hợp lệ.");
            }

            product.Id = products.Max(x => x.Id) + 1;
            products.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut]
        [Route("api/products/{id}")]
        public IActionResult Update(int id, [FromBody] Product updatedProduct)
        {
            var existing = products.FirstOrDefault(x => x.Id == id);
            if (existing == null)
            {
                return NotFound($"Không tìm thấy sản phẩm có ID: {id}");
            }

            existing.Name = updatedProduct.Name;
            existing.Price = updatedProduct.Price;
            existing.Description = updatedProduct.Description;

            return NoContent();
        }

        [HttpDelete]
        [Route("api/products/{id}")]
        public IActionResult Delete(int id)
        {
            var product = products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound($"Không tìm thấy sản phẩm có ID: {id}");
            }

            products.Remove(product);
            return NoContent();
            // return Ok(new { message = "Xóa thành công" });
        }
    }
}