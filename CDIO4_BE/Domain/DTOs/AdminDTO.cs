namespace CDIO4_BE.Domain.DTOs
{
    public class TaoNguoiDungDto
    {
        public string Ten { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public string MatKhau { get; set; }
    }

    public class CapNhatSanPhamDto
    {
        public string TenSanPham { get; set; }
        public decimal Gia { get; set; }
        public string MoTa { get; set; }
    }
    public class KhoaTaiKhoanDto
    {
        public bool BiKhoa { get; set; }
    }
    public class TaoSanPhamDto
    {
        public string TenSanPham { get; set; }
        public decimal Gia { get; set; }
        public string MoTa { get; set; }
    }
}
