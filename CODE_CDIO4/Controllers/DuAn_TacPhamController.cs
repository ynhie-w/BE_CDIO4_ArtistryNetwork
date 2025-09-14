using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;
using CODE_CDIO4.DTOs;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DuAn_TacPhamController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DuAn_TacPhamController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy tất cả tác phẩm trong các dự án
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy tất cả tác phẩm dự án", Description = "Lấy toàn bộ danh sách tác phẩm thuộc về các dự án.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công, trả về danh sách tác phẩm.", typeof(IEnumerable<TacPhamDTO>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không có tác phẩm nào trong các dự án.")]
        public async Task<ActionResult<IEnumerable<TacPhamDTO>>> GetAllTacPhamsInProjects()
        {
            var tacPhams = await _context.DuAn_TacPhams
                .Include(datp => datp.TacPham)
                    .ThenInclude(tp => tp.TheLoai)
                .Include(datp => datp.TacPham)
                    .ThenInclude(tp => tp.NguoiTao)
                .Select(datp => new TacPhamDTO
                {
                    Id = datp.TacPham.Id,
                    Ten = datp.TacPham.Ten,
                    MoTa = datp.TacPham.MoTa,
                    Gia = datp.TacPham.Gia,
                    TrangThai = datp.TacPham.TrangThai,
                    NgayTao = datp.TacPham.NgayTao,
                    TenTheLoai = datp.TacPham.TheLoai.Ten,
                    TenNguoiTao = datp.TacPham.NguoiTao.Ten
                })
                .ToListAsync();

            if (!tacPhams.Any())
            {
                return NotFound(new { message = "Không có tác phẩm nào trong các dự án." });
            }

            return Ok(tacPhams);
        }

        /// <summary>
        /// Lấy tất cả các tác phẩm của một dự án theo ID
        /// </summary>
        [HttpGet("DuAn/{idDuAn}")]
        [SwaggerOperation(Summary = "Lấy tác phẩm theo ID dự án", Description = "Lấy danh sách các tác phẩm thuộc về một dự án cụ thể.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công, trả về danh sách tác phẩm.", typeof(IEnumerable<TacPhamDTO>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Dự án không tồn tại hoặc không có tác phẩm nào.")]
        public async Task<ActionResult<IEnumerable<TacPhamDTO>>> GetTacPhamsByDuAn(int idDuAn)
        {
            var duAn = await _context.DuAnCongDongs.FindAsync(idDuAn);
            if (duAn == null)
                return NotFound("Không tìm thấy dự án với ID đã cho.");

            var tacPhams = await _context.DuAn_TacPhams
                .Where(datp => datp.Id_DuAn == idDuAn)
                .Include(datp => datp.TacPham)
                    .ThenInclude(tp => tp.TheLoai)
                .Include(datp => datp.TacPham)
                    .ThenInclude(tp => tp.NguoiTao)
                .Select(datp => new TacPhamDTO
                {
                    Id = datp.TacPham.Id,
                    Ten = datp.TacPham.Ten,
                    MoTa = datp.TacPham.MoTa,
                    Gia = datp.TacPham.Gia,
                    TrangThai = datp.TacPham.TrangThai, 
                    NgayTao = datp.TacPham.NgayTao,
                    TenTheLoai = datp.TacPham.TheLoai.Ten,
                    TenNguoiTao = datp.TacPham.NguoiTao.Ten
                })
                .ToListAsync();

            if (!tacPhams.Any())
                return NotFound(new { message = "Dự án này không có tác phẩm nào." });

            return Ok(tacPhams);
        }

        /// <summary>
        /// Thêm một tác phẩm vào dự án bằng SP
        /// </summary>
        [HttpPost("DuAn/SP")]
        public async Task<ActionResult> AddTacPhamToDuAnUsingSP(DuAn_TacPham duAnTacPham)
        {
            var duAnExists = await _context.DuAnCongDongs.AnyAsync(da => da.Id == duAnTacPham.Id_DuAn);
            var tacPhamExists = await _context.TacPhams.AnyAsync(tp => tp.Id == duAnTacPham.Id_TacPham);

            if (!duAnExists || !tacPhamExists)
                return BadRequest("ID dự án hoặc ID tác phẩm không tồn tại.");

            // đảm bảo TrangThai hợp lệ
            var trangThaiHopLe = new List<string> { "Đang bán", "Đã bán", "Ẩn" };
            if (!trangThaiHopLe.Contains(duAnTacPham.TrangThai))
                duAnTacPham.TrangThai = "Đang bán";

            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC insert_DuAn_TacPham 
                    @id_tacpham = {duAnTacPham.Id_TacPham}, 
                    @id_duan = {duAnTacPham.Id_DuAn}, 
                    @trangthai = {duAnTacPham.TrangThai}");
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

            return Ok(duAnTacPham);
        }

        [HttpPut("DuAn/SP/{id}")]
        public async Task<ActionResult> UpdateTrangThaiTacPhamSP(int id, [FromBody] string trangthai, int idNguoiDung)
        {
            // kiểm tra giá trị hợp lệ
            var trangThaiHopLe = new List<string> { "Đang bán", "Đã bán", "Ẩn" };
            if (!trangThaiHopLe.Contains(trangthai))
                return BadRequest("Trạng thái không hợp lệ.");

            try
            {
                var result = await _context.Set<MessageResult>()
                    .FromSqlInterpolated($@"
                    EXEC update_DuAn_TacPham 
                        @id = {id}, 
                        @id_nguoidung = {idNguoiDung}, 
                        @trangthai = {trangthai}")
                    .FirstOrDefaultAsync();

                return Ok(result?.Message ?? "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DuAn/TacPham/{id}")]
        public async Task<ActionResult> DeleteSoftTacPhamSP(int id, int idNguoiDung)
        {
            try
            {
                var result = await _context.Set<MessageResult>()
                    .FromSqlInterpolated($@"
                    EXEC deleteSoft_DuAn_TacPham 
                        @id = {id}, 
                        @id_nguoidung = {idNguoiDung}")
                    .FirstOrDefaultAsync();

                return Ok(result?.Message ?? "Xóa mềm thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DTO tạm để nhận message từ SP
        public class MessageResult
        {
            public string Message { get; set; } = string.Empty;
        }
    }
}
