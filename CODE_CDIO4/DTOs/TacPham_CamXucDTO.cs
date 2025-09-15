namespace CODE_CDIO4.DTOs
{
    public class TacPhamCamXucDTO
    {
        public int Id_NguoiDung { get; set; }
        public int Id_TacPham { get; set; }
        public int Id_CamXuc { get; set; }
        public DateTime NgayTao { get; set; }
    }

    // Request DTO (dùng cho POST/PUT)
    public class TacPhamCamXucRequest
    {
        public int Id_NguoiDung { get; set; }
        public int Id_TacPham { get; set; }
        public int Id_CamXuc { get; set; }
    }
}
