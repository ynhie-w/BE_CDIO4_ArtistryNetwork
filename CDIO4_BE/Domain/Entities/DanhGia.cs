using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDIO4_BE.Domain.Entities
{
    [Table("DANHGIA")]
    public class DanhGia
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_tacpham")]
        public int Id_TacPham { get; set; }

        [Column("id_nguoidung")]
        public int Id_NguoiDung { get; set; }

        [Column("diem")]
        public int Diem { get; set; }


        [Column("ngaytao")]
        public DateTime NgayTao { get; set; }
        [Column("trangthai")]
        public bool TrangThai { get; set; } = true;
        // 🔹 Navigation properties
        [ForeignKey(nameof(Id_NguoiDung))]
        public NguoiDung? NguoiDanhGia { get; set; }

        [ForeignKey(nameof(Id_TacPham))]
        public TacPham? TacPham { get; set; }
    }
}