using CDIO4_BE.Domain.DTOs;
using System.Security.Claims;

namespace CDIO4_BE.Services.Interfaces
{
    public interface ITaiKhoanService
    {
        // Đăng nhập, trả về JWT token nếu thành công
        Task<string> DangNhap(DangNhapDto yeuCau);

        // Đăng xuất, xoá token hoặc session nếu cần
        Task<bool> DangXuat();

        // Đăng ký tài khoản mới, trả về ID người dùng
        Task<int> DangKy(DangKyDto dto);

        // Đổi mật khẩu hiện tại
        Task<bool> DoiMatKhau(ClaimsPrincipal user, DoiMatKhauDto dto);

        // Quên mật khẩu, gửi hướng dẫn reset
        Task<bool> QuenMatKhau(QuenMatKhauDto dto);

        // Đặt lại mật khẩu bằng token reset
        Task<bool> DatLaiMatKhau(DatLaiMatKhauDto dto);
    }
}
