namespace CODE_CDIO4.DTOs
{
    public class DonHangChiTietDTO
    {
        public int Id_TacPham { get; set; }
        public string TenTacPham { get; set; } = "";
        public decimal Gia { get; set; }
    }

    public class DonHangDTO
    {
        public int Id { get; set; }
        public DateTime NgayMua { get; set; }
        public string TrangThai { get; set; } = "";
        public decimal TongTien { get; set; }
        public List<DonHangChiTietDTO> DonHang_ChiTiets { get; set; } = new();
    }
}
