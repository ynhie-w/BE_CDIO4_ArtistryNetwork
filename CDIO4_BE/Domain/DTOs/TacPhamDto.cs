namespace CDIO4_BE.Domain.DTOs
{
    // Người dùng
    public class NguoiDungDto
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string? AnhDaiDien { get; set; }
    }

    // Tác phẩm (list, chi tiết, thêm, sửa)
    public class TacPhamListDto
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string? Anh { get; set; }
        public DateTime NgayTao { get; set; }
        public NguoiDungDto? NguoiTao { get; set; }
        public int SoLuongBinhLuan { get; set; }
        public int SoLuongCamXuc { get; set; }
        public int LuotXem { get; set; }
    }

    public class TacPhamDto
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string? Anh { get; set; }
        public decimal Gia { get; set; }
        public int LuotXem { get; set; }
        public DateTime NgayTao { get; set; }
        public NguoiDungDto? NguoiTao { get; set; }
        public ThongKeDto ThongKe { get; set; }
        public string? CamXucCuaToi { get; set; }
    }
    public class ThongKeDto
    {
        public int LuotThich { get; set; }
        public int LuotBinhLuan { get; set; }
    }

    public class TaoTacPhamDto
    {
        public string Ten { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public List<string> Anh { get; set; } = new();
        public int? Id_TheLoai { get; set; }
        public decimal Gia { get; set; }
    }

    public class SuaTacPhamDto
    {
        public string? Ten { get; set; }
        public string? MoTa { get; set; }
        public List<string>? Anh { get; set; }
        public decimal? Gia { get; set; }
        public int? Id_TheLoai { get; set; }
    }

    // Bình luận
    public class BinhLuanDto
    {
        public int Id { get; set; }
        public NguoiDungDto? NguoiBinhLuan { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public int Level { get; set; }
        public List<BinhLuanDto> TraLoi { get; set; } = new();
        public bool ChuSoHuu { get; set; }
    }

    public class ThemBinhLuanRequest
    {
        public string NoiDung { get; set; } = string.Empty;
        public int? IdBinhLuanCha { get; set; }
    }

    public class SuaBinhLuanDto
    {
        public int IdBinhLuan { get; set; }
        public string NoiDungMoi { get; set; }
    }

    // Cảm xúc
    public class CamXucDto
    {
        public string TenCamXuc { get; set; } = string.Empty;
        public NguoiDungDto? NguoiCamXuc { get; set; }
    }
    public class CamXucRequest
    {
        public int IdCamXuc { get; set; }
    }
    public class UpsertCamXucDto
    {
        public int IdTacPham { get; set; }
        public int IdCamXuc { get; set; }
    }
}
