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
    public class TacPham_CamXucController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TacPham_CamXucController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== LẤY CẢM XÚC ====================
        [HttpGet("nguoidung/{idNguoiDung}/tacpham/{idTacPham}")]
        [SwaggerOperation(Summary = "Lấy cảm xúc của một người dùng đối với một tác phẩm")]
        public async Task<ActionResult<TacPhamCamXucDTO>> GetCamXucCuaTacPham(int idNguoiDung, int idTacPham)
        {
            var camXuc = await _context.TacPham_CamXucs
                                       .AsNoTracking()
                                       .SingleOrDefaultAsync(cx => cx.Id_NguoiDung == idNguoiDung
                                                                && cx.Id_TacPham == idTacPham
                                                                && cx.TrangThai);

            if (camXuc == null)
                return NotFound("Người dùng chưa có cảm xúc cho tác phẩm này.");

            var dto = new TacPhamCamXucDTO
            {
                Id_NguoiDung = camXuc.Id_NguoiDung,
                Id_TacPham = camXuc.Id_TacPham,
                Id_CamXuc = camXuc.Id_CamXuc,
                NgayTao = camXuc.NgayTao
            };

            return Ok(dto);
        }

        // ==================== TẠO HOẶC CẬP NHẬT (UPSERT) ====================
        [HttpPost]
        [SwaggerOperation(Summary = "Thêm hoặc cập nhật cảm xúc cho tác phẩm")]
        public async Task<IActionResult> UpsertCamXuc([FromBody] TacPhamCamXucRequest request)
        {
            var existingCamXuc = await _context.TacPham_CamXucs
                                               .SingleOrDefaultAsync(cx => cx.Id_NguoiDung == request.Id_NguoiDung
                                                                        && cx.Id_TacPham == request.Id_TacPham);

            if (existingCamXuc == null)
            {
                var newCamXuc = new TacPham_CamXuc
                {
                    Id_NguoiDung = request.Id_NguoiDung,
                    Id_TacPham = request.Id_TacPham,
                    Id_CamXuc = request.Id_CamXuc,
                    NgayTao = DateTime.Now,
                    TrangThai = true
                };

                _context.TacPham_CamXucs.Add(newCamXuc);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCamXucCuaTacPham),
                    new { idNguoiDung = newCamXuc.Id_NguoiDung, idTacPham = newCamXuc.Id_TacPham },
                    new TacPhamCamXucDTO
                    {
                        Id_NguoiDung = newCamXuc.Id_NguoiDung,
                        Id_TacPham = newCamXuc.Id_TacPham,
                        Id_CamXuc = newCamXuc.Id_CamXuc,
                        NgayTao = newCamXuc.NgayTao
                    });
            }
            else
            {
                existingCamXuc.Id_CamXuc = request.Id_CamXuc;
                existingCamXuc.NgayTao = DateTime.Now;
                existingCamXuc.TrangThai = true;

                _context.Entry(existingCamXuc).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new TacPhamCamXucDTO
                {
                    Id_NguoiDung = existingCamXuc.Id_NguoiDung,
                    Id_TacPham = existingCamXuc.Id_TacPham,
                    Id_CamXuc = existingCamXuc.Id_CamXuc,
                    NgayTao = existingCamXuc.NgayTao
                });
            }
        }

        // ==================== XÓA CẢM XÚC (SOFT DELETE) ====================
        [HttpDelete("nguoidung/{idNguoiDung}/tacpham/{idTacPham}")]
        [SwaggerOperation(Summary = "Xóa cảm xúc (soft delete)")]
        public async Task<IActionResult> DeleteCamXuc(int idNguoiDung, int idTacPham)
        {
            var camXuc = await _context.TacPham_CamXucs
                                       .SingleOrDefaultAsync(cx => cx.Id_NguoiDung == idNguoiDung
                                                                && cx.Id_TacPham == idTacPham
                                                                && cx.TrangThai);

            if (camXuc == null)
                return NotFound("Không tìm thấy cảm xúc để xóa.");

            camXuc.TrangThai = false;
            _context.Entry(camXuc).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
