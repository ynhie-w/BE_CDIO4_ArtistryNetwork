using CODE_CDIO4.Models;
using CODE_CDIO4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace CODE_CDIO4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HoaDonController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/HoaDon
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy tất cả hóa đơn")]
        public async Task<ActionResult<IEnumerable<HoaDon>>> GetAll()
        {
            return await _context.HoaDons.ToListAsync();
        }

        // GET: api/HoaDon/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy hóa đơn theo ID")]
        public async Task<ActionResult<HoaDon>> GetById(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null) return NotFound("Không tìm thấy hóa đơn.");
            return hoaDon;
        }

        // POST: api/HoaDon
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo hóa đơn mới")]
        public async Task<ActionResult<HoaDon>> Create(HoaDon hoaDon)
        {
            hoaDon.NgayLap = DateTime.Now;
            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = hoaDon.Id }, hoaDon);
        }

        // PUT: api/HoaDon/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật hóa đơn")]
        public async Task<IActionResult> Update(int id, HoaDon hoaDon)
        {
            if (id != hoaDon.Id) return BadRequest("ID không khớp.");

            _context.Entry(hoaDon).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/HoaDon/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa hóa đơn")]
        public async Task<IActionResult> Delete(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null) return NotFound("Không tìm thấy hóa đơn.");

            _context.HoaDons.Remove(hoaDon);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa hóa đơn thành công." });
        }
    }
}
