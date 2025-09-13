using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CODE_CDIO4.Models
{
    [Table("BINHLUAN")]
    public class BinhLuan
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_tacpham")]
        public int Id_TacPham { get; set; }

        [Column("id_nguoidung")]
        public int Id_NguoiDung { get; set; }

        [Required]
        [Column("noidung", TypeName = "NVARCHAR(MAX)")]
        public string NoiDung { get; set; } = string.Empty;

        [Column("ngaytao")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Column("level")]
        public int Level { get; set; } = 0;

        // 🔹 Navigation properties
        [ForeignKey(nameof(Id_NguoiDung))]
        [JsonIgnore]
        public NguoiDung? NguoiBinhLuan { get; set; }
        [JsonIgnore]

        [ForeignKey(nameof(Id_TacPham))]
        public TacPham? TacPham { get; set; }
    }
}
