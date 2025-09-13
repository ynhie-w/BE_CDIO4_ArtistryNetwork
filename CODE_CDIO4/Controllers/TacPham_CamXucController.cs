using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TacPham_CamXucController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TacPham_CamXucController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== LẤY CẢM XÚC ====================

        // GET: api/TacPham_CamXuc/nguoidung/1/tacpham/10
        [HttpGet("nguoidung/{idNguoiDung}/tacpham/{idTacPham}")]
        [SwaggerOperation(
            Summary = "Lấy cảm xúc của một người dùng đối với một tác phẩm",
            Description = "Trả về cảm xúc của người dùng (nếu có) đối với một tác phẩm cụ thể."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công, trả về cảm xúc của người dùng.", typeof(TacPham_CamXuc))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Người dùng chưa có cảm xúc cho tác phẩm này.")]
        public async Task<ActionResult<TacPham_CamXuc>> GetCamXucCuaTacPham(int idNguoiDung, int idTacPham)
        {
            var camXuc = await _context.TacPham_CamXucs
                                       .AsNoTracking()
                                       .SingleOrDefaultAsync(cx => cx.Id_NguoiDung == idNguoiDung && cx.Id_TacPham == idTacPham);

            if (camXuc == null)
            {
                return NotFound("Người dùng chưa có cảm xúc cho tác phẩm này.");
            }

            return Ok(camXuc);
        }

        // ==================== TẠO VÀ CẬP NHẬT CẢM XÚC (UPSERT) ====================

        // POST: api/TacPham_CamXuc
        [HttpPost]
        [SwaggerOperation(
            Summary = "Tạo hoặc cập nhật cảm xúc cho tác phẩm",
            Description = "Thêm một cảm xúc mới vào tác phẩm hoặc thay đổi cảm xúc đã có."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Cập nhật thành công.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Tạo mới thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ.")]
        public async Task<IActionResult> UpsertCamXuc([FromBody] TacPham_CamXuc tacPhamCamXuc)
        {
            // Kiểm tra tính hợp lệ của Id_NguoiDung, Id_TacPham và Id_CamXuc
            var nguoiDung = await _context.NguoiDungs.FindAsync(tacPhamCamXuc.Id_NguoiDung);
            var tacPham = await _context.TacPhams.FindAsync(tacPhamCamXuc.Id_TacPham);
            var camXuc = await _context.CamXucs.FindAsync(tacPhamCamXuc.Id_CamXuc);

            if (nguoiDung == null || tacPham == null || camXuc == null)
            {
                return BadRequest("Id_NguoiDung, Id_TacPham, hoặc Id_CamXuc không tồn tại.");
            }

            // Tìm xem cảm xúc đã tồn tại chưa
            var existingCamXuc = await _context.TacPham_CamXucs
                                               .SingleOrDefaultAsync(cx => cx.Id_NguoiDung == tacPhamCamXuc.Id_NguoiDung && cx.Id_TacPham == tacPhamCamXuc.Id_TacPham);

            if (existingCamXuc == null)
            {
                // Nếu chưa tồn tại, thêm mới
                tacPhamCamXuc.NgayTao = DateTime.Now;
                _context.TacPham_CamXucs.Add(tacPhamCamXuc);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCamXucCuaTacPham), new { idNguoiDung = tacPhamCamXuc.Id_NguoiDung, idTacPham = tacPhamCamXuc.Id_TacPham }, tacPhamCamXuc);
            }
            else
            {
                // Nếu đã tồn tại, cập nhật
                existingCamXuc.Id_CamXuc = tacPhamCamXuc.Id_CamXuc;
                existingCamXuc.NgayTao = DateTime.Now;
                _context.Entry(existingCamXuc).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(existingCamXuc);
            }
        }

        // ==================== XÓA CẢM XÚC ====================

        // DELETE: api/TacPham_CamXuc/nguoidung/1/tacpham/10
        [HttpDelete("nguoidung/{idNguoiDung}/tacpham/{idTacPham}")]
        [SwaggerOperation(
            Summary = "Xóa cảm xúc của người dùng đối với một tác phẩm",
            Description = "Xóa một cảm xúc cụ thể của người dùng đối với một tác phẩm."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Xóa thành công.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy cảm xúc để xóa.")]
        public async Task<IActionResult> DeleteCamXuc(int idNguoiDung, int idTacPham)
        {
            var camXuc = await _context.TacPham_CamXucs
                                       .SingleOrDefaultAsync(cx => cx.Id_NguoiDung == idNguoiDung && cx.Id_TacPham == idTacPham);

            if (camXuc == null)
            {
                return NotFound("Không tìm thấy cảm xúc để xóa.");
            }

            _context.TacPham_CamXucs.Remove(camXuc);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}