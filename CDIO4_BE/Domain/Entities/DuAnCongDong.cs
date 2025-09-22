using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CDIO4_BE.Domain.Entities
{
    [Table("DUANCONGDONG")]
    public class DuAnCongDong
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("tenduan")]
        [Required, StringLength(100)]
        public string TenDuAn { get; set; } = string.Empty;

        [Column("mota")]
        public string? MoTa { get; set; }

        [Column("ngaytao")]
        public DateTime NgayTao { get; set; }

        [Column("ngaybatdau")]
        public DateTime? NgayBatDau { get; set; }   

        [Column("ngayketthuc")]
        public DateTime? NgayKetThuc { get; set; }  

        [Column("id_quanly")]
        public int Id_QuanLy { get; set; }

        [Column("trangthai")]
        [MaxLength(20)]
        public string TrangThai { get; set; } 
        [JsonIgnore]
        // 🔹 Navigation properties
        [ForeignKey(nameof(Id_QuanLy))]
        public NguoiDung? QuanLy { get; set; }

        public ICollection<ThamGiaDuAn>? ThamGiaDuAns { get; set; }
        public ICollection<DuAn_TacPham>? DuAn_TacPhams { get; set; }
    }
}
