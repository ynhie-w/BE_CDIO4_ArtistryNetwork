namespace CODE_CDIO4.DTOs
{
    public class ThamGiaDuAnDTO
    {
        public int Id_DuAn { get; set; }
        public int Id_NguoiDung { get; set; }
        public string VaiTro { get; set; } = string.Empty;
    }

    public class UpdateThamGiaDTO
    {
        public int Id_DuAn { get; set; }
        public int Id_NguoiDungQuanLy { get; set; }
        public int Id_NguoiDungThamGia { get; set; }
        public string VaiTro { get; set; } = string.Empty;
    }

    public class DeleteThamGiaDTO
    {
        public int Id_DuAn { get; set; }
        public int Id_NguoiDungQuanLy { get; set; }
        public int Id_NguoiDungThamGia { get; set; }
    }
}

