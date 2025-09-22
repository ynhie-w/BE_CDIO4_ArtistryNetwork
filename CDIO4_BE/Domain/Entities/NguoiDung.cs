
using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
namespace CDIO4_BE.Domain.Entities
{
        [Table("NGUOIDUNG")]
        public class NguoiDung
        {
            [Key]
            [Column("id")]
            public int Id { get; set; }

            [Required]
            [Column("ten")]
            [StringLength(50)]
            public string Ten { get; set; } = string.Empty;

            [Column("anhdaidien")]
            [StringLength(255)]
            public string? AnhDaiDien { get; set; }

            [Column("sdt")]
            [StringLength(15)]
            public string? Sdt { get; set; }

            [Column("email")]
            [StringLength(100)]
            public string? Email { get; set; }

            [Required]
            [Column("matkhau")]
            public byte[] MatKhau { get; set; } = null!;

            [Column("diemthuong")]
            public int DiemThuong { get; set; } = 0;

            [Column("id_capdo")]
            public int? Id_CapDo { get; set; }
            [ForeignKey(nameof(Id_CapDo))]
            public CapDo? CapDo { get; set; }

            [Column("id_phanquyen")]
            public int Id_PhanQuyen { get; set; } = 1;
          
            [Column("ngaytao")]
            public DateTime NgayTao { get; set; } = DateTime.Now;

            [Column("trangthai")]
            public bool TrangThai { get; set; } = true;

            [ForeignKey(nameof(Id_PhanQuyen))]
            [JsonIgnore] // Ngăn vòng lặp khi serialize
            public Quyen? Quyen { get; set; }
            [JsonIgnore]

            public ICollection<TacPham>? TacPhams { get; set; }
            [JsonIgnore]
            public ICollection<GioHang>? GioHangs { get; set; }
            [JsonIgnore]
            public ICollection<DonHang>? DonHangs { get; set; }
            [JsonIgnore]
            public ICollection<BinhLuan>? BinhLuans { get; set; }
            [JsonIgnore]
            public ICollection<TacPham_CamXuc>? TacPham_CamXucs { get; set; }
            [JsonIgnore]
            public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
            [JsonIgnore]
            public ICollection<ThongBao>? ThongBaos { get; set; }
            [JsonIgnore]
            public ICollection<DuAnCongDong>? DuAnQuanLy { get; set; }
            [JsonIgnore]
            public ICollection<ThamGiaDuAn>? ThamGiaDuAns { get; set; }
            public ICollection<BoSuuTap>? BoSuuTaps { get; set; }
        public ICollection<DatLaiMatKhauToken> datLaiMatKhauTokens { get; set; }
        }
    }

