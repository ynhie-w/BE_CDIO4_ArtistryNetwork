namespace CDIO4_BE.Domain.DTOs
{
    // Đăng nhập
    public class DangNhapDto
    {
        public string EmailSdt { get; set; }
        public string MatKhau { get; set; }
    }
    public class DangNhapResponseDto
    {
        public string Token { get; set; }
        public CapNhatNguoiDungDto NguoiDung { get; set; }
    }
        public class CapNhatNguoiDungDto
        {
            public int Id { get; set; }
            public string Ten { get; set; }
            public string Email { get; set; }
            public string Sdt { get; set; }
            public string AnhDaiDien { get; set; }
            public int Id_Quyen { get; set; }
            public bool TrangThai { get; set; }
            public DateTime NgayTao { get; set; }
        }
    // Đăng ký
    public class DangKyDto
    {
        public string Ten { get; set; }             
        public string EmailSdt { get; set; }        
        public string MatKhau { get; set; } 
        public string NhapLaiMatKhau { get; set; }
    }

    // Đổi mật khẩu
    public class DoiMatKhauDto
    {
        public string MatKhauCu { get; set; }
        public string MatKhauMoi { get; set; }
        public string MatKhauMoiNhapLai { get; set; }
    }

    // Quên mật khẩu
    public class QuenMatKhauDto
    {
        public string EmailSdt { get; set; }
    }

    // Đặt lại mật khẩu
    public class DatLaiMatKhauDto
    {
        public string Token { get; set; }  
        public string MatKhauMoi { get; set; }
    }
  
}
