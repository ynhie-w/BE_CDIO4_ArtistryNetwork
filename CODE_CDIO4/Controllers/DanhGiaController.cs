using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;
using CODE_CDIO4.DTOs;
using CODE_CDIO4.Services;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

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
        // ==================== LẤY TẤT CẢ ====================
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy tất cả đánh giá (đang hoạt động)")]
        [SwaggerResponse(200, "Lấy thành công", typeof(IEnumerable<DanhGiaDTO>))]
        [SwaggerResponse(404, "Không có đánh giá nào")]
        public async Task<ActionResult<IEnumerable<DanhGiaDTO>>> GetAllDanhGia()
        {
            var danhGias = await _context.DanhGias
                                         .Where(dg => dg.TrangThai == true)
                                         .Include(dg => dg.NguoiDanhGia)
                                         .OrderByDescending(dg => dg.NgayTao)
                                         .ToListAsync();

            if (!danhGias.Any())
                return NotFound("Không có đánh giá nào.");

            var dtos = danhGias.Select(dg => new DanhGiaDTO
            {
                Id = dg.Id,
                Id_TacPham = dg.Id_TacPham,
                Id_NguoiDung = dg.Id_NguoiDung,
                Diem = dg.Diem,
                NgayTao = dg.NgayTao,
                TenNguoiDung = dg.NguoiDanhGia?.Ten
            }).ToList();

            return Ok(dtos);
        }

        // ==================== LẤY DANH SÁCH THEO TACPHAM ====================
        [HttpGet("TacPham/{idTacPham}")]
        [SwaggerOperation(Summary = "Lấy danh sách đánh giá theo tác phẩm")]
        [SwaggerResponse(200, "Lấy thành công", typeof(IEnumerable<DanhGiaDTO>))]
        [SwaggerResponse(404, "Không tìm thấy đánh giá")]
        public async Task<ActionResult<IEnumerable<DanhGiaDTO>>> GetDanhGiaByTacPham(int idTacPham)
        {
            var danhGias = await _context.DanhGias
                                         .Where(dg => dg.Id_TacPham == idTacPham && dg.TrangThai == true)
                                         .Include(dg => dg.NguoiDanhGia)
                                         .OrderByDescending(dg => dg.NgayTao)
                                         .ToListAsync();

            if (!danhGias.Any())
                return NotFound("Không tìm thấy đánh giá nào cho tác phẩm này.");

            var dtos = danhGias.Select(dg => new DanhGiaDTO
            {
                Id = dg.Id,
                Id_TacPham = dg.Id_TacPham,
                Diem = dg.Diem,
                NgayTao = dg.NgayTao,
                TenNguoiDung = dg.NguoiDanhGia?.Ten
            }).ToList();

            return Ok(dtos);
        }

        // ==================== LẤY THEO ID ====================
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy chi tiết đánh giá theo ID")]
        [SwaggerResponse(200, "Lấy thành công", typeof(DanhGiaDTO))]
        [SwaggerResponse(404, "Không tìm thấy đánh giá")]
        public async Task<ActionResult<DanhGiaDTO>> GetDanhGiaById(int id)
        {
            var danhGia = await _context.DanhGias
                                        .Include(dg => dg.NguoiDanhGia)
                                        .FirstOrDefaultAsync(dg => dg.Id == id && dg.TrangThai == true);

            if (danhGia == null)
                return NotFound("Không tìm thấy đánh giá này.");

            var dto = new DanhGiaDTO
            {
                Id = danhGia.Id,
                Id_TacPham = danhGia.Id_TacPham,
                Diem = danhGia.Diem,
                NgayTao = danhGia.NgayTao,
                TenNguoiDung = danhGia.NguoiDanhGia?.Ten
            };

            return Ok(dto);
        }

        // ==================== TẠO ĐÁNH GIÁ ====================
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo đánh giá mới cho tác phẩm")]
        [SwaggerResponse(201, "Tạo thành công", typeof(DanhGiaDTO))]
        [SwaggerResponse(400, "ID người dùng hoặc tác phẩm không hợp lệ")]
        public async Task<IActionResult> InsertDanhGia([FromBody] DanhGiaInsertDTO dto)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(dto.Id_NguoiDung);
            var tacPham = await _context.TacPhams.FindAsync(dto.Id_TacPham);

            if (nguoiDung == null || tacPham == null)
                return BadRequest("ID người dùng hoặc ID tác phẩm không hợp lệ.");

            // Kiểm tra đã đánh giá chưa
            var existing = await _context.DanhGias
                .FirstOrDefaultAsync(dg => dg.Id_NguoiDung == dto.Id_NguoiDung
                                        && dg.Id_TacPham == dto.Id_TacPham
                                        && dg.TrangThai == true);

            if (existing != null)
                return BadRequest("Bạn đã đánh giá tác phẩm này rồi.");

            var danhGia = new DanhGia
            {
                Id_TacPham = dto.Id_TacPham,
                Id_NguoiDung = dto.Id_NguoiDung,
                Diem = dto.Diem,
                NgayTao = DateTime.UtcNow,
                TrangThai = true
            };

            _context.DanhGias.Add(danhGia);
            await _context.SaveChangesAsync();

            // Gửi thông báo cho chủ tác phẩm
            await _notificationService.CreateThongBaoAsync(
                tacPham.Id_NguoiTao,
                $"{nguoiDung.Ten} đã đánh giá tác phẩm của bạn."
            );

            return CreatedAtAction(nameof(GetDanhGiaById), new { id = danhGia.Id }, new DanhGiaDTO
            {
                Id = danhGia.Id,
                Id_TacPham = danhGia.Id_TacPham,
                Id_NguoiDung = danhGia.Id_NguoiDung,
                Diem = danhGia.Diem,
                NgayTao = danhGia.NgayTao,
                TenNguoiDung = nguoiDung.Ten
            });
        }

        // ==================== CẬP NHẬT ĐÁNH GIÁ ====================
        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật đánh giá đã tồn tại")]
        [SwaggerResponse(200, "Cập nhật thành công", typeof(DanhGiaDTO))]
        [SwaggerResponse(400, "Không có quyền cập nhật")]
        [SwaggerResponse(404, "Không tìm thấy đánh giá")]
        public async Task<IActionResult> UpdateDanhGia([FromBody] DanhGiaUpdateDTO dto)
        {
            var danhGia = await _context.DanhGias.FindAsync(dto.Id);
            if (danhGia == null || danhGia.TrangThai == false)
                return NotFound("Không tìm thấy đánh giá để cập nhật.");

            if (danhGia.Id_NguoiDung != dto.Id_NguoiDung)
                return BadRequest("Bạn không có quyền cập nhật đánh giá này.");

            danhGia.Diem = dto.Diem;
            danhGia.NgayTao = DateTime.UtcNow;

            _context.DanhGias.Update(danhGia);
            await _context.SaveChangesAsync();

            var tacPham = await _context.TacPhams.FindAsync(danhGia.Id_TacPham);
            var nguoiDung = await _context.NguoiDungs.FindAsync(danhGia.Id_NguoiDung);

            await _notificationService.CreateThongBaoAsync(
                tacPham!.Id_NguoiTao,
                $"{nguoiDung!.Ten} đã cập nhật đánh giá cho tác phẩm của bạn."
            );

            return Ok(new DanhGiaDTO
            {
                Id = danhGia.Id,
                Id_TacPham = danhGia.Id_TacPham,
                Id_NguoiDung = danhGia.Id_NguoiDung,
                Diem = danhGia.Diem,
                NgayTao = danhGia.NgayTao,
                TenNguoiDung = nguoiDung.Ten
            });
        }

        // ==================== XÓA MỀM ====================
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa mềm đánh giá")]
        [SwaggerResponse(204, "Xóa thành công")]
        [SwaggerResponse(400, "Không có quyền xóa")]
        [SwaggerResponse(404, "Không tìm thấy đánh giá")]
        public async Task<IActionResult> DeleteDanhGia(int id, [FromQuery] int idNguoiDung)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null || danhGia.TrangThai == false)
                return NotFound("Không tìm thấy đánh giá để xóa.");

            if (danhGia.Id_NguoiDung != idNguoiDung)
                return BadRequest("Bạn không có quyền xóa đánh giá này.");

            danhGia.TrangThai = false;
            _context.DanhGias.Update(danhGia);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

   
}
