using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE_CDIO4.Models
{
    [Table("THAMGIADUAN")]
    public class ThamGiaDuAn
    {
        [Column("id_duan")]
        public int Id_DuAn { get; set; }

        [Column("id_nguoidung")]
        public int Id_NguoiDung { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("vaitro")]
        public string VaiTro { get; set; } = "Tham gia";

        [Column("ngaythamgia")]
        public DateTime NgayThamGia { get; set; } = DateTime.Now;

        // 🔹 Navigation
        [ForeignKey(nameof(Id_NguoiDung))]
        public NguoiDung? NguoiDung { get; set; }

        [ForeignKey(nameof(Id_DuAn))]
        public DuAnCongDong? DuAnCongDong { get; set; }
    }
}
