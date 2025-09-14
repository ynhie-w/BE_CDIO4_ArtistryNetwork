using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;
using CODE_CDIO4.Services;
using Swashbuckle.AspNetCore.Annotations;
using CODE_CDIO4.DTOs;
using Microsoft.Data.SqlClient;

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

        // ==================== TẠO MỚI BÌNH LUẬN (DTO) ====================
        [HttpPost]
        [SwaggerOperation(
            Summary = "Tạo một bình luận mới",
            Description = "Thêm một bình luận mới vào cơ sở dữ liệu và gửi thông báo cho tác giả của tác phẩm."
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Bình luận được tạo thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ (IdNguoiDung hoặc IdTacPham không tồn tại).")]
        [HttpPost]
        public async Task<ActionResult<BinhLuan>> PostBinhLuan([FromBody] BinhLuanInsertDTO dto)
        {
            // Kiểm tra dữ liệu đầu vào
            var nguoiDung = await _context.NguoiDungs.FindAsync(dto.IdNguoiDung);
            var tacPham = await _context.TacPhams.FindAsync(dto.IdTacPham);

            if (nguoiDung == null || tacPham == null)
                return BadRequest("IdNguoiDung hoặc IdTacPham không hợp lệ.");

            // Dùng stored procedure để insert bình luận và lấy ID
            int newBinhLuanId;
            var connection = _context.Database.GetDbConnection();
            await using (var command = connection.CreateCommand())
            {
                command.CommandText = "insert_BinhLuan";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@id_tacpham", dto.IdTacPham));
                command.Parameters.Add(new SqlParameter("@id_nguoidung", dto.IdNguoiDung));
                command.Parameters.Add(new SqlParameter("@noidung", dto.NoiDung));
                command.Parameters.Add(new SqlParameter("@level", dto.Level));

                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                var result = await command.ExecuteScalarAsync();
                newBinhLuanId = Convert.ToInt32(result); // chuyển object? sang int
            }

            if (newBinhLuanId == 0)
                return StatusCode(500, "Không thể thêm bình luận.");

            // Tạo thông báo cho chủ tác phẩm
            await _notificationService.CreateThongBaoAsync(
                tacPham.Id_NguoiTao,
                $"{nguoiDung.Ten} đã bình luận về tác phẩm của bạn."
            );

            // Lấy lại bình luận vừa tạo để trả về
            var newBinhLuan = await _context.BinhLuans.FindAsync(newBinhLuanId);

            return CreatedAtAction(nameof(GetBinhLuan), new { id = newBinhLuan.Id }, newBinhLuan);
        }


        // ==================== CẬP NHẬT BÌNH LUẬN ====================
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật nội dung bình luận", Description = "Chỉ cập nhật nội dung của bình luận dựa trên ID và DTO.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Cập nhật thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "ID không khớp hoặc dữ liệu không hợp lệ.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Người dùng không có quyền sửa bình luận.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy bình luận.")]
        public async Task<IActionResult> PutBinhLuan(int id, [FromBody] BinhLuanUpdateDTO dto)
        {
            var binhLuan = await _context.BinhLuans.FindAsync(id);
            if (binhLuan == null)
                return NotFound("Không tìm thấy bình luận.");

            // Kiểm tra quyền: chỉ người tạo bình luận mới được sửa
            if (binhLuan.Id_NguoiDung != dto.IdNguoiDung)
                return Forbid("Bạn không có quyền sửa bình luận này.");

            // Gọi stored procedure chỉ update nội dung
            var connection = _context.Database.GetDbConnection();
            await using (var command = connection.CreateCommand())
            {
                command.CommandText = "update_BinhLuan";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@id_binhluan", id));
                command.Parameters.Add(new SqlParameter("@id_nguoidung", dto.IdNguoiDung));
                command.Parameters.Add(new SqlParameter("@noidungmoi", dto.NoiDungMoi));

                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {
                    return BadRequest(ex.Message); // Lỗi từ RAISERROR trong SP
                }
            }

            return NoContent();
        }


        // ==================== XÓA BÌNH LUẬN ====================

        [HttpDelete("{id}")]
        [SwaggerOperation(
    Summary = "Xóa một bình luận (xóa mềm)",
    Description = "Chỉ người tạo bình luận hoặc chủ tác phẩm mới có quyền xóa. Dùng stored procedure để xóa mềm."
)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Xóa thành công.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Người dùng không có quyền xóa bình luận.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy bình luận.")]
        public async Task<IActionResult> DeleteBinhLuan(int id, [FromBody] BinhLuanDeleteDTO dto)
        {
            var binhLuan = await _context.BinhLuans.FindAsync(id);
            if (binhLuan == null)
                return NotFound("Không tìm thấy bình luận.");

            var connection = _context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = "delete_BinhLuan";
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@id_binhluan", id));
            command.Parameters.Add(new SqlParameter("@id_nguoidung", dto.IdNguoiDung));

            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                return Forbid(ex.Message); // RAISERROR trong SP
            }

            return NoContent();
        }

        // ==================== HỖ TRỢ ====================

        private bool BinhLuanExists(int id)
        {
            return _context.BinhLuans.Any(e => e.Id == id);
        }


    }
}