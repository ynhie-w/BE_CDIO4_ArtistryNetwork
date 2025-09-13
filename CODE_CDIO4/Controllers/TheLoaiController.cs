using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheLoaiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TheLoaiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TheLoai
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheLoai>>> GetTheLoais()
        {
            var theLoais = await _context.TheLoais.ToListAsync();
            return Ok(theLoais);
        }

        // GET: api/TheLoai/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TheLoai>> GetTheLoai(int id)
        {
            var theLoai = await _context.TheLoais.FindAsync(id);

            if (theLoai == null)
            {
                return NotFound("Không tìm thấy thể loại này.");
            }

            return Ok(theLoai);
        }

        // POST: api/TheLoai
        [HttpPost]
        public async Task<ActionResult<TheLoai>> PostTheLoai(TheLoai theLoai)
        {
            // Kiểm tra xem thể loại đã tồn tại chưa để tránh trùng lặp
            var existingTheLoai = await _context.TheLoais.FirstOrDefaultAsync(t => t.Ten.ToLower() == theLoai.Ten.ToLower());
            if (existingTheLoai != null)
            {
                return Conflict("Thể loại này đã tồn tại.");
            }

            _context.TheLoais.Add(theLoai);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTheLoai), new { id = theLoai.Id }, theLoai);
        }

        // PUT: api/TheLoai/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTheLoai(int id, TheLoai theLoai)
        {
            if (id != theLoai.Id)
            {
                return BadRequest("ID trong URL không khớp với ID của đối tượng.");
            }

            _context.Entry(theLoai).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TheLoaiExists(id))
                {
                    return NotFound("Không tìm thấy thể loại để cập nhật.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/TheLoai/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTheLoai(int id)
        {
            var theLoai = await _context.TheLoais.FindAsync(id);
            if (theLoai == null)
            {
                return NotFound("Không tìm thấy thể loại để xóa.");
            }

            _context.TheLoais.Remove(theLoai);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã xóa thể loại '{theLoai.Ten}' thành công." });
        }

        private bool TheLoaiExists(int id)
        {
            return _context.TheLoais.Any(e => e.Id == id);
        }
    }
}
