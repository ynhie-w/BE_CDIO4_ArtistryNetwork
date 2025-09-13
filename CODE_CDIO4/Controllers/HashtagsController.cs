using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HashtagsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HashtagsController(AppDbContext context)
        {
            _context = context;
        }

        // ================= HASHTAG CRUD =================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hashtag>>> GetHashtags()
        {
            return await _context.Hashtags.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Hashtag>> GetHashtag(int id)
        {
            var hashtag = await _context.Hashtags.FindAsync(id);

            if (hashtag == null)
                return NotFound("Không tìm thấy hashtag với ID này.");

            return Ok(hashtag);
        }

        [HttpPost]
        public async Task<ActionResult<Hashtag>> PostHashtag(Hashtag hashtag)
        {
            var existingHashtag = await _context.Hashtags
                .FirstOrDefaultAsync(h => h.Ten.ToLower() == hashtag.Ten.ToLower());

            if (existingHashtag != null)
                return Ok(existingHashtag); // Nếu tồn tại thì trả về luôn

            _context.Hashtags.Add(hashtag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHashtag), new { id = hashtag.Id }, hashtag);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHashtag(int id)
        {
            var hashtag = await _context.Hashtags.FindAsync(id);
            if (hashtag == null)
                return NotFound("Không tìm thấy hashtag để xóa.");

            _context.Hashtags.Remove(hashtag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ================= LIÊN KẾT TÁC PHẨM - HASHTAG =================

        // GET: api/Hashtags/TacPham/5
        [HttpGet("TacPham/{idTacPham}")]
        public async Task<ActionResult<IEnumerable<Hashtag>>> GetHashtagsByTacPham(int idTacPham)
        {
            var hashtags = await _context.TacPham_Hashtags
                .Where(th => th.Id_TacPham == idTacPham)
                .Include(th => th.Hashtag)
                .Select(th => th.Hashtag)
                .ToListAsync();

            if (!hashtags.Any())
                return NotFound("Tác phẩm này chưa có hashtag nào.");

            return Ok(hashtags);
        }

        // POST: api/Hashtags/TacPham/5
        [HttpPost("TacPham/{idTacPham}")]
        public async Task<IActionResult> AddHashtagToTacPham(int idTacPham, [FromBody] int idHashtag)
        {
            // kiểm tra tồn tại
            var exists = await _context.TacPham_Hashtags
                .AnyAsync(th => th.Id_TacPham == idTacPham && th.Id_Hashtag == idHashtag);

            if (exists)
                return Conflict("Hashtag này đã được gán cho tác phẩm.");

            var link = new TacPham_Hashtags
            {
                Id_TacPham = idTacPham,
                Id_Hashtag = idHashtag
            };

            _context.TacPham_Hashtags.Add(link);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm hashtag vào tác phẩm thành công." });
        }

        // DELETE: api/Hashtags/TacPham/5/Hashtag/3
        [HttpDelete("TacPham/{idTacPham}/Hashtag/{idHashtag}")]
        public async Task<IActionResult> RemoveHashtagFromTacPham(int idTacPham, int idHashtag)
        {
            var link = await _context.TacPham_Hashtags.FindAsync(idTacPham, idHashtag);
            if (link == null)
                return NotFound("Liên kết này không tồn tại.");

            _context.TacPham_Hashtags.Remove(link);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa hashtag khỏi tác phẩm thành công." });
        }
    }
}
