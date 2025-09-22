using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDIO4_BE.Domain.Entities
{
    [Table("THONGBAO")]
    public class ThongBao
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_nguoidung")]
        public int Id_NguoiDung { get; set; }

        [Column("noidung", TypeName = "NVARCHAR(MAX)")]
        public string? NoiDung { get; set; }

        [Column("dadoc")]
        public bool DaDoc { get; set; } = false;

        [Column("ngaytao")]
        public DateTime NgayTao { get; set; }

        [Column("trangthai")]
        public bool TrangThai { get; set; } = true;

        // 🔹 Navigation
        [ForeignKey(nameof(Id_NguoiDung))]
        public NguoiDung? NguoiDung { get; set; }
    }
}
