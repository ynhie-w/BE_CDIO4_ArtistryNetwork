using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Services.Interfaces;
using System.Threading.Tasks;
using System.Security.Claims;

namespace CDIO4_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Chỉ dành cho thành viên đăng nhập
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;

        public GioHangController(IGioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }

        // ===== Xem giỏ hàng =====
        [HttpGet]
        public async Task<IActionResult> XemGioHang()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var gioHang = await _gioHangService.LayGioHang(userId);
            return Ok(gioHang);
        }

        // ===== Thêm sản phẩm =====
        [HttpPost("Them")]
        public async Task<IActionResult> ThemSanPham([FromBody] ThemGioHangDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var ketQua = await _gioHangService.ThemSanPham(userId, dto);
            if (!ketQua) return BadRequest(new { ThongBao = "Thêm sản phẩm thất bại" });

            return Ok(new { ThongBao = "Thêm sản phẩm thành công" });
        }

        // ===== Xóa sản phẩm =====
        [HttpDelete("Xoa")]
        public async Task<IActionResult> XoaSanPham([FromQuery] int sanPhamId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var ketQua = await _gioHangService.XoaSanPham(userId, sanPhamId);
            if (!ketQua) return BadRequest(new { ThongBao = "Xóa sản phẩm thất bại" });

            return Ok(new { ThongBao = "Xóa sản phẩm thành công" });
        }
    }
}
