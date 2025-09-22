namespace CDIO4_BE.Domain.DTOs
{
        public class CapNhatNguoiDungDto
        {
            public int Id { get; set; }
            public string? Ten { get; set; }
            public string? Email { get; set; }
            public string? Sdt { get; set; }
            public string? AnhDaiDien { get; set; }
            public string? MatKhauMoi { get; set; }
            public int? DiemThuong { get; set; }
            public bool? TrangThai { get; set; }
        }
    }
