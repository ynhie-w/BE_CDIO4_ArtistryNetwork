using CODE_CDIO4.Models;
using CODE_CDIO4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace CODE_CDIO4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TacPhamController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TacPhamController(AppDbContext context)
        {
            _context = context;
        }

        // ================== INSERT ==================
        [HttpPost("insert")]
        [SwaggerOperation(Summary = "Thêm tác phẩm mới", Description = "Thêm một tác phẩm mới vào hệ thống (gồm tên, mô tả, ảnh, thể loại, giá, trạng thái).")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thêm tác phẩm thành công")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ")]
        public async Task<IActionResult> InsertTacPham([FromForm] TacPham model, [FromForm] string tenTheLoai, [FromForm] string hashtags)
        {
            var parameters = new[]
            {
                new SqlParameter("@ten", model.Ten),
                new SqlParameter("@mota", (object?)model.MoTa ?? DBNull.Value),
                new SqlParameter("@anh", model.Anh),
                new SqlParameter("@ten_theloai", tenTheLoai),
                new SqlParameter("@hashtags", hashtags ?? string.Empty),
                new SqlParameter("@trangthai", model.TrangThai),
                new SqlParameter("@id_nguoitao", model.Id_NguoiTao),
                new SqlParameter("@gia", (object?)model.Gia ?? DBNull.Value)
            };

            var result = await _context.TacPhams
                .FromSqlRaw("EXEC insert_TacPham @ten, @mota, @anh, @ten_theloai, @hashtags, @trangthai, @id_nguoitao, @gia", parameters)
                .ToListAsync();

            return Ok(new { message = "Thêm tác phẩm thành công", data = result });
        }

        // ================== UPDATE ==================
        [HttpPut("update/{id}")]
        [SwaggerOperation(Summary = "Cập nhật tác phẩm", Description = "Cập nhật thông tin của một tác phẩm theo ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Cập nhật tác phẩm thành công")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy tác phẩm")]
        public async Task<IActionResult> UpdateTacPham(int id, [FromForm] TacPham model, [FromForm] string tenTheLoai, [FromForm] string hashtags)
        {
            var parameters = new[]
            {
                new SqlParameter("@id", id),
                new SqlParameter("@ten", model.Ten),
                new SqlParameter("@mota", (object?)model.MoTa ?? DBNull.Value),
                new SqlParameter("@anh", model.Anh),
                new SqlParameter("@ten_theloai", tenTheLoai),
                new SqlParameter("@hashtags", hashtags ?? string.Empty),
                new SqlParameter("@trangthai", model.TrangThai),
                new SqlParameter("@gia", (object?)model.Gia ?? DBNull.Value)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC update_TacPham @id, @ten, @mota, @anh, @ten_theloai, @hashtags, @trangthai, @gia", parameters);

            return Ok(new { message = "Cập nhật tác phẩm thành công" });
        }

        // ================== DELETE ==================
        [HttpDelete("delete/{id}")]
        [SwaggerOperation(Summary = "Xóa tác phẩm", Description = "Xóa một tác phẩm theo ID, kiểm tra quyền của người thực hiện.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Xóa tác phẩm thành công")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Không có quyền xóa tác phẩm")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy tác phẩm")]
        public async Task<IActionResult> DeleteTacPham(int id, [FromQuery] int idNguoiThucHien)
        {
            var parameters = new[]
            {
                new SqlParameter("@id", id),
                new SqlParameter("@id_nguoithuchien", idNguoiThucHien)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC delete_TacPham @id, @id_nguoithuchien", parameters);

            return Ok(new { message = "Xóa tác phẩm thành công" });
        }
    }
}
