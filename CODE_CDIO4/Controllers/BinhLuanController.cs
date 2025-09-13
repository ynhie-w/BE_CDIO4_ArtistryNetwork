using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;
using CODE_CDIO4.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BinhLuanController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;

        public BinhLuanController(AppDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }
        // ==================== LẤY TẤT CẢ BÌNH LUẬN ====================

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lấy tất cả bình luận",
            Description = "Trả về danh sách tất cả bình luận trong hệ thống (bao gồm thông tin người dùng)."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công, trả về danh sách bình luận.")]
        public async Task<ActionResult<IEnumerable<BinhLuan>>> GetAllBinhLuans()
        {
            var binhLuans = await _context.BinhLuans
                .Include(b => b.NguoiBinhLuan)
                .Include(b => b.TacPham)
                .OrderByDescending(b => b.NgayTao)
                .ToListAsync();

            return Ok(binhLuans);
        }

        // ==================== LẤY BÌNH LUẬN theo TÁC PHẨM ====================

        [HttpGet("TacPham/{idTacPham}")]
        [SwaggerOperation(Summary = "Lấy tất cả bình luận của một tác phẩm", Description = "Trả về danh sách tất cả bình luận (bao gồm thông tin người dùng) của một tác phẩm dựa trên ID tác phẩm.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công, trả về danh sách bình luận.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy bình luận nào cho tác phẩm này.")]
        public async Task<ActionResult<IEnumerable<BinhLuan>>> GetBinhLuansByTacPham(int idTacPham)
        {
            var binhLuans = await _context.BinhLuans
                .Where(b => b.Id_TacPham == idTacPham)
                .Include(b => b.NguoiBinhLuan)
                .OrderByDescending(b => b.NgayTao)
                .ToListAsync();

            if (!binhLuans.Any())
            {
                return NotFound("Không tìm thấy bình luận nào cho tác phẩm này.");
            }

            return Ok(binhLuans);
        }
        // ==================== LẤY BÌNH LUẬN cụ thể ====================
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy một bình luận cụ thể", Description = "Trả về thông tin chi tiết của một bình luận dựa trên ID bình luận.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công, trả về bình luận cụ thể.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy bình luận.")]
        public async Task<ActionResult<BinhLuan>> GetBinhLuan(int id)
        {
            var binhLuan = await _context.BinhLuans.FindAsync(id);

            if (binhLuan == null)
            {
                return NotFound();
            }

            return Ok(binhLuan);
        }

        // ==================== TẠO MỚI BÌNH LUẬN ====================

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo một bình luận mới", Description = "Thêm một bình luận mới vào cơ sở dữ liệu và gửi thông báo cho tác giả của tác phẩm.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Bình luận được tạo thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ (Id_NguoiDung hoặc Id_TacPham không tồn tại).")]
        public async Task<ActionResult<BinhLuan>> PostBinhLuan(BinhLuan binhLuan)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(binhLuan.Id_NguoiDung);
            var tacPham = await _context.TacPhams.FindAsync(binhLuan.Id_TacPham);

            if (nguoiDung == null || tacPham == null)
            {
                return BadRequest("Id_NguoiDung hoặc Id_TacPham không hợp lệ.");
            }

            binhLuan.NgayTao = DateTime.Now;
            _context.BinhLuans.Add(binhLuan);
            await _context.SaveChangesAsync();

            await _notificationService.CreateThongBaoAsync(
                tacPham.Id_NguoiTao,
                $"{nguoiDung.Ten} đã bình luận về tác phẩm của bạn."
            );

            return CreatedAtAction(nameof(GetBinhLuan), new { id = binhLuan.Id }, binhLuan);
        }

        // ==================== CẬP NHẬT BÌNH LUẬN ====================

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật một bình luận", Description = "Cập nhật nội dung của một bình luận đã có dựa trên ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Cập nhật thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "ID không khớp.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy bình luận cần cập nhật.")]
        public async Task<IActionResult> PutBinhLuan(int id, BinhLuan binhLuan)
        {
            if (id != binhLuan.Id)
            {
                return BadRequest("ID không khớp.");
            }

            // ⚠️ Cần thêm logic xác minh quyền sở hữu ở đây

            _context.Entry(binhLuan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BinhLuanExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // ==================== XÓA BÌNH LUẬN ====================

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa một bình luận", Description = "Xóa một bình luận khỏi cơ sở dữ liệu dựa trên ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Xóa thành công.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy bình luận cần xóa.")]
        public async Task<IActionResult> DeleteBinhLuan(int id)
        {
            var binhLuan = await _context.BinhLuans.FindAsync(id);
            if (binhLuan == null)
            {
                return NotFound();
            }
            _context.BinhLuans.Remove(binhLuan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ==================== HỖ TRỢ ====================

        private bool BinhLuanExists(int id)
        {
            return _context.BinhLuans.Any(e => e.Id == id);
        }
    }
}