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

        public decimal TongTien { get; set; }           // Tổng tiền trước giảm giá
        public decimal GiamGiaTien { get; set; }       // Số tiền được giảm
        public decimal ThanhTien { get; set; }         // Tổng tiền sau khi giảm

        public int? IdGiamGia { get; set; }            // Id giảm giá (nếu có)
        public string? MaGiamGia { get; set; }         // Mã giảm giá (nếu có)

        public List<DonHangChiTietDTO> DonHang_ChiTiets { get; set; } = new();
    }

    public class DonHangView
    {
        public int IdDonHang { get; set; }
        public DateTime NgayMua { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = "";

        // Thông tin tác phẩm
        public int IdTacPham { get; set; }
        public string TenTacPham { get; set; } = "";
        public decimal Gia { get; set; }

        // Thông tin người mua
        public int IdNguoiMua { get; set; }
        public string TenNguoiMua { get; set; } = "";
    }
}
