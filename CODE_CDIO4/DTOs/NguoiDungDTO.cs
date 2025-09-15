namespace CODE_CDIO4.DTOs
{
    public class CreateUserDto
    {
        public string Ten { get; set; } = null!;
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public string MatKhau { get; set; } = null!;
        public int? DiemThuong { get; set; }
        public int? Id_CapDo { get; set; }
        public int? Id_PhanQuyen { get; set; }
        public string? AnhDaiDien { get; set; }
    }
    public class UpdateUserDto
    {
        public string? Ten { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public string? MatKhau { get; set; }
        public int? DiemThuong { get; set; }
        public int? Id_CapDo { get; set; }
        public int? Id_PhanQuyen { get; set; }
        public bool? TrangThai { get; set; }
        public string? AnhDaiDien { get; set; }
    }
    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Sdt { get; set; }
        public string MatKhau { get; set; } = null!;
    }
    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Ten { get; set; } = null!;
        public string? Email { get; set; }
        public string? Sdt { get; set; }
        public int Id_PhanQuyen { get; set; }
        public bool TrangThai { get; set; }
    }
}
