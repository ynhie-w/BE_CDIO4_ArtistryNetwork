using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CDIO4_BE.Domain.Entities
{
    [Table("HOADON_CHITIET")]
    public class HoaDon_ChiTiet
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_hoadon")]
        public int Id_HoaDon { get; set; }

        [Column("id_tacpham")]
        public int Id_TacPham { get; set; }

        [Required]
        [Column("thanhtien", TypeName = "decimal(12,2)")]
        public decimal ThanhTien { get; set; }
        [Column("trangthai")]
        public bool TrangThai { get; set; } = true;
        // 🔹 Navigation
        [ForeignKey(nameof(Id_HoaDon))]
        public HoaDon? HoaDon { get; set; }

        [ForeignKey(nameof(Id_TacPham))]
        public TacPham? TacPham { get; set; }
    }
}
