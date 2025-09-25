namespace CDIO4_BE.Domain.DTOs
{
    public class DonHangDto
    {
        public int Id { get; set; }
        public DateTime NgayMua { get; set; }
        public string TrangThai { get; set; }
        public decimal TongTien { get; set; }
        public decimal GiamGia { get; set; }
        public List<DonHangChiTietDto> ChiTiets { get; set; } = new List<DonHangChiTietDto>();
    }
    public class ChiTietDto
    {
        public int Id { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
    }
    public class CapNhatTrangThaiDonHangDto
    {
        public string TrangThai { get; set; }
    }

    public class HuyDonHangDto
    {
        public string LyDoHuy { get; set; } = string.Empty;
    }

    public class DonHangChiTietDto
    {
        public int Id_TacPham { get; set; }
        public string TenTacPham { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
    }

}
