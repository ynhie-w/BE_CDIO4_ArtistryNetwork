using CDIO4_BE.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
    public DateTime NgayTao { get; set; }

    [Column("level")]
    public int Level { get; set; } = 0; // 0 = bình luận gốc, 1 = trả lời

    [Column("trangthai")]
    public bool TrangThai { get; set; } = true;

    // 🔹 Nếu là trả lời thì lưu id bình luận gốc
    [Column("id_binhluancha")]
    public int? Id_BinhLuanCha { get; set; }

    // 🔹 Navigation properties
    [ForeignKey(nameof(Id_NguoiDung))]
    [JsonIgnore]
    public NguoiDung? NguoiBinhLuan { get; set; }

    [ForeignKey(nameof(Id_TacPham))]
    [JsonIgnore]
    public TacPham? TacPham { get; set; }
}
