namespace CDIO4_BE.Domain.DTOs
{
    public class GioHangDto
    {
        public int Id_TacPham { get; set; }
        public string TenTacPham { get; set; }
        public DateTime NgayThem { get; set; }
    }
    public class ThemGioHangDto
    {
        public int SanPhamId { get; set; }
        public int SoLuong { get; set; }
    }

    public class TaoDonHangDto
    {
        public string DiaChiGiaoHang { get; set; }
        public string HinhThucThanhToan { get; set; }
        // có thể thêm các trường khác nếu cần
    }
}
