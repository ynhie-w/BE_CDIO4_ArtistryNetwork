namespace CODE_CDIO4.DTOs
{
    public class TacPhamDTO
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string? Anh { get; set; }
        public decimal Gia { get; set; }
        public bool TrangThai { get; set; }
        public string Loai { get; set; } = "Bán";
        public DateTime NgayTao { get; set; }
        public int LuotXem { get; set; }

        // Thông tin thêm từ navigation
        public string? TenTheLoai { get; set; }
        public string? TenNguoiTao { get; set; }
    }
}
