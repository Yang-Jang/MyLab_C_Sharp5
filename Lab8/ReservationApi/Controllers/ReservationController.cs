using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationApi.Data;
using ReservationApi.Models;

namespace ReservationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Lấy tất cả (GET: api/Reservation)
        [HttpGet]
        public async Task<IEnumerable<Reservation>> Get()
        {
            return await _context.Reservations.ToListAsync();
        }

        // 2. Lấy 1 cái theo ID (GET: api/Reservation/5)
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await _context.Reservations.FindAsync(id);
            if (res == null) return NotFound();
            return Ok(res);
        }

        // 3. Thêm mới (POST: api/Reservation)
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Reservation res)
        {
            _context.Reservations.Add(res);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = res.Id }, res);
        }

        // 4. Cập nhật (PUT: api/Reservation/5)
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Reservation res)
        {
            if (id != res.Id) return BadRequest();

            _context.Entry(res).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent(); 
        }

        // 5. Xóa (DELETE: api/Reservation/5)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _context.Reservations.FindAsync(id);
            if (res == null) return NotFound();

            _context.Reservations.Remove(res);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}