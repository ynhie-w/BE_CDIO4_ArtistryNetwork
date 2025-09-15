using CODE_CDIO4.Models;
using CODE_CDIO4.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using CODE_CDIO4.Repository;

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

        // ==================== GET ALL ====================
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy tất cả hóa đơn (bao gồm chi tiết)")]
        public async Task<ActionResult<IEnumerable<HoaDonDTO>>> GetAll()
        {
            var data = await _context.HoaDons
                .Include(h => h.HoaDon_ChiTiets)
                .Where(h => h.TrangThai == true)
                .Select(h => new HoaDonDTO
                {
                    Id = h.Id,
                    Id_DonHang = h.Id_DonHang,
                    SoHoaDon = h.SoHoaDon,
                    NgayLap = h.NgayLap,
                    TenNguoiLap = h.NguoiLapHoaDon.Ten,
                    ThanhTien = h.ThanhTien,
                    GhiChu = h.GhiChu,
                    TrangThai = h.TrangThai,
                    ChiTiet = h.HoaDon_ChiTiets.Select(c => new HoaDonChiTietDTO
                    {
                        Id = c.Id,
                        Id_HoaDon = c.Id_HoaDon,
                        Id_TacPham = c.Id_TacPham,
                        ThanhTien = c.ThanhTien,
                        TrangThai = c.TrangThai
                    }).ToList()
                })
                .ToListAsync();

            return Ok(data);
        }

        // ==================== GET BY ID ====================
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy hóa đơn theo ID (bao gồm chi tiết)")]
        public async Task<ActionResult<HoaDonDTO>> GetById(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.HoaDon_ChiTiets)
                .FirstOrDefaultAsync(h => h.Id == id && h.TrangThai == true);

            if (hoaDon == null) return NotFound("Không tìm thấy hóa đơn.");

            var dto = new HoaDonDTO
            {
                Id = hoaDon.Id,
                Id_DonHang = hoaDon.Id_DonHang,
                SoHoaDon = hoaDon.SoHoaDon,
                NgayLap = hoaDon.NgayLap,
                TenNguoiLap = hoaDon.NguoiLapHoaDon.Ten,
                ThanhTien = hoaDon.ThanhTien,
                GhiChu = hoaDon.GhiChu,
                TrangThai = hoaDon.TrangThai,
                ChiTiet = hoaDon.HoaDon_ChiTiets.Select(c => new HoaDonChiTietDTO
                {
                    Id = c.Id,
                    Id_HoaDon = c.Id_HoaDon,
                    Id_TacPham = c.Id_TacPham,
                    ThanhTien = c.ThanhTien,
                    TrangThai = c.TrangThai
                }).ToList()
            };

            return Ok(dto);
        }

        // ==================== CREATE ====================
        [HttpPost]
  
        [SwaggerOperation(Summary = "Tạo hóa đơn mới kèm chi tiết")]
        public async Task<ActionResult<HoaDonDTO>> Create(HoaDonDTO dto)
        {
            var hoaDon = new HoaDon
            {
                Id_DonHang = dto.Id_DonHang,
                SoHoaDon = dto.SoHoaDon,
                NgayLap = DateTime.Now,
                NguoiLap = dto.Id_NguoiLap,   // gán FK
                ThanhTien = dto.ThanhTien,
                GhiChu = dto.GhiChu,
                TrangThai = true,
                HoaDon_ChiTiets = dto.ChiTiet?.Select(c => new HoaDon_ChiTiet
                {
                    Id_TacPham = c.Id_TacPham,
                    ThanhTien = c.ThanhTien,
                    TrangThai = true
                }).ToList()
            };

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            // map lại sang DTO trả về
            dto.Id = hoaDon.Id;
            dto.TrangThai = hoaDon.TrangThai;
            dto.ChiTiet = hoaDon.HoaDon_ChiTiets.Select(c => new HoaDonChiTietDTO
            {
                Id = c.Id,
                Id_HoaDon = c.Id_HoaDon,
                Id_TacPham = c.Id_TacPham,
                ThanhTien = c.ThanhTien,
                TrangThai = c.TrangThai
            }).ToList();

            return CreatedAtAction(nameof(GetById), new { id = hoaDon.Id }, dto);
        }


        // ==================== UPDATE ====================
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật hóa đơn kèm chi tiết")]
        public async Task<IActionResult> Update(int id, HoaDonDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID không khớp.");

            var hoaDon = await _context.HoaDons
                .Include(h => h.HoaDon_ChiTiets)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hoaDon == null || hoaDon.TrangThai == false)
                return NotFound("Không tìm thấy hóa đơn.");

            // Cập nhật hóa đơn
            hoaDon.Id_DonHang = dto.Id_DonHang;
            hoaDon.SoHoaDon = dto.SoHoaDon;
            hoaDon.NgayLap = dto.NgayLap;
            hoaDon.NguoiLap = dto.Id_NguoiLap;
            hoaDon.ThanhTien = dto.ThanhTien;
            hoaDon.GhiChu = dto.GhiChu;

            // Đồng bộ chi tiết
            var chiTietIds = dto.ChiTiet?.Select(c => c.Id).ToList() ?? new List<int>();

            // Xóa chi tiết cũ không còn trong DTO
            var chiTietCanXoa = hoaDon.HoaDon_ChiTiets
                .Where(c => !chiTietIds.Contains(c.Id))
                .ToList();

                foreach (var c in chiTietCanXoa)
                {
                    hoaDon.HoaDon_ChiTiets.Remove(c);
                }


            // Cập nhật hoặc thêm mới
            foreach (var cDto in dto.ChiTiet ?? new List<HoaDonChiTietDTO>())
            {
                var existing = hoaDon.HoaDon_ChiTiets.FirstOrDefault(c => c.Id == cDto.Id);
                if (existing != null)
                {
                    existing.Id_TacPham = cDto.Id_TacPham;
                    existing.ThanhTien = cDto.ThanhTien;
                    existing.TrangThai = cDto.TrangThai;
                }
                else
                {
                    hoaDon.HoaDon_ChiTiets.Add(new HoaDon_ChiTiet
                    {
                        Id_TacPham = cDto.Id_TacPham,
                        ThanhTien = cDto.ThanhTien,
                        TrangThai = cDto.TrangThai
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==================== SOFT DELETE ====================
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa mềm hóa đơn (soft delete) kèm chi tiết")]
        public async Task<IActionResult> Delete(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.HoaDon_ChiTiets)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hoaDon == null) return NotFound("Không tìm thấy hóa đơn.");

            hoaDon.TrangThai = false;
            foreach (var c in hoaDon.HoaDon_ChiTiets)
                c.TrangThai = false;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa mềm hóa đơn và chi tiết thành công." });
        }
    }
}
