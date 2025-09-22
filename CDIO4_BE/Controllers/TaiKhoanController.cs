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
/*Controller cho tài khoản / xác thực (TaiKhoanController)
Chức năng liên quan đến đăng nhập, đăng ký, đổi/quên mật khẩu, quản lý thông tin cá nhân.*/
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
            if (string.IsNullOrEmpty(yeuCau.EmailSdt))
                return BadRequest(new { ThongBao = "Email hoặc số điện thoại không được trống" });
            if (string.IsNullOrEmpty(yeuCau.MatKhau))
                return BadRequest(new { ThongBao = "Mật khẩu không được trống" });

            var newUserId = await _taiKhoanService.DangKy(yeuCau);

            return Ok(new { NewUserId = newUserId, ThongBao = "Đăng ký thành công" });
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
        [SwaggerOperation(Summary = "Đổi mật khẩu, cần mật khẩu cũ và mật khẩu mới  (cần đăng nhập)")]
        public async Task<IActionResult> DoiMatKhau([FromBody] DoiMatKhauDto yeuCau)
        {
            var ketQua = await _taiKhoanService.DoiMatKhau(User, yeuCau);

            if (!ketQua)
                return BadRequest(new { ThongBao = "Mật khẩu cũ không đúng hoặc không tìm thấy người dùng" });

            return Ok(new { ThongBao = "Đổi mật khẩu thành công" });
        }
        /*est nội bộ (không email/SMS)
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

        // ====== LẤY THÔNG TIN CÁ NHÂN (CHỈ NGƯỜI ĐÃ ĐĂNG NHẬP =======//
        [Authorize]
        [HttpGet("ThongTin")]
        [SwaggerOperation(Summary = "Lấy thông tin cá nhân (chỉ thành viên) (cần đăng nhập)")]
        public async Task<IActionResult> ThongTin()
        {
            // Lấy userId từ JWT
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized(new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                    title = "Unauthorized",
                    status = 401,
                    detail = "Token không hợp lệ hoặc hết hạn",
                    traceId = HttpContext.TraceIdentifier
                });
            }

            var nguoiDung = await _context.NguoiDungs
                .Where(u => u.Id == userId)
                .Select(u => new CapNhatNguoiDungDto
                {
                    Id = u.Id,
                    Ten = u.Ten,
                    Email = u.Email,
                    Sdt = u.Sdt,
                    AnhDaiDien = u.AnhDaiDien,
                    MatKhauMoi = "",        // Không trả mật khẩu hiện tại
                    DiemThuong = u.DiemThuong,
                    TrangThai = u.TrangThai
                })
                .FirstOrDefaultAsync();

            if (nguoiDung == null)
            {
                return NotFound(new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    title = "Not Found",
                    status = 404,
                    detail = "Người dùng không tồn tại",
                    traceId = HttpContext.TraceIdentifier
                });
            }

            return Ok(nguoiDung);
        }
        //===== CẬP NHẬT THÔNG TIN NGƯỜI DÙNG (CHỈ NGƯỜI ĐÃ ĐĂNG NHẬP) ======//
        //===== CẬP NHẬT EMAIL (CHỈ NGƯỜI ĐÃ ĐĂNG NHẬP) ======//
        [HttpPatch("CapNhatEmail")]
            [Authorize]
        [SwaggerOperation(Summary = "Chỉnh sửa email  (cần đăng nhập)")]
        
            public async Task<IActionResult> CapNhatEmail([FromBody] CapNhatNguoiDungDto dto)
            {
                if (string.IsNullOrEmpty(dto?.Email))
                    return BadRequest(new { message = "Vui lòng cung cấp email mới" });

                // Validate định dạng email
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(dto.Email, emailPattern))
                    return BadRequest(new { message = "Email không hợp lệ" });

                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                    return Unauthorized(new { message = "Token không hợp lệ hoặc hết hạn" });

                var nguoiDung = await _context.NguoiDungs.FindAsync(userId);
                if (nguoiDung == null)
                    return NotFound(new { message = "Người dùng không tồn tại" });

                nguoiDung.Email = dto.Email;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật email thành công" });
            }

        //===== CẬP NHẬT SDT (CHỈ NGƯỜI ĐÃ ĐĂNG NHẬP) ======//
        [HttpPatch("CapNhatSdt")]
        [Authorize]
        [SwaggerOperation(Summary = "Chỉnh sửa số điện thoại (cần đăng nhập)")]
        public async Task<IActionResult> CapNhatSdt([FromBody] CapNhatNguoiDungDto dto)
        {
            if (string.IsNullOrEmpty(dto?.Sdt))
                return BadRequest(new { message = "Vui lòng cung cấp số điện thoại mới" });

            // Validate sdt: chỉ 10 số, không chữ, không ký tự khác
            var sdtPattern = @"^\d{10}$";
            if (!Regex.IsMatch(dto.Sdt, sdtPattern))
                return BadRequest(new { message = "Số điện thoại phải đủ 10 số" });

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc hết hạn" });

            var nguoiDung = await _context.NguoiDungs.FindAsync(userId);
            if (nguoiDung == null)
                return NotFound(new { message = "Người dùng không tồn tại" });

            nguoiDung.Sdt = dto.Sdt;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật số điện thoại thành công" });
        }
        //===== CẬP NHẬT ẢNH ĐẠI DIỆN (CHỈ NGƯỜI ĐÃ ĐĂNG NHẬP) ======//
        [HttpPatch("CapNhatAnhDaiDien")]
        [Authorize]
        [SwaggerOperation(Summary = "Chỉnh sửa ảnh đại diện (cần đăng nhập)")]
        public async Task<IActionResult> CapNhatAnhDaiDien([FromBody] CapNhatNguoiDungDto dto)
        {
            if (string.IsNullOrEmpty(dto?.AnhDaiDien))
                return BadRequest(new { message = "Vui lòng cung cấp link ảnh đại diện mới" });

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc hết hạn" });

            var nguoiDung = await _context.NguoiDungs.FindAsync(userId);
            if (nguoiDung == null)
                return NotFound(new { message = "Người dùng không tồn tại" });

            nguoiDung.AnhDaiDien = dto.AnhDaiDien;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật ảnh đại diện thành công" });
        }

    }
}

