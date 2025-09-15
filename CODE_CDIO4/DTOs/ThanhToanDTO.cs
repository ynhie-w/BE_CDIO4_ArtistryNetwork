namespace CODE_CDIO4.DTOs
{
    public class ThanhToanDTO
    {
        public int Id_DonHang { get; set; }
        public string PhuongThuc { get; set; } = string.Empty;
        public string? TrangThai { get; set; } = "Chờ xử lý";
    }
}
