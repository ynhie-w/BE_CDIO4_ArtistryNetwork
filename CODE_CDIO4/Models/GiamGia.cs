using CODE_CDIO4.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("GIAMGIA")]
public class GiamGia
{
    [Key]
    public int Id { get; set; }
    public string MaGiamGia { get; set; } = "";
    public byte LoaiGiam { get; set; } 
    public decimal GiaTri { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public int? SoLanSuDung { get; set; }
    public int DaSuDung { get; set; } = 0;

    // Navigation
    public ICollection<DonHang>? DonHangs { get; set; } = new List<DonHang>();
}