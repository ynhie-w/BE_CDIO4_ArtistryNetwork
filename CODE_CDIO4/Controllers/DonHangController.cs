using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;
using CODE_CDIO4.DTOs;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonHangController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DonHangController(AppDbContext context)
        {
            _context = context;
        }

        // ================= DTO =================
       

        // ================== TẠO ĐƠN HÀNG DÙNG SP ==================
        [HttpPost]
        public async Task<ActionResult<DonHangDTO>> CreateDonHang(int idNguoiMua, [FromBody] List<int> danhSachTacPham, decimal giamGia = 0)
        {
            if (danhSachTacPham == null || !danhSachTacPham.Any())
                return BadRequest("Danh sách tác phẩm rỗng.");

            // Tạo DataTable để truyền TVP
            var table = new System.Data.DataTable();
            table.Columns.Add("id_tacpham", typeof(int));
            foreach (var id in danhSachTacPham)
            {
                table.Rows.Add(id);
            }

            var paramNguoiMua = new SqlParameter("@idNguoiMua", idNguoiMua);
            var paramDanhSach = new SqlParameter("@DanhSachTacPham", table)
            {
                SqlDbType = System.Data.SqlDbType.Structured,
                TypeName = "TacPhamList"
            };
            var paramGiamGia = new SqlParameter("@giamgia", giamGia);

            int newDonHangId;

            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC insert_DonHang @idNguoiMua, @DanhSachTacPham, @giamgia",
                    paramNguoiMua, paramDanhSach, paramGiamGia
                );

                // Lấy Id vừa tạo từ SP
                newDonHangId = await _context.DonHangs
                    .OrderByDescending(dh => dh.Id)
                    .Select(dh => dh.Id)
                    .FirstAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            // Trả về DTO
            var donHang = await _context.DonHangs
                .Include(dh => dh.DonHang_ChiTiets)
                    .ThenInclude(ct => ct.TacPham)
                .FirstOrDefaultAsync(dh => dh.Id == newDonHangId);

            if (donHang == null)
                return NotFound("Không tìm thấy đơn hàng vừa tạo.");

            var dto = new DonHangDTO
            {
                Id = donHang.Id,
                NgayMua = donHang.NgayMua,
                TrangThai = donHang.TrangThai,
                TongTien = donHang.TongTien,
                DonHang_ChiTiets = donHang.DonHang_ChiTiets.Select(ct => new DonHangChiTietDTO
                {
                    Id_TacPham = ct.Id_TacPham,
                    TenTacPham = ct.TacPham.Ten,
                    Gia = ct.TacPham.Gia
                }).ToList()
            };

            return CreatedAtAction(nameof(GetDonHang), new { id = dto.Id }, dto);
        }

        // ================== LẤY ĐƠN HÀNG THEO ID ==================
        [HttpGet("{id}")]
        public async Task<ActionResult<DonHangDTO>> GetDonHang(int id)
        {
            var donHang = await _context.DonHangs
                .Include(dh => dh.DonHang_ChiTiets)
                    .ThenInclude(ct => ct.TacPham)
                .FirstOrDefaultAsync(dh => dh.Id == id);

            if (donHang == null)
                return NotFound("Không tìm thấy đơn hàng.");

            var dto = new DonHangDTO
            {
                Id = donHang.Id,
                NgayMua = donHang.NgayMua,
                TrangThai = donHang.TrangThai,
                TongTien = donHang.TongTien,
                DonHang_ChiTiets = donHang.DonHang_ChiTiets.Select(ct => new DonHangChiTietDTO
                {
                    Id_TacPham = ct.Id_TacPham,
                    TenTacPham = ct.TacPham.Ten,
                    Gia = ct.TacPham.Gia
                }).ToList()
            };

            return Ok(dto);
        }
    }
}
