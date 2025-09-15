namespace CODE_CDIO4.DTOs
{
    // DTO khi tạo hóa đơn
    public class CreateHoaDonDTO
    {
        public int? Id_NguoiMua { get; set; }      // Id người mua (nullable, phòng TH mua hộ)
        public DateTime NgayLap { get; set; } = DateTime.Now; // Ngày lập hóa đơn
        public decimal TongTien { get; set; }      // Tổng tiền
        public string? GhiChu { get; set; }        // Ghi chú thêm (nếu có)
        public List<int>? ChiTietSanPhamIds { get; set; } // danh sách sản phẩm (id) trong hóa đơn
    }

    // DTO trả về cho client (xem hóa đơn)
    public class HoaDonDTO
    {
        public int Id { get; set; }
        public int Id_DonHang { get; set; }
        public string? SoHoaDon { get; set; }
        public DateTime NgayLap { get; set; }

        public int Id_NguoiLap { get; set; }     // FK trong DB
        public string? TenNguoiLap { get; set; } // để hiển thị ra ngoài

        public decimal ThanhTien { get; set; }
        public string? GhiChu { get; set; }
        public bool TrangThai { get; set; }
        public List<HoaDonChiTietDTO>? ChiTiet { get; set; }
    }


    public class HoaDonChiTietDTO
    {
        public int Id { get; set; }
        public int Id_HoaDon { get; set; }
        public int Id_TacPham { get; set; }
        public decimal ThanhTien { get; set; }
        public bool TrangThai { get; set; }
    }

}
