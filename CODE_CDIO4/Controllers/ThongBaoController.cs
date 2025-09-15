using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Models;
using CODE_CDIO4.DTOs;
using Swashbuckle.AspNetCore.Annotations;
using CODE_CDIO4.Repository;

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

        // GET: api/ThongBao/NguoiDung/5
        [HttpGet("NguoiDung/{idNguoiDung}")]
        [SwaggerOperation(Summary = "Lấy tất cả thông báo của người dùng")]
        [SwaggerResponse(StatusCodes.Status200OK, "Truy xuất thành công.", typeof(IEnumerable<ThongBao>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Người dùng chưa có thông báo nào.")]
        public async Task<ActionResult<IEnumerable<ThongBao>>> GetThongBaoByNguoiDung(int idNguoiDung)
        {
            var thongBaos = await _context.ThongBaos
                                          .Where(tb => tb.Id_NguoiDung == idNguoiDung && tb.TrangThai == true)
                                          .OrderByDescending(tb => tb.NgayTao)
                                          .ToListAsync();

            if (!thongBaos.Any())
                return NotFound("Người dùng này chưa có thông báo nào.");

            return Ok(thongBaos);
        }

        // POST: api/ThongBao
        [HttpPost]
        [SwaggerOperation(Summary = "Thêm thông báo bằng proc insert_ThongBao")]
        public async Task<IActionResult> PostThongBao([FromBody] ThongBaoDTO dto)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@id_nguoidung", dto.IdNguoiDung),
                    new SqlParameter("@noidung", dto.NoiDung)
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC insert_ThongBao @id_nguoidung, @noidung", parameters);

                return Ok(new { message = "Thêm thông báo thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/ThongBao
        [HttpDelete]
        [SwaggerOperation(Summary = "Xóa mềm thông báo bằng proc delete_ThongBao")]
        public async Task<IActionResult> DeleteThongBao([FromBody] DeleteThongBaoDTO dto)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@id_thongbao", dto.IdThongBao),
                    new SqlParameter("@id_nguoidung", dto.IdNguoiDung)
                };

                var result = await _context.ThongBaos
                                           .FromSqlRaw("EXEC delete_ThongBao @id_thongbao, @id_nguoidung", parameters)
                                           .ToListAsync();

                return Ok(new { message = "Xóa mềm thông báo thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
