namespace CDIO4_BE.Domain.DTOs
{
    /// <summary>
    /// DTO cho đăng nhập
    /// </summary>
    public class DangNhapDto
    {
        /// <summary>Email hoặc số điện thoại</summary>
        public string EmailSdt { get; set; }

        /// <summary>Mật khẩu</summary>
        public string MatKhau { get; set; }
    }

    /// <summary>
    /// DTO cho đăng ký
    /// </summary>
    public class DangKyDto
    {
        /// <summary>Họ và tên</summary>
        public string Ten { get; set; }

        /// <summary>Email hoặc Số điện thoại (có thể null)</summary>
        public string EmailSdt { get; set; }

        /// <summary>Mật khẩu (chưa hash, sẽ hash trong C#)</summary>
        public string MatKhau
        {
            get; set;

        }
    }

    /// <summary>
    /// DTO cho đổi mật khẩu
    /// </summary>
    public class DoiMatKhauDto
    {
        /// <summary>Mật khẩu cũ</summary>
        public string MatKhauCu { get; set; }

        /// <summary>Mật khẩu mới</summary>
        public string MatKhauMoi { get; set; }
    }


    /// <summary>
    /// DTO cho quên mật khẩu
    /// </summary>
    public class QuenMatKhauDto
    {
        /// <summary>Email hoặc số điện thoại để reset mật khẩu</summary>
        public string EmailSdt { get; set; }
    }

    public class DatLaiMatKhauDto
    {
        /// <summary>Token reset được gửi qua email</summary>
        public string Token { get; set; }

        /// <summary>Mật khẩu mới</summary>
        public string MatKhauMoi { get; set; }
    }

}
