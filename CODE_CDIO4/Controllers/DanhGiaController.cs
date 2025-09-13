using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;
using CODE_CDIO4.DTOs;
using CODE_CDIO4.Services;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DanhGiaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService; 

        public DanhGiaController(AppDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // ==================== CRUD ====================
        [HttpGet("TacPham/{idTacPham}")]
        public async Task<ActionResult<IEnumerable<DanhGiaDto>>> GetDanhGiaByTacPham(int idTacPham)
        {
            var danhGias = await _context.DanhGias
                                         .Where(dg => dg.Id_TacPham == idTacPham)
                                         .Include(dg => dg.NguoiDanhGia)
                                         .OrderByDescending(dg => dg.NgayTao)
                                         .ToListAsync();

            if (!danhGias.Any())
            {
                return NotFound("Không tìm thấy đánh giá nào cho tác phẩm này.");
            }

            var danhGiaDtos = danhGias.Select(dg => new DanhGiaDto
            {
                Id = dg.Id,
                Id_TacPham = dg.Id_TacPham,
                Diem = dg.Diem,
                NgayTao = dg.NgayTao,
                TenNguoiDung = dg.NguoiDanhGia?.Ten
            }).ToList();

            return Ok(danhGiaDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DanhGiaDto>> GetDanhGiaById(int id)
        {
            var danhGia = await _context.DanhGias
                                        .Include(dg => dg.NguoiDanhGia)
                                        .FirstOrDefaultAsync(dg => dg.Id == id);

            if (danhGia == null)
            {
                return NotFound("Không tìm thấy đánh giá này.");
            }

            var dto = new DanhGiaDto
            {
                Id = danhGia.Id,
                Id_TacPham = danhGia.Id_TacPham,
                Diem = danhGia.Diem,
                NgayTao = danhGia.NgayTao,
                TenNguoiDung = danhGia.NguoiDanhGia?.Ten
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertDanhGia([FromBody] DanhGia danhGia)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(danhGia.Id_NguoiDung);
            var tacPham = await _context.TacPhams.FindAsync(danhGia.Id_TacPham);

            if (nguoiDung == null || tacPham == null)
                return BadRequest("ID người dùng hoặc ID tác phẩm không hợp lệ.");

            var existingDanhGia = await _context.DanhGias
                .Include(dg => dg.NguoiDanhGia)
                .FirstOrDefaultAsync(dg => dg.Id_NguoiDung == danhGia.Id_NguoiDung
                                        && dg.Id_TacPham == danhGia.Id_TacPham);

            if (existingDanhGia == null)
            {
                danhGia.NgayTao = DateTime.UtcNow;
                _context.DanhGias.Add(danhGia);
                await _context.SaveChangesAsync();

                // ✅ gửi thông báo cho chủ sở hữu tác phẩm
                await _notificationService.CreateThongBaoAsync(
                    tacPham.Id_NguoiTao,
                    $"{nguoiDung.Ten} đã đánh giá tác phẩm của bạn."
                );

                var dto = new DanhGiaDto
                {
                    Id = danhGia.Id,
                    Id_TacPham = danhGia.Id_TacPham,
                    Diem = danhGia.Diem,
                    NgayTao = danhGia.NgayTao,
                    TenNguoiDung = nguoiDung?.Ten
                };

                return CreatedAtAction(nameof(GetDanhGiaById), new { id = dto.Id }, dto);
            }
            else
            {
                existingDanhGia.Diem = danhGia.Diem;
                existingDanhGia.NgayTao = DateTime.UtcNow;

                _context.DanhGias.Update(existingDanhGia);
                await _context.SaveChangesAsync();

                // ✅ gửi thông báo cập nhật
                await _notificationService.CreateThongBaoAsync(
                    tacPham.Id_NguoiTao,
                    $"{nguoiDung.Ten} đã cập nhật đánh giá cho tác phẩm của bạn."
                );

                var dto = new DanhGiaDto
                {
                    Id = existingDanhGia.Id,
                    Id_TacPham = existingDanhGia.Id_TacPham,
                    Diem = existingDanhGia.Diem,
                    NgayTao = existingDanhGia.NgayTao,
                    TenNguoiDung = existingDanhGia.NguoiDanhGia?.Ten
                };

                return Ok(dto);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDanhGia(int id)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null)
            {
                return NotFound("Không tìm thấy đánh giá để xóa.");
            }

            _context.DanhGias.Remove(danhGia);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
