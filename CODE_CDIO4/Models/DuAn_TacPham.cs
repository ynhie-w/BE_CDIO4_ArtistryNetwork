using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE_CDIO4.Models
{
    [Table("DUAN_TACPHAM")]
    public class DuAn_TacPham
    {
        // Khóa chính tổng hợp (composite key)
        [Key]
        [Column("id_tacpham")]
        public int Id_TacPham { get; set; }

        [Key]
        [Column("id_duan")]
        public int Id_DuAn { get; set; }

        [Column("trangthai")]
        public string TrangThai { get; set; }  

        [Column("ngaydang")]
        public DateTime NgayDang { get; set; }

        // 🔹 Navigation properties
        [ForeignKey(nameof(Id_DuAn))]
        public DuAnCongDong? DuAn { get; set; }

        [ForeignKey(nameof(Id_TacPham))]
        public TacPham? TacPham { get; set; }
    }
}