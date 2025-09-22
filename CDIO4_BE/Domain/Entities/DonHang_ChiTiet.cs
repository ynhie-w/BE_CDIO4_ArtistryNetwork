using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDIO4_BE.Domain.Entities
{
    [Table("DONHANG_CHITIET")]
    public class DonHang_ChiTiet
    {
        [Column("id_donhang")]
        public int Id_DonHang { get; set; }

        [Column("id_tacpham")]
        public int Id_TacPham { get; set; }

        [ForeignKey(nameof(Id_DonHang))]
        public DonHang DonHang { get; set; } = null!;

        [ForeignKey(nameof(Id_TacPham))]
        public TacPham TacPham { get; set; } = null!;
    }
}