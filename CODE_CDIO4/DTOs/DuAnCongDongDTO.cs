namespace CODE_CDIO4.DTOs
{
    // Request khi tạo/cập nhật dự án
    public class DuAnCongDongRequestDTO
    {
        public string TenDuAn { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public int Id_QuanLy { get; set; }
    }

    // Response trả về client
    public class DuAnCongDongResponseDTO
    {
        public int Id { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? TenQuanLy { get; set; }
    }

    // DTO nhận message từ SP
    public class MessageResult
    {
        public string Message { get; set; } = string.Empty;
    }
}
