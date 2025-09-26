
    namespace CDIO4_BE.Domain.DTOs
    {

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
        public int LuotXem {  get; set; }
    }

    public class NguoiDungDto
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string? AnhDaiDien { get; set; }
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

    }

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

    // DTO cho request bình luận
    public class ThemBinhLuanRequest
    {
        public string NoiDung { get; set; } = string.Empty;
        public int? IdBinhLuanCha { get; set; } // Nếu là trả lời bình luận
    }
    public class CamXucDto
    {
        public string TenCamXuc { get; set; } = string.Empty;
        public NguoiDungDto? NguoiCamXuc { get; set; }
    }
    public class SuaBinhLuanDto
    {
        public int IdBinhLuan { get; set; }
        public string NoiDungMoi { get; set; }
    }

    public class UpsertCamXucDto
    {
        public int IdTacPham { get; set; }
        public int IdCamXuc { get; set; } // Ví dụ: Like, Love, Haha...
    }


}