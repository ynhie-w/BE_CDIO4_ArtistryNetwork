using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CODE_CDIO4.Models;
using System.Text.Json.Serialization;

[Table("TACPHAM")]
public class TacPham
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("ten")]
    public string Ten { get; set; } = string.Empty;

    [Column("mota", TypeName = "NVARCHAR(MAX)")]
    public string? MoTa { get; set; }

    [MaxLength(255)]
    [Column("anh")]
    public string? Anh { get; set; }

    [Column("id_theloai")]
    public int? Id_TheLoai { get; set; }
    [Column("id_nguoitao")]
    public int Id_NguoiTao { get; set; }

    [Column("ngaytao")]
    public DateTime NgayTao { get; set; }

    [Required]
    [Column("gia", TypeName = "decimal(18,2)")]
    public decimal Gia { get; set; } = 0;

    [Column("luotxem")]
    public int LuotXem { get; set; }
    [Column("trangthai")]
    public bool TrangThai { get; set; } = true;
    // ================== Navigation Properties ==================
    [ForeignKey(nameof(Id_NguoiTao))]
    public NguoiDung? NguoiTao { get; set; }

    [ForeignKey(nameof(Id_TheLoai))]
    public TheLoai? TheLoai { get; set; }

    public ICollection<TacPham_Hashtags>? TacPham_Hashtags { get; set; }
    public ICollection<TacPham_CamXuc>? TacPham_CamXucs { get; set; }
    public ICollection<DonHang_ChiTiet>? DonHang_ChiTiets { get; set; }
    public ICollection<HoaDon_ChiTiet>? HoaDon_ChiTiets { get; set; }
    public ICollection<BoSuuTap>? BoSuuTaps { get; set; }
    public ICollection<GioHang>? GioHangs { get; set; }
    [JsonIgnore]
    public ICollection<BinhLuan>? BinhLuans { get; set; }
    public ICollection<DuAn_TacPham>? DuAn_TacPhams { get; set; }
    public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
}
