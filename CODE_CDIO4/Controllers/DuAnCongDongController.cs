using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;
using CODE_CDIO4.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DuAnCongDongController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DuAnCongDongController(AppDbContext context)
        {
            _context = context;
        }

        // =================== DỰ ÁN ===================

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách dự án cộng đồng", Description = "Trả về tất cả dự án kèm thông tin quản lý.")]
        public async Task<ActionResult<IEnumerable<DuAnCongDongResponseDTO>>> GetDuAnCongDongs()
        {
            var duAn = await _context.DuAnCongDongs
                                     .Include(da => da.QuanLy)
                                     .Select(da => new DuAnCongDongResponseDTO
                                     {
                                         Id = da.Id,
                                         TenDuAn = da.TenDuAn,
                                         MoTa = da.MoTa,
                                         NgayTao = da.NgayTao,
                                         NgayBatDau = da.NgayBatDau,
                                         NgayKetThuc = da.NgayKetThuc,
                                         TrangThai = da.TrangThai,
                                         TenQuanLy = da.QuanLy.Ten
                                     })
                                     .ToListAsync();

            return Ok(duAn);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Chi tiết dự án cộng đồng", Description = "Trả về chi tiết một dự án theo ID.")]
        public async Task<ActionResult<DuAnCongDongResponseDTO>> GetDuAnCongDong(int id)
        {
            var duAn = await _context.DuAnCongDongs
                                     .Include(da => da.QuanLy)
                                     .FirstOrDefaultAsync(da => da.Id == id);

            if (duAn == null)
                return NotFound("Không tìm thấy dự án này.");

            var dto = new DuAnCongDongResponseDTO
            {
                Id = duAn.Id,
                TenDuAn = duAn.TenDuAn,
                MoTa = duAn.MoTa,
                NgayTao = duAn.NgayTao,
                NgayBatDau = duAn.NgayBatDau,
                NgayKetThuc = duAn.NgayKetThuc,
                TrangThai = duAn.TrangThai,
                TenQuanLy = duAn.QuanLy?.Ten
            };

            return Ok(dto);
        }

        [HttpPost("SP")]
        [SwaggerOperation(Summary = "Thêm dự án (SP)", Description = "Dùng stored procedure để thêm mới dự án.")]
        public async Task<ActionResult> PostDuAnCongDongSP(DuAnCongDongRequestDTO dto)
        {
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC insert_DuAnCongDong 
                        @tenduan = {dto.TenDuAn}, 
                        @mota = {dto.MoTa}, 
                        @ngaybatdau = {dto.NgayBatDau}, 
                        @ngayketthuc = {dto.NgayKetThuc}, 
                        @id_quanly = {dto.Id_QuanLy}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Thêm dự án thành công");
        }

        [HttpPut("SP/{id}")]
        [SwaggerOperation(Summary = "Cập nhật dự án (SP)", Description = "Chỉ quản lý mới có quyền cập nhật.")]
        public async Task<ActionResult> PutDuAnCongDongSP(int id, [FromQuery] int idNguoiDung, [FromBody] DuAnCongDongRequestDTO dto)
        {
            try
            {
                var result = await _context.Set<MessageResult>()
                    .FromSqlInterpolated($@"
                        EXEC update_DuAnCongDong 
                            @id = {id}, 
                            @id_nguoidung = {idNguoiDung}, 
                            @tenduan = {dto.TenDuAn}, 
                            @mota = {dto.MoTa}, 
                            @ngaybatdau = {dto.NgayBatDau}, 
                            @ngayketthuc = {dto.NgayKetThuc}, 
                            @trangthai = {dto.TrangThai}")
                    .FirstOrDefaultAsync();

                return Ok(result?.Message ?? "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("SP/{id}")]
        [SwaggerOperation(Summary = "Xóa mềm dự án (SP)", Description = "Đánh dấu trạng thái dự án là 'Đóng'.")]
        public async Task<ActionResult> DeleteDuAnCongDongSP(int id)
        {
            try
            {
                var result = await _context.Set<MessageResult>()
                    .FromSqlInterpolated($@"EXEC delete_DuAnCongDong @id = {id}")
                    .FirstOrDefaultAsync();

                return Ok(result?.Message ?? "Xóa mềm thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
