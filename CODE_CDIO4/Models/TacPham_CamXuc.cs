using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE_CDIO4.Models
{
    [Table("TACPHAM_CAMXUC")]
    public class TacPham_CamXuc
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_nguoidung")]
        public int Id_NguoiDung { get; set; }

        [Required]
        [Column("id_tacpham")]
        public int Id_TacPham { get; set; }

        [Required]
        [Column("id_camxuc")]
        public int Id_CamXuc { get; set; }

        [Column("ngaytao")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Required]
        [Column("trangthai")]
        public bool TrangThai { get; set; } = true;

        // 🔹 Navigation properties
        [ForeignKey(nameof(Id_NguoiDung))]
        public NguoiDung? NguoiDung { get; set; }

        [ForeignKey(nameof(Id_TacPham))]
        public TacPham? TacPham { get; set; }

        [ForeignKey(nameof(Id_CamXuc))]
        public CamXuc? CamXuc { get; set; }
    }
}
