using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Lấy tất cả hashtag", Description = "Trả về danh sách hashtag đang hoạt động.")]
        public async Task<ActionResult<IEnumerable<Hashtag>>> GetHashtags()
        {
            return await _context.Hashtags
                .Where(h => h.TrangThai == true)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy hashtag theo ID", Description = "Trả về thông tin chi tiết của một hashtag.")]
        public async Task<ActionResult<Hashtag>> GetHashtag(int id)
        {
            var hashtag = await _context.Hashtags.FindAsync(id);

            if (hashtag == null || hashtag.TrangThai == false)
                return NotFound("Không tìm thấy hashtag với ID này.");

            return Ok(hashtag);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo hashtag mới", Description = "Thêm một hashtag mới vào cơ sở dữ liệu.")]
        public async Task<ActionResult<Hashtag>> PostHashtag(Hashtag hashtag)
        {
            var existingHashtag = await _context.Hashtags
                .FirstOrDefaultAsync(h => h.Ten.ToLower() == hashtag.Ten.ToLower());

            if (existingHashtag != null)
            {
                if (!existingHashtag.TrangThai)
                {
                    existingHashtag.TrangThai = true; // Khôi phục nếu bị xóa mềm
                    await _context.SaveChangesAsync();
                }
                return Ok(existingHashtag);
            }

            hashtag.TrangThai = true;
            _context.Hashtags.Add(hashtag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHashtag), new { id = hashtag.Id }, hashtag);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật hashtag", Description = "Cập nhật thông tin của một hashtag.")]
        public async Task<IActionResult> PutHashtag(int id, Hashtag hashtag)
        {
            if (id != hashtag.Id) return BadRequest("ID không khớp.");

            if (!_context.Hashtags.Any(h => h.Id == id))
                return NotFound("Không tìm thấy hashtag để cập nhật.");

            _context.Entry(hashtag).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa hashtag", Description = "Xóa mềm hashtag bằng cách set TrangThai = false.")]
        public async Task<IActionResult> DeleteHashtag(int id)
        {
            var hashtag = await _context.Hashtags.FindAsync(id);
            if (hashtag == null)
                return NotFound("Không tìm thấy hashtag để xóa.");

            hashtag.TrangThai = false; // ❌ Xóa mềm thay vì xóa hẳn
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa hashtag thành công." });
        }

        // ================= LIÊN KẾT TÁC PHẨM - HASHTAG =================

        [HttpGet("TacPham/{idTacPham}")]
        [SwaggerOperation(Summary = "Lấy hashtag theo tác phẩm", Description = "Trả về danh sách hashtag gắn với tác phẩm.")]
        public async Task<ActionResult<IEnumerable<Hashtag>>> GetHashtagsByTacPham(int idTacPham)
        {
            var hashtags = await _context.TacPham_Hashtags
                .Where(th => th.Id_TacPham == idTacPham && th.Hashtag.TrangThai == true)
                .Include(th => th.Hashtag)
                .Select(th => th.Hashtag)
                .ToListAsync();

            if (!hashtags.Any())
                return NotFound("Tác phẩm này chưa có hashtag nào.");

            return Ok(hashtags);
        }

        [HttpPost("TacPham/{idTacPham}")]
        [SwaggerOperation(Summary = "Thêm hashtag vào tác phẩm", Description = "Gán một hashtag có sẵn cho tác phẩm.")]
        public async Task<IActionResult> AddHashtagToTacPham(int idTacPham, [FromBody] int idHashtag)
        {
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

        [HttpDelete("TacPham/{idTacPham}/Hashtag/{idHashtag}")]
        [SwaggerOperation(Summary = "Xóa hashtag khỏi tác phẩm", Description = "Gỡ bỏ liên kết hashtag ra khỏi tác phẩm.")]
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
