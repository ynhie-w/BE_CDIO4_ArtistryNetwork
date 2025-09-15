namespace CODE_CDIO4.DTOs
{
    // DTO dùng để nhận request từ client
    public class TacPhamRequestDTO
    {
        public string Ten { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string Anh { get; set; } = string.Empty;
        public string? TenTheLoai { get; set; }
        public string? Hashtags { get; set; }
        public bool TrangThai { get; set; }
        public int Id_NguoiTao { get; set; }
        public decimal? Gia { get; set; }
    }

    // DTO để trả về cho client
    public class TacPhamResponseDTO
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string Anh { get; set; } = string.Empty;
        public string? TheLoai { get; set; }
        public List<string>? Hashtags { get; set; }
        public bool TrangThai { get; set; }
        public int Id_NguoiTao { get; set; }
        public decimal? Gia { get; set; }
    }
    public class TacPhamDTO
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public decimal? Gia { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public string TenTheLoai { get; set; } = string.Empty;
        public string TenNguoiTao { get; set; } = string.Empty;
    }
}
