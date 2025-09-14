using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CODE_CDIO4.Models
{
    [Table("DONHANG")]
    public class DonHang
    {
        [Key]
        public int Id { get; set; }
        [Column("id_nguoimua")]
        public int Id_NguoiMua { get; set; }
        [Column("ngaymua")]
        public DateTime NgayMua { get; set; }
        [Column("trangthai")]
        public string TrangThai { get; set; }
        [Column("tongtien", TypeName = "decimal(12,2)")]
        public decimal TongTien { get; set; } = 0;
        [Column("giamgia", TypeName = "decimal(5,2)")]
        public decimal GiamGia { get; set; } = 0;


        // Navigation
        [ForeignKey(nameof(Id_NguoiMua))]
        public NguoiDung? NguoiMua { get; set; }
        public ICollection<DonHang_ChiTiet>? DonHang_ChiTiets { get; set; } = new List<DonHang_ChiTiet>();
        public ICollection<ThanhToan>? ThanhToans { get; set; } = new List<ThanhToan>();
        public ICollection<HoaDon>? hoaDons { get; set; } = new List<HoaDon>();
    }
}
