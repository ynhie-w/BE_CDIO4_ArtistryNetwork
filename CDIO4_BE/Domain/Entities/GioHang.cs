using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDIO4_BE.Domain.Entities
{
    [Table("GIOHANG")]
    public class GioHang
    {
        [Column("id_nguoidung")]
        public int Id_NguoiMua { get; set; }

        [Column("id_tacpham")]
        public int Id_TacPham { get; set; }

        [Column("ngaythem")]
        public DateTime NgayThem { get; set; } = DateTime.Now;

        [Column("trangthai")]
        public bool TrangThai { get; set; } = true;
        // 🔹 Navigation properties
        [ForeignKey(nameof(Id_NguoiMua))]
        public NguoiDung? NguoiMua { get; set; }

        [ForeignKey(nameof(Id_TacPham))]
        public TacPham? TacPham { get; set; }
    }
}

