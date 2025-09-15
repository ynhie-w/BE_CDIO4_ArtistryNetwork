using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Models;
using Swashbuckle.AspNetCore.Annotations;
using CODE_CDIO4.Repository;

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
        [HttpGet("TacPham/{idTacPham}")]
        [SwaggerOperation(Summary = "Lấy tất cả hashtag đang hiển thị của một tác phẩm")]
        public async Task<ActionResult<IEnumerable<Hashtag>>> GetHashtagsByTacPham(int idTacPham)
        {
            var hashtags = await _context.TacPham_Hashtags
                                         .Where(th => th.Id_TacPham == idTacPham && th.TrangThai == true)
                                         .Select(th => th.Hashtag!)
                                         .ToListAsync();

            if (!hashtags.Any())
                return NotFound("Tác phẩm này không có hashtag nào đang hiển thị.");

            return Ok(hashtags);
        }

        // POST: api/TacPham_Hashtags
        [HttpPost]
        [SwaggerOperation(Summary = "Thêm một hashtag vào tác phẩm")]
        public async Task<ActionResult<TacPham_Hashtags>> AddHashtagToTacPham(TacPham_Hashtags tacPhamHashtag)
        {
            var exists = await _context.TacPham_Hashtags
                                       .FindAsync(tacPhamHashtag.Id_TacPham, tacPhamHashtag.Id_Hashtag);
            if (exists != null)
                return Conflict("Mối liên kết này đã tồn tại.");

            tacPhamHashtag.TrangThai = true;
            _context.TacPham_Hashtags.Add(tacPhamHashtag);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("ID tác phẩm hoặc ID hashtag không hợp lệ.");
            }

            return CreatedAtAction(nameof(GetHashtagsByTacPham), new { idTacPham = tacPhamHashtag.Id_TacPham }, tacPhamHashtag);
        }

        // PUT: api/TacPham_Hashtags/TacPham/5/Hashtag/10
        [HttpPut("TacPham/{idTacPham}/Hashtag/{idHashtag}")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái hiển thị hashtag của tác phẩm")]
        public async Task<IActionResult> UpdateTrangThai(int idTacPham, int idHashtag, [FromBody] bool trangThai)
        {
            var tacPhamHashtag = await _context.TacPham_Hashtags.FindAsync(idTacPham, idHashtag);
            if (tacPhamHashtag == null)
                return NotFound("Không tìm thấy mối liên kết.");

            tacPhamHashtag.TrangThai = trangThai;
            _context.Entry(tacPhamHashtag).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/TacPham_Hashtags/TacPham/5/Hashtag/10
        [HttpDelete("TacPham/{idTacPham}/Hashtag/{idHashtag}")]
        [SwaggerOperation(Summary = "Xóa hẳn hashtag khỏi tác phẩm")]
        public async Task<IActionResult> RemoveHashtagFromTacPham(int idTacPham, int idHashtag)
        {
            var tacPhamHashtag = await _context.TacPham_Hashtags.FindAsync(idTacPham, idHashtag);
            if (tacPhamHashtag == null)
                return NotFound("Không tìm thấy mối liên kết.");

            _context.TacPham_Hashtags.Remove(tacPhamHashtag);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa hashtag khỏi tác phẩm." });
        }
    }
}
