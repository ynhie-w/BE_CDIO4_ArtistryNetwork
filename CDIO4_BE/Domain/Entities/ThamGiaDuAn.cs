using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDIO4_BE.Domain.Entities
{
    [Table("THAMGIADUAN")]
    public class ThamGiaDuAn
    {
        [Key, Column("id_duan", Order = 0)]
        public int Id_DuAn { get; set; }

        [Key, Column("id_nguoidung", Order = 1)]
        public int Id_NguoiDung { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("vaitro")]
        public string VaiTro { get; set; } = "Tham gia";

        [Column("ngaythamgia")]
        public DateTime NgayThamGia { get; set; } = DateTime.Now;

        [Column("trangthai")]
        public bool TrangThai { get; set; } = true;

        // 🔹 Navigation
        [ForeignKey(nameof(Id_NguoiDung))]
        public NguoiDung? NguoiDung { get; set; }

        [ForeignKey(nameof(Id_DuAn))]
        public DuAnCongDong? DuAnCongDong { get; set; }
    }
}
