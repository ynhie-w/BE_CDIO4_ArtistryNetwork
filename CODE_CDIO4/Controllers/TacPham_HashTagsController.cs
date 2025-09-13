using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TacPham_HashtagsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TacPham_HashtagsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TacPham_Hashtags/TacPham/5
        // Lấy tất cả hashtag của một tác phẩm
        [HttpGet("TacPham/{idTacPham}")]
        public async Task<ActionResult<IEnumerable<Hashtag>>> GetHashtagsByTacPham(int idTacPham)
        {
            var hashtags = await _context.TacPham_Hashtags
                                         .Where(th => th.Id_TacPham == idTacPham)
                                         .Select(th => th.Hashtag)
                                         .ToListAsync();

            if (hashtags == null || !hashtags.Any())
            {
                return NotFound("Tác phẩm này không có hashtag nào.");
            }

            return Ok(hashtags);
        }

        // POST: api/TacPham_Hashtags
        // Thêm một hashtag vào một tác phẩm
        [HttpPost]
        public async Task<ActionResult<TacPham_Hashtags>> AddHashtagToTacPham(TacPham_Hashtags tacPhamHashtag)
        {
            // Kiểm tra xem mối liên kết đã tồn tại chưa
            var exists = await _context.TacPham_Hashtags
                                       .AnyAsync(th => th.Id_TacPham == tacPhamHashtag.Id_TacPham && th.Id_Hashtag == tacPhamHashtag.Id_Hashtag);
            if (exists)
            {
                return Conflict("Mối liên kết giữa tác phẩm và hashtag này đã tồn tại.");
            }

            _context.TacPham_Hashtags.Add(tacPhamHashtag);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Xử lý lỗi nếu Id_TacPham hoặc Id_Hashtag không tồn tại
                return BadRequest("ID tác phẩm hoặc ID hashtag không hợp lệ.");
            }

            return Ok(tacPhamHashtag);
        }

        // DELETE: api/TacPham_Hashtags/TacPham/5/Hashtag/10
        // Xóa một hashtag khỏi một tác phẩm
        [HttpDelete("TacPham/{idTacPham}/Hashtag/{idHashtag}")]
        public async Task<IActionResult> RemoveHashtagFromTacPham(int idTacPham, int idHashtag)
        {
            var tacPhamHashtag = await _context.TacPham_Hashtags.FindAsync(idTacPham, idHashtag);
            if (tacPhamHashtag == null)
            {
                return NotFound("Không tìm thấy mối liên kết này.");
            }

            _context.TacPham_Hashtags.Remove(tacPhamHashtag);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}