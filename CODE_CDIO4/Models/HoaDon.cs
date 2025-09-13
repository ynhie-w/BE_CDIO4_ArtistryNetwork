using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CODE_CDIO4.Models
{
    [Table("HOADON")]
    public class HoaDon
    {
        [Key]
        public int Id { get; set; }
        [Column("id_donhang")]
        public int Id_DonHang { get; set; }
        [ForeignKey(nameof(Id_DonHang))]
        public DonHang? DonHang { get; set; }
        [Required]
        [Column("sohoadon")]
        [StringLength(50)]
        public string SoHoaDon { get; set; } = string.Empty;
        [Column("ngaylap")]
        public DateTime NgayLap { get; set; }
        [Column("nguoilap")]
        public int NguoiLap { get; set; }
        [Column("tongtien", TypeName = "decimal(12,2)")]
        public decimal TongTien { get; set; }

        [Column("ghichu")]
        [StringLength(255)]
        public string? GhiChu { get; set; }

        // Navigation
        [ForeignKey(nameof(NguoiLap))]
        public NguoiDung? NguoiLapHoaDon { get; set; }
        public ICollection<HoaDon_ChiTiet>? HoaDon_ChiTiets { get; set; } = new List<HoaDon_ChiTiet>();
    }
}
