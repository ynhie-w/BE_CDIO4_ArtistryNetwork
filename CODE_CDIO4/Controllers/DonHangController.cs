using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;
using CODE_CDIO4.DTOs;
using Swashbuckle.AspNetCore.Annotations;

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
        // ================== XEM ĐƠN HÀNG THEO TÁC PHẨM (CHỦ SỞ HỮU) ==================
        [HttpGet("TacPham/{idTacPham}")]
        [SwaggerOperation(Summary = "Xem tất cả đơn hàng của một tác phẩm (chỉ chủ sở hữu xem được)")]
        [SwaggerResponse(200, "Lấy thành công", typeof(IEnumerable<DonHangView>))]
        [SwaggerResponse(400, "Không có quyền xem hoặc lỗi SP")]
        public async Task<ActionResult<IEnumerable<DonHangView>>> GetDonHangByTacPham(
            [FromRoute, SwaggerParameter("ID tác phẩm", Required = true)] int idTacPham,
            [FromQuery, SwaggerParameter("ID chủ sở hữu", Required = true)] int idNguoiTao)
        {
            try
            {
                var paramTacPham = new SqlParameter("@id_tacpham", idTacPham);
                var paramNguoiTao = new SqlParameter("@id_nguoitao", idNguoiTao);

                var result = await _context.Set<DonHangView>()
                    .FromSqlRaw("EXEC sp_XemDonHang_TacPham @id_tacpham, @id_nguoitao", paramTacPham, paramNguoiTao)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================== XEM ĐƠN HÀNG THEO NGƯỜI MUA ==================
        [HttpGet("NguoiMua/{idNguoiMua}")]
        [SwaggerOperation(Summary = "Xem tất cả đơn hàng của một người mua (chỉ chính họ xem được)")]
        [SwaggerResponse(200, "Lấy thành công", typeof(IEnumerable<DonHangView>))]
        [SwaggerResponse(400, "Không có quyền xem hoặc lỗi SP")]
        public async Task<ActionResult<IEnumerable<DonHangView>>> GetDonHangByNguoiMua(
            [FromRoute, SwaggerParameter("ID người mua", Required = true)] int idNguoiMua,
            [FromQuery, SwaggerParameter("ID yêu cầu (chính người mua)", Required = true)] int idYeuCau)
        {
            try
            {
                var paramNguoiMua = new SqlParameter("@id_nguoimua", idNguoiMua);
                var paramYeuCau = new SqlParameter("@id_yeucau", idYeuCau);

                var result = await _context.Set<DonHangView>()
                    .FromSqlRaw("EXEC sp_XemDonHang_NguoiMua @id_nguoimua, @id_yeucau", paramNguoiMua, paramYeuCau)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //============== Xem đơn hàng của tác giả (tất cả đơn có tác phẩm của tác giả này)====
        [HttpGet("TacGia/{idTacGia}")]
        [SwaggerOperation(Summary = "Xem tất cả đơn hàng chứa tác phẩm của tác giả")]
        public async Task<IActionResult> GetByTacGia(int idTacGia)
        {
            var result = await _context.DonHangViews
                .FromSqlRaw("EXEC get_DonHang_ByTacGia @p0", idTacGia)
                .ToListAsync();
            return Ok(result);
        }
        // ================== TẠO ĐƠN HÀNG DÙNG SP ==================
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo đơn hàng mới. nhập id đơn hàng ví dụ [1,2,3]")]
        [SwaggerResponse(201, "Tạo thành công", typeof(DonHangDTO))]
        [SwaggerResponse(400, "Danh sách tác phẩm rỗng hoặc SP lỗi")]
        public async Task<ActionResult<DonHangDTO>> CreateDonHang(
            [FromQuery, SwaggerParameter("ID người mua", Required = true)] int idNguoiMua,
            [FromBody, SwaggerParameter("Danh sách ID tác phẩm", Required = true)] List<int> danhSachTacPham,
            [FromQuery, SwaggerParameter("ID giảm giá (nếu có)")] int? idGiamGia = null)
        {
            if (danhSachTacPham == null || !danhSachTacPham.Any())
                return BadRequest("Danh sách tác phẩm rỗng.");

            var table = new System.Data.DataTable();
            table.Columns.Add("id_tacpham", typeof(int));
            foreach (var id in danhSachTacPham)
                table.Rows.Add(id);

            var paramNguoiMua = new SqlParameter("@idNguoiMua", idNguoiMua);
            var paramDanhSach = new SqlParameter("@DanhSachTacPham", table)
            {
                SqlDbType = System.Data.SqlDbType.Structured,
                TypeName = "TacPhamList"
            };
            var paramIdGiamGia = new SqlParameter("@IdGiamGia", idGiamGia.HasValue ? idGiamGia.Value : (object)DBNull.Value);

            int newDonHangId;

            try
            {
                var result = await _context.Set<NewDonHangID>()
                    .FromSqlRaw("EXEC insert_DonHang @idNguoiMua, @DanhSachTacPham, @IdGiamGia",
                                paramNguoiMua, paramDanhSach, paramIdGiamGia)
                    .ToListAsync();

                newDonHangId = result.FirstOrDefault()?.NewDonHangIDs ?? 0;

                if (newDonHangId == 0)
                    return BadRequest("Không thể tạo đơn hàng.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var donHang = await _context.DonHangs
                .Include(dh => dh.DonHang_ChiTiets)
                    .ThenInclude(ct => ct.TacPham)
                .Include(dh => dh.GiamGias)
                .FirstOrDefaultAsync(dh => dh.Id == newDonHangId);

            if (donHang == null)
                return NotFound("Không tìm thấy đơn hàng vừa tạo.");

            var dto = new DonHangDTO
            {
                Id = donHang.Id,
                NgayMua = donHang.NgayMua,
                TrangThai = donHang.TrangThai,
                TongTien = donHang.TongTien,
                MaGiamGia = donHang.GiamGias != null ? donHang.GiamGias.MaGiamGia : null,
                DonHang_ChiTiets = donHang.DonHang_ChiTiets.Select(ct => new DonHangChiTietDTO
                {
                    Id_TacPham = ct.Id_TacPham,
                    TenTacPham = ct.TacPham.Ten,
                    Gia = ct.TacPham.Gia
                }).ToList()
            };

            return CreatedAtAction(nameof(GetDonHang), new { id = dto.Id }, dto);
        }

        // Helper class để nhận ID mới từ SP
        public class NewDonHangID
        {
            public int NewDonHangIDs { get; set; }
        }

        // ================== XÓA MỀM ĐƠN HÀNG ==================
        [HttpDelete("{id}")]
            [SwaggerOperation(Summary = "Xóa mềm đơn hàng (ẩn người mua)")]
            [SwaggerResponse(204, "Xóa thành công")]
            [SwaggerResponse(404, "Không tìm thấy đơn hàng")]
            [SwaggerResponse(400, "Lỗi khi xóa đơn hàng")]
            public async Task<IActionResult> DeleteDonHang(
                [FromRoute, SwaggerParameter("ID đơn hàng cần xóa", Required = true)] int id)
            {
                var donHang = await _context.DonHangs.FindAsync(id);
                if (donHang == null)
                    return NotFound("Không tìm thấy đơn hàng.");

                try
                {
                    // Gọi stored procedure delete_DonHang
                    var param = new SqlParameter("@id", id);
                    await _context.Database.ExecuteSqlRawAsync("EXEC delete_DonHang @id", param);

                    return NoContent();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            // ================== LẤY ĐƠN HÀNG THEO ID ==================
            [HttpGet("{id}")]
            [SwaggerOperation(Summary = "Lấy thông tin đơn hàng theo ID")]
            [SwaggerResponse(200, "Lấy thành công", typeof(DonHangDTO))]
            [SwaggerResponse(404, "Không tìm thấy đơn hàng")]
            public async Task<ActionResult<DonHangDTO>> GetDonHang(
                [FromRoute, SwaggerParameter("ID đơn hàng", Required = true)] int id)
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

