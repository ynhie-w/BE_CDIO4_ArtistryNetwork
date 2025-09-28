using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Services.Interfaces;
using CDIO4_BE.Helper;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using CDIO4_BE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CDIO4_BE.Repository;
using System.Text.RegularExpressions;
namespace CDIO4_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaiKhoanController : ControllerBase
    {

        public TaiKhoanController(ITaiKhoanService taiKhoanService, AppDbContext context)
        {
            _taiKhoanService = taiKhoanService;
            _context = context;
        }

        private readonly ITaiKhoanService _taiKhoanService;
        private readonly AppDbContext _context;


        // ===== ĐĂNG NHẬP =====
        [HttpPost("dangnhap")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Đăng nhập bằng email hoặc số điện thoại và mật khẩu, trả về JWT Token")]
        public async Task<IActionResult> DangNhap([FromBody] DangNhapDto yeuCau)
        {
            var token = await _taiKhoanService.DangNhap(yeuCau);
            if (token == null)
                return Unauthorized(new { ThongBao = "Sai email/sdt hoặc mật khẩu" });

            return Ok(new { Token = token });
        }

        // ===== ĐĂNG KÝ =====
        [HttpPost("dangky")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Đăng ký tài khoản mới")]
        public async Task<IActionResult> DangKy([FromBody] DangKyDto yeuCau)
        {
            try
            {
                var newUserId = await _taiKhoanService.DangKy(yeuCau);
                return Ok(new { NewUserId = newUserId, ThongBao = "Đăng ký thành công" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ThongBao = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ThongBao = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ThongBao = "Đã xảy ra lỗi hệ thống", ChiTiet = ex.Message });
            }
        }



        // ===== ĐĂNG XUẤT =====
        [HttpPost("dangxuat")]
        [Authorize]
        [SwaggerOperation(Summary = "Đăng xuất, hủy JWT token hiện tại")]
        public async Task<IActionResult> DangXuat()
        {
            var ketQua = await _taiKhoanService.DangXuat();
            return Ok(new { ThongBao = ketQua ? "Đăng xuất thành công" : "Lỗi khi đăng xuất" });
        }

        // ===== ĐỔI MẬT KHẨU =====
        [HttpPut("doimatkhau")]
        [Authorize]
        [SwaggerOperation(Summary = "Đổi mật khẩu, cần mật khẩu cũ và mật khẩu mới (cần đăng nhập)")]
        public async Task<IActionResult> DoiMatKhau([FromBody] DoiMatKhauDto yeuCau)
        {
            var ketQua = await _taiKhoanService.DoiMatKhau(User, yeuCau);

            if (!ketQua)
                return BadRequest(new { ThongBao = "Mật khẩu cũ không đúng hoặc không tìm thấy người dùng" });

            return Ok(new { ThongBao = "Đổi mật khẩu thành công" });
        }
        /*test nội bộ (không email/SMS)
        Gọi API POST /api/TaiKhoan/quenmatkhau với JSON:
        {
          "emailSdt": "0763576386"
        }
        Trong console của backend, bạn sẽ thấy in ra:
        Link reset mật khẩu: https://yourdomain.com/resetpassword?token=xxxxxxxxxxxxxxxxxxxxxxxx
         Copy token từ link đó.
        Gọi API POST /api/TaiKhoan/datlaimatkhau với JSON:
        {
          "token": "xxxxxxxxxxxxxxxxxxxxxxxx",
          "matKhauMoi": "MatKhauMoi123"
        }
        Nếu thành công, mật khẩu của người dùng đã được đổi.*/
        // ===== QUÊN MẬT KHẨU =====
        [HttpPost("quenmatkhau")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Gửi yêu cầu quên mật khẩu, tạo token và gửi link reset. lấy token reset ở console của backend")]
        public async Task<IActionResult> QuenMatKhau([FromBody] QuenMatKhauDto yeuCau)
        {
            var ketQua = await _taiKhoanService.QuenMatKhau(yeuCau);
            return Ok(new { ThongBao = ketQua ? "Vui lòng kiểm tra email/SĐT để reset mật khẩu" : "Không tìm thấy người dùng" });
        }

        [HttpPost("datlaimatkhau")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Đặt lại mật khẩu từ token reset")]
        public async Task<IActionResult> DatLaiMatKhau([FromBody] DatLaiMatKhauDto yeuCau)
        {
            var ketQua = await _taiKhoanService.DatLaiMatKhau(yeuCau);
            if (!ketQua) return BadRequest(new { ThongBao = "Token không hợp lệ hoặc đã hết hạn" });

            return Ok(new { ThongBao = "Đặt lại mật khẩu thành công" });
        }

        [Authorize]
        [HttpGet("ThongTin")]
        [SwaggerOperation(Summary = "lấy thông tin cá nhân (đã đăng nhập)")]
        public async Task<IActionResult> ThongTin()
        {
            var info = await _taiKhoanService.LayThongTin(User);
            if (info == null) return Unauthorized();
            return Ok(info);
        }

        [HttpPatch("CapNhatEmail")]
        [SwaggerOperation(Summary = "Cap Nhat Email (đã đăng nhập)")]
        public async Task<IActionResult> CapNhatEmail([FromBody] CapNhatNguoiDungDto dto)
        {
            if (await _taiKhoanService.CapNhatEmail(User, dto.Email))
                return Ok(new { message = "Cập nhật email thành công" });
            return BadRequest(new { message = "Email không hợp lệ hoặc đã tồn tại" });
        }

        [HttpPatch("CapNhatSdt")]
        [SwaggerOperation(Summary = "Cap Nhat sdt (đã đăng nhập)")]
        public async Task<IActionResult> CapNhatSdt([FromBody] CapNhatNguoiDungDto dto)
        {
            if (await _taiKhoanService.CapNhatSdt(User, dto.Sdt))
                return Ok(new { message = "Cập nhật số điện thoại thành công" });
            return BadRequest(new { message = "SĐT không hợp lệ hoặc đã tồn tại" });
        }

        [HttpPatch("CapNhatAnhDaiDien")]
        [SwaggerOperation(Summary = "Cap Nhat anh dai dien (đã đăng nhập)")]
        public async Task<IActionResult> CapNhatAnhDaiDien([FromBody] CapNhatNguoiDungDto dto)
        {
            if (await _taiKhoanService.CapNhatAnhDaiDien(User, dto.AnhDaiDien))
                return Ok(new { message = "Cập nhật ảnh đại diện thành công" });
            return BadRequest(new { message = "Link ảnh không hợp lệ" });
        }

    }
}

