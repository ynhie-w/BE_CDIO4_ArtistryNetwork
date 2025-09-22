using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CDIO4_BE.Domain.Entities
{
    [Table("BOSUUTAP")]
    public class BoSuuTap
    {
        [Column("id_nguoidung")]
        public int Id_NguoiDung { get; set; }

        [Column("id_tacpham")]
        public int Id_TacPham { get; set; }

        [Column("ngaythem")]
        public DateTime NgayThem { get; set; }

        [Column("trangthai")]
        public bool TrangThai { get; set; } = true;

        // 🔹 Navigation properties
        [ForeignKey(nameof(Id_NguoiDung))]
        [JsonIgnore]
        public NguoiDung? NguoiDung { get; set; }

        [ForeignKey(nameof(Id_TacPham))]
        [JsonIgnore]
        public TacPham? TacPham { get; set; }
    }
}
