namespace CODE_CDIO4.DTOs
{
    public class GiamGiaDTO
    {
        public int Id { get; set; }
        public string MaGiamGia { get; set; } = "";
        public byte LoaiGiam { get; set; } // 0 = %, 1 = tiền mặt
        public decimal GiaTri { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int? SoLanSuDung { get; set; }
        public int DaSuDung { get; set; }
    }

    public class CreateGiamGiaDTO
    {
        public string MaGiamGia { get; set; } = "";
        public byte LoaiGiam { get; set; }
        public decimal GiaTri { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int? SoLanSuDung { get; set; }
    }
}
