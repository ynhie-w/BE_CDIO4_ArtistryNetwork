namespace CODE_CDIO4.DTOs
{
    public class ThongBaoDTO
    {
        public int IdNguoiDung { get; set; }
        public string NoiDung { get; set; } = string.Empty;
    }

    public class DeleteThongBaoDTO
    {
        public int IdThongBao { get; set; }
        public int IdNguoiDung { get; set; }
    }
}
