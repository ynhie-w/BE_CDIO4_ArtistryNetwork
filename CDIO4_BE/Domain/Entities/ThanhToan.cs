using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDIO4_BE.Domain.Entities
{
    [Table("THANHTOAN")]
    public class ThanhToan
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_donhang")]
        public int Id_DonHang { get; set; }

        [Required]
        [Column("phuongthuc")]
        [MaxLength(20)]
        public string PhuongThuc { get; set; } = string.Empty; // momo, vnpay, paypal

        [Required]
        [Column("trangthai")]
        [MaxLength(20)]
        public string TrangThai { get; set; } = "Chờ xử lý";

        [Column("ngaytt")]
        public DateTime NgayTT { get; set; } = DateTime.Now;

        // 🔹 Navigation
        [ForeignKey(nameof(Id_DonHang))]
        public DonHang? DonHang { get; set; }
    }
}
