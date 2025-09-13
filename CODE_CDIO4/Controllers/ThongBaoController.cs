using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongBaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ThongBaoController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== GET ====================

        // GET: api/ThongBao/NguoiDung/5
        // Lấy tất cả thông báo của 1 người dùng
        [HttpGet("NguoiDung/{idNguoiDung}")]
        public async Task<ActionResult<IEnumerable<ThongBao>>> GetThongBaoByNguoiDung(int idNguoiDung)
        {
            var thongBaos = await _context.ThongBaos
                                          .Where(tb => tb.Id_NguoiDung == idNguoiDung)
                                          .OrderByDescending(tb => tb.NgayTao)
                                          .ToListAsync();

            if (!thongBaos.Any())
                return NotFound("Người dùng này chưa có thông báo nào.");

            return Ok(thongBaos);
        }

        // GET: api/ThongBao/5
        // Lấy chi tiết 1 thông báo
        [HttpGet("{id}")]
        public async Task<ActionResult<ThongBao>> GetThongBao(int id)
        {
            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao == null)
                return NotFound("Không tìm thấy thông báo.");

            return Ok(thongBao);
        }

        // ==================== POST ====================

        // POST: api/ThongBao
        // Tạo mới thông báo
        [HttpPost]
        public async Task<ActionResult<ThongBao>> PostThongBao(ThongBao thongBao)
        {
            // kiểm tra người dùng có tồn tại không
            var exists = await _context.NguoiDungs.AnyAsync(n => n.Id == thongBao.Id_NguoiDung);
            if (!exists)
                return BadRequest("Người dùng không tồn tại.");

            thongBao.NgayTao = DateTime.Now;
            _context.ThongBaos.Add(thongBao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThongBao), new { id = thongBao.Id }, thongBao);
        }

        // ==================== PUT ====================

        // PUT: api/ThongBao/5/Read
        // Đánh dấu thông báo đã đọc
        [HttpPut("{id}/Read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao == null)
                return NotFound("Không tìm thấy thông báo.");

            thongBao.DaDoc = true;
            _context.ThongBaos.Update(thongBao);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã đánh dấu thông báo là đã đọc." });
        }

        // ==================== DELETE ====================

        // DELETE: api/ThongBao/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThongBao(int id)
        {
            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao == null)
                return NotFound("Không tìm thấy thông báo.");

            _context.ThongBaos.Remove(thongBao);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa thông báo thành công." });
        }
    }
}
