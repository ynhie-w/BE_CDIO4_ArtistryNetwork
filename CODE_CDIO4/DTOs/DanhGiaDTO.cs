// File: DTOs/DanhGiaDto.cs
namespace CODE_CDIO4.DTOs
{
    public class DanhGiaDto
    {
        public int Id { get; set; }
        public int Id_TacPham { get; set; }
        public int Diem { get; set; }
        public DateTime NgayTao { get; set; }
        // Chỉ bao gồm thông tin đơn giản của người dùng để tránh vòng lặp
        public string? TenNguoiDung { get; set; }
    }
}