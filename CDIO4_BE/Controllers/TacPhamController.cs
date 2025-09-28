using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace CDIO4_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TacPhamController : ControllerBase
    {
        private readonly ITacPhamService _tacPhamService;
        private readonly AppDbContext _context;

        public TacPhamController(ITacPhamService tacPhamService, AppDbContext context)
        {
            _tacPhamService = tacPhamService;
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Không tìm thấy userId trong token");
            }
            return userId;
        }
        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Tạo tác phẩm mới (cần đăng nhập)")]
        public async Task<IActionResult> TaoTacPham([FromBody] TaoTacPhamDto dto)
        {
            var userId = GetUserId();
            var id = await _tacPhamService.TaoTacPham(dto, userId);
            return Ok(new { message = "Tác phẩm đã được tạo thành công", Id = id });
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Sửa tác phẩm (cần đăng nhập, chỉ chủ sở hữu mới được sửa)")]
        public async Task<IActionResult> SuaTacPham(int id, [FromBody] SuaTacPhamDto dto)
        {
            var userId = GetUserId();
            var result = await _tacPhamService.SuaTacPham(id, dto, userId);
            if (!result)
                return Forbid("Bạn không có quyền sửa tác phẩm này hoặc tác phẩm không tồn tại.");

            return Ok("Tác phẩm đã được cập nhật thành công.");
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Xóa tác phẩm (cần đăng nhập, chỉ chủ sở hữu mới được xóa)")]
        public async Task<IActionResult> XoaTacPham(int id)
        {
            var userId = GetUserId();
            var result = await _tacPhamService.XoaTacPham(id, userId);
            if (!result)
                return Forbid("Bạn không có quyền xóa tác phẩm này hoặc tác phẩm không tồn tại.");

            return Ok("Tác phẩm đã được xóa thành công.");
        }

        // GET: /api/tacPham/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Xem chi tiết tác phẩm, sẽ tăng lượt xem (không cần đăng nhập) ")]
        public async Task<IActionResult> LayChiTietTacPham(int id)
        {
            var tp = await _tacPhamService.LayChiTietTacPham(id);
            if (tp == null)
                return NotFound("Không tìm thấy tác phẩm");

            bool chuSoHuu = false;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    if (tp.NguoiTao.Id == userId)
                    {
                        chuSoHuu = true;
                    }
                }
            }

            return Ok(new
            {
                TacPham = tp,
                ChuSoHuu = chuSoHuu
            });
        }

        [HttpGet("Search")]
        [SwaggerOperation(Summary = "Tìm kiếm tác phẩm theo từ khóa (không cần đăng nhập)")]
        public async Task<IActionResult> TimKiemTacPham(string keyword)
        {
            var list = await _tacPhamService.TimKiemTacPham(keyword);
            return Ok(list);
        }

        [HttpGet("BoSuuTap")]
        [Authorize]
        [SwaggerOperation(Summary = "Lấy danh sách bộ sưu tập của người dùng đã đăng nhập (cần đăng nhập)")]
        public async Task<IActionResult> LayBoSuuTap()
        {
            var userId = GetUserId();
            var list = await _tacPhamService.LayBoSuuTap(userId);
            return Ok(list);
        }

        [HttpGet("My")]
        [Authorize]
        [SwaggerOperation(Summary = "Lấy danh sách tác phẩm của chính mình  (cần đăng nhập)")]
        public async Task<IActionResult> LayTacPhamCuaToi()
        {
            var userId = GetUserId();
            var list = await _tacPhamService.LayTacPhamCuaToi(userId);
            return Ok(list);
        }

        [HttpPost("ThemVaoBoSuuTap/{idTacPham}")]
        [Authorize]
        [SwaggerOperation(Summary = "Thêm tác phẩm vào bộ sưu tập của người dùng  (cần đăng nhập)")]
        public async Task<IActionResult> ThemVaoBoSuuTap(int idTacPham)
        {
            var userId = GetUserId();
            await _tacPhamService.ThemVaoBoSuuTap(userId, idTacPham);
            return Ok("Đã thêm vào bộ sưu tập");
        }

        [HttpDelete("BoSuuTap/{idTacPham}")]
        [Authorize]
        [SwaggerOperation(Summary = "Xóa tác phẩm khỏi bộ sưu tập của người dùng  (cần đăng nhập)")]
        public async Task<IActionResult> XoaKhoiBoSuuTap(int idTacPham)
        {
            var userId = GetUserId();
            try
            {
                await _tacPhamService.XoaKhoiBoSuuTap(userId, idTacPham);
                return Ok("Đã xóa tác phẩm khỏi bộ sưu tập.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Mua/{idTacPham}")]
        [Authorize]
        [SwaggerOperation(Summary = "Mua tác phẩm (demo chỉ ghi nhận đánh giá 5 sao)  (cần đăng nhập)")]
        public async Task<IActionResult> MuaTacPham(int idTacPham)
        {
            var userId = GetUserId();
            await _tacPhamService.MuaTacPham(userId, idTacPham);
            return Ok("Đã mua tác phẩm");
        }

        [HttpGet("tacpham/{idTacPham}")]
        [SwaggerOperation(Summary = "Xem danh sách bình luận của tác phẩm (bao gồm trả lời)")]
        public async Task<IActionResult> XemDanhSachBinhLuanCuaTacPham(int idTacPham)
        {
            int? currentUserId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    currentUserId = userId;
                }
            }

            var binhLuans = await _tacPhamService.XemDanhSachBinhLuanCuaTacPham(idTacPham, currentUserId);
            var tacPham = await _tacPhamService.LayChiTietTacPham(idTacPham);
            bool chuSoHuuTacPham = tacPham?.NguoiTao?.Id == currentUserId;

            return Ok(new
            {
                BinhLuan = binhLuans,
                ChuSoHuuTacPham = chuSoHuuTacPham
            });
        }

        [HttpPost("{idTacPham}/binhluan")]
        [SwaggerOperation(Summary = "Thêm bình luận hoặc trả lời bình luận  (cần đăng nhập)")]
        [Authorize]
        public async Task<IActionResult> ThemBinhLuan(int idTacPham, [FromBody] ThemBinhLuanRequest request)
        {
            var userId = GetUserId();
            if (request.IdBinhLuanCha.HasValue)
                await _tacPhamService.ThemTraLoiBinhLuan(userId, request.IdBinhLuanCha, request.NoiDung);
            else
                await _tacPhamService.ThemBinhLuan(userId, idTacPham, request.NoiDung);

            return Ok("Bình luận đã được thêm");
        }

        [HttpPut("binhluan/sua")]
        [Authorize]
        [SwaggerOperation(Summary = "Sửa nội dung bình luận  (cần đăng nhập)")]
        public async Task<IActionResult> SuaBinhLuan([FromBody] SuaBinhLuanDto request)
        {
            var userId = GetUserId();
            try
            {
                await _tacPhamService.SuaBinhLuan(userId, request.IdBinhLuan, request.NoiDungMoi);
                return Ok("Bình luận đã được cập nhật thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("binhluan/{idBinhLuan}")]
        [Authorize]
        [SwaggerOperation(Summary = "Xóa bình luận  (cần đăng nhập)")]
        public async Task<IActionResult> XoaBinhLuan(int idBinhLuan)
        {
            var userId = GetUserId();
            try
            {
                await _tacPhamService.XoaBinhLuan(userId, idBinhLuan);
                return Ok("Bình luận đã được xóa thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{idTacPham}/camxuc")]
        [SwaggerOperation(Summary = "Thêm/cập nhật cảm xúc  (cần đăng nhập)")]
        [Authorize]
        public async Task<IActionResult> UpsertCamXuc(int idTacPham, [FromBody] int idCamXuc)
        {
            var userId = GetUserId();
            try
            {
                await _tacPhamService.UpsertCamXuc(userId, idTacPham, idCamXuc);
                return Ok(new { message = "Cảm xúc đã được ghi nhận/cập nhật" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("camxuc/{idTacPham}")]
        [SwaggerOperation(Summary = "Xóa cảm xúc (cần đăng nhập)")]
        [Authorize]
        public async Task<IActionResult> XoaCamXuc(int idTacPham)
        {
            var userId = GetUserId();
            try
            {
                await _tacPhamService.XoaCamXuc(userId, idTacPham);
                return Ok("Cảm xúc đã được xóa thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
