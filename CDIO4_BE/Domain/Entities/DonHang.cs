using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDIO4_BE.Domain.Entities
{
    [Table("DONHANG")]
    public class DonHang
    {
        [Key]
        public int Id { get; set; }

        [Column("id_nguoimua")]
        public int? Id_NguoiMua { get; set; } // có thể null khi xóa mềm

        [Column("ngaymua")]
        public DateTime NgayMua { get; set; } = DateTime.Now;

        [Column("trangthai")]
        public string TrangThai { get; set; } = "Đang xử lý";

        [Column("tongtien", TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; } = 0;

        [Column("giamgia", TypeName = "decimal(18,2)")]
        public decimal GiamGia { get; set; } = 0;

        [Column("IdGiamGia")]
        public int? IdGiamGia { get; set; } // FK tới GiamGia

        // Navigation
        [ForeignKey(nameof(Id_NguoiMua))]
        public NguoiDung? NguoiMua { get; set; }

        [ForeignKey(nameof(IdGiamGia))]
        public GiamGia? GiamGias { get; set; } 

        public ICollection<DonHang_ChiTiet>? DonHang_ChiTiets { get; set; } = new List<DonHang_ChiTiet>();
        public ICollection<ThanhToan>? ThanhToans { get; set; } = new List<ThanhToan>();
        public ICollection<HoaDon>? HoaDons { get; set; } = new List<HoaDon>();
    }
}
