using CDIO4_BE.Domain.DTOs;
using System.Security.Claims;

namespace CDIO4_BE.Services.Interfaces
{
    public interface ITaiKhoanService
    {
        // Đăng nhập, trả về JWT token nếu thành công
        Task<DangNhapResponseDto?> DangNhap(DangNhapDto yeuCau);

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
        // Lấy thông tin người dùng
        Task<CapNhatNguoiDungDto?> LayThongTin(ClaimsPrincipal user);

        // Cập nhật email
        Task<bool> CapNhatEmail(ClaimsPrincipal user, string email);

        // Cập nhật số điện thoại
        Task<bool> CapNhatSdt(ClaimsPrincipal user, string sdt);

        // Cập nhật ảnh đại diện
        Task<bool> CapNhatAnhDaiDien(ClaimsPrincipal user, string anhDaiDien);
    }
}
