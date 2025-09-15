using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.DTOs;
using CODE_CDIO4.Repository;
using Swashbuckle.AspNetCore.Annotations;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiamGiaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GiamGiaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả mã giảm giá")]
        public async Task<ActionResult<IEnumerable<GiamGiaDTO>>> GetAll()
        {
            var result = await _context.GiamGias
                .Select(g => new GiamGiaDTO
                {
                    Id = g.Id,
                    MaGiamGia = g.MaGiamGia,
                    LoaiGiam = g.LoaiGiam,
                    GiaTri = g.GiaTri,
                    NgayBatDau = g.NgayBatDau,
                    NgayKetThuc = g.NgayKetThuc,
                    SoLanSuDung = g.SoLanSuDung,
                    DaSuDung = g.DaSuDung
                }).ToListAsync();
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Thêm mã giảm giá mới")]
        public async Task<IActionResult> Create([FromBody] CreateGiamGiaDTO dto)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@MaGiamGia", dto.MaGiamGia),
                    new SqlParameter("@LoaiGiam", dto.LoaiGiam),
                    new SqlParameter("@GiaTri", dto.GiaTri),
                    new SqlParameter("@NgayBatDau", dto.NgayBatDau),
                    new SqlParameter("@NgayKetThuc", dto.NgayKetThuc),
                    new SqlParameter("@SoLanSuDung", dto.SoLanSuDung ?? (object)DBNull.Value)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC insert_GiamGia @MaGiamGia, @LoaiGiam, @GiaTri, @NgayBatDau, @NgayKetThuc, @SoLanSuDung", parameters);
                return Ok("Thêm mã giảm giá thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Sửa thông tin mã giảm giá")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateGiamGiaDTO dto)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@MaGiamGia", dto.MaGiamGia),
                    new SqlParameter("@LoaiGiam", dto.LoaiGiam),
                    new SqlParameter("@GiaTri", dto.GiaTri),
                    new SqlParameter("@NgayBatDau", dto.NgayBatDau),
                    new SqlParameter("@NgayKetThuc", dto.NgayKetThuc),
                    new SqlParameter("@SoLanSuDung", dto.SoLanSuDung ?? (object)DBNull.Value)
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC update_GiamGia @Id, @MaGiamGia, @LoaiGiam, @GiaTri, @NgayBatDau, @NgayKetThuc, @SoLanSuDung", parameters);
                return Ok("Cập nhật mã giảm giá thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa mã giảm giá")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var param = new SqlParameter("@Id", id);
                await _context.Database.ExecuteSqlRawAsync("EXEC delete_GiamGia @Id", param);
                return Ok("Xóa mã giảm giá thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
