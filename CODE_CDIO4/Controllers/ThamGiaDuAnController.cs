using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using Microsoft.Data.SqlClient;
using CODE_CDIO4.Repository;
using CODE_CDIO4.DTOs;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThamGiaDuAnController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ThamGiaDuAnController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ThamGiaDuAn/{idDuAn}
        [HttpGet("{idDuAn}")]
        [SwaggerOperation(Summary = "Lấy danh sách người tham gia dự án")]
        public async Task<ActionResult<IEnumerable<ThamGiaDuAn>>> GetByDuAn(int idDuAn)
        {
            var list = await _context.ThamGiaDuAns
                                     .Where(t => t.Id_DuAn == idDuAn && t.TrangThai == true)
                                     .Include(t => t.NguoiDung)
                                     .ToListAsync();

            if (!list.Any())
                return NotFound("Không có ai tham gia dự án này.");

            return Ok(list);
        }

        // POST: api/ThamGiaDuAn/Insert
        [HttpPost("Insert")]
        [SwaggerOperation(Summary = "Thêm người dùng vào dự án (gọi proc insert_ThamGiaDuAn)")]
        public async Task<IActionResult> Insert([FromBody] ThamGiaDuAnDTO dto)
        {
            var parameters = new[]
            {
                new SqlParameter("@id_duan", dto.Id_DuAn),
                new SqlParameter("@id_nguoidung", dto.Id_NguoiDung),
                new SqlParameter("@vaitro", dto.VaiTro)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC insert_ThamGiaDuAn @id_duan, @id_nguoidung, @vaitro", parameters);
                return Ok(new { message = "Thêm người tham gia thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi thêm người tham gia", error = ex.Message });
            }
        }

        // PUT: api/ThamGiaDuAn/Update
        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Sửa vai trò người tham gia (proc update_ThamGiaDuAn)")]
        public async Task<IActionResult> Update([FromBody] UpdateThamGiaDTO dto)
        {
            var parameters = new[]
            {
                new SqlParameter("@id_duan", dto.Id_DuAn),
                new SqlParameter("@id_nguoidung", dto.Id_NguoiDungQuanLy),
                new SqlParameter("@id_nguoidung_thamgia", dto.Id_NguoiDungThamGia),
                new SqlParameter("@vaitro", dto.VaiTro)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC update_ThamGiaDuAn @id_duan, @id_nguoidung, @id_nguoidung_thamgia, @vaitro", parameters);
                return Ok(new { message = "Cập nhật vai trò thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật vai trò", error = ex.Message });
            }
        }

        // DELETE: api/ThamGiaDuAn/Delete
        [HttpDelete("Delete")]
        [SwaggerOperation(Summary = "Xóa mềm người tham gia (proc deleteSoft_ThamGiaDuAn)")]
        public async Task<IActionResult> Delete([FromBody] DeleteThamGiaDTO dto)
        {
            var parameters = new[]
            {
                new SqlParameter("@id_duan", dto.Id_DuAn),
                new SqlParameter("@id_nguoidung", dto.Id_NguoiDungQuanLy),
                new SqlParameter("@id_nguoidung_thamgia", dto.Id_NguoiDungThamGia)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC deleteSoft_ThamGiaDuAn @id_duan, @id_nguoidung, @id_nguoidung_thamgia", parameters);
                return Ok(new { message = "Xóa mềm người tham gia thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa người tham gia", error = ex.Message });
            }
        }
    }
}
