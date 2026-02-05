using Microsoft.AspNetCore.Mvc;

namespace Bai2.Controllers
{
    
    // [Route("api/[controller]")] 
    // [ApiController]
    // không cần vì đã có ở Program.cs ( khác với bài 1 )
    public class BooksController : ControllerBase
    {
        public class Book { public int Id { get; set; } public string Title { get; set; } }
        private static List<Book> books = new List<Book>
        {
            new Book { Id = 1, Title = "Dế Mèn Phiêu Lưu Ký" },
            new Book { Id = 2, Title = "Harry Potter" }
        };

        [HttpGet]
        public IActionResult GetBooks()
        {
            return Ok(books);
        }

        [HttpGet]
        public IActionResult GetBookId(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public IActionResult CreateBook([FromBody] Book newBook)
        {
            newBook.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;

            books.Add(newBook);            
            // return CreatedAtAction(nameof(GetBookId), new { id = newBook.Id }, newBook);
            return Ok(newBook);
        }

        [HttpPut]
        public IActionResult UpdateBook(int id, [FromBody] Book updatedBook)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound($"Không tìm thấy sách với ID: {id}");

            books.Remove(book);
            books.Add(updatedBook);
            return Ok(updatedBook);
        }

        [HttpDelete]
        public IActionResult DeleteBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound($"Không tìm thấy sách với ID: {id}");

            books.Remove(book);
            return Ok($"Đã xóa sách với ID: {id}");
        }
    }
}