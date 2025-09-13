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
                    Loai = datp.TacPham.Loai,
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
            {
                return NotFound("Không tìm thấy dự án với ID đã cho.");
            }

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
                    Loai = datp.TacPham.Loai,
                    NgayTao = datp.TacPham.NgayTao,
                    TenTheLoai = datp.TacPham.TheLoai.Ten, // ✅ sửa ở đây
                    TenNguoiTao = datp.TacPham.NguoiTao.Ten
                })
                .ToListAsync();

            if (!tacPhams.Any())
            {
                return NotFound(new { message = "Dự án này không có tác phẩm nào." });
            }

            return Ok(tacPhams);
        }

        /// <summary>
        /// Thêm một tác phẩm vào dự án
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Thêm tác phẩm vào dự án", Description = "Thêm một tác phẩm vào một dự án cụ thể.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thêm thành công.", typeof(DuAn_TacPham))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Mối liên kết đã tồn tại.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "ID dự án hoặc ID tác phẩm không hợp lệ.")]
        public async Task<ActionResult<DuAn_TacPham>> AddTacPhamToDuAn(DuAn_TacPham duAnTacPham)
        {
            var duAnExists = await _context.DuAnCongDongs.AnyAsync(da => da.Id == duAnTacPham.Id_DuAn);
            var tacPhamExists = await _context.TacPhams.AnyAsync(tp => tp.Id == duAnTacPham.Id_TacPham);

            if (!duAnExists || !tacPhamExists)
            {
                return BadRequest("ID dự án hoặc ID tác phẩm không tồn tại.");
            }

            var exists = await _context.DuAn_TacPhams
                .AnyAsync(datp => datp.Id_DuAn == duAnTacPham.Id_DuAn && datp.Id_TacPham == duAnTacPham.Id_TacPham);
            if (exists)
            {
                return Conflict("Mối liên kết giữa dự án và tác phẩm này đã tồn tại.");
            }

            duAnTacPham.NgayDang = DateTime.Now;
            _context.DuAn_TacPhams.Add(duAnTacPham);
            await _context.SaveChangesAsync();

            return Ok(duAnTacPham);
        }

        /// <summary>
        /// Xóa một tác phẩm khỏi dự án
        /// </summary>
        [HttpDelete("DuAn/{idDuAn}/TacPham/{idTacPham}")]
        [SwaggerOperation(Summary = "Xóa tác phẩm khỏi dự án", Description = "Xóa một tác phẩm khỏi một dự án bằng ID dự án và ID tác phẩm.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Xóa thành công.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy mối liên kết để xóa.")]
        public async Task<IActionResult> RemoveTacPhamFromDuAn(int idDuAn, int idTacPham)
        {
            var duAnTacPham = await _context.DuAn_TacPhams.FindAsync(idDuAn, idTacPham);
            if (duAnTacPham == null)
            {
                return NotFound("Không tìm thấy mối liên kết này.");
            }

            _context.DuAn_TacPhams.Remove(duAnTacPham);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
