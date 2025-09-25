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
    [Authorize] // Chỉ dành cho thành viên
    public class DonHangController : ControllerBase
    {
        private readonly IDonHangService _donHangService;

        public DonHangController(IDonHangService donHangService)
        {
            _donHangService = donHangService;
        }

        [HttpGet]
        public async Task<IActionResult> XemDonHang()
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Message = "Không tìm thấy userId trong token" });

            var userId = int.Parse(userIdClaim);
            var donHang = await _donHangService.LayDanhSachDonHang(userId);
            return Ok(donHang);
        }

        [HttpPost("TaoMoi")]
        public async Task<IActionResult> TaoDonHang([FromBody] TaoDonHangDto dto)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Message = "Không tìm thấy userId trong token" });

            var userId = int.Parse(userIdClaim);
            var newOrderId = await _donHangService.TaoDonHang(userId, dto);
            if (newOrderId == 0) return BadRequest(new { ThongBao = "Tạo đơn hàng thất bại" });
            return Ok(new { DonHangId = newOrderId, ThongBao = "Tạo đơn hàng thành công" });
        }
    }
}
