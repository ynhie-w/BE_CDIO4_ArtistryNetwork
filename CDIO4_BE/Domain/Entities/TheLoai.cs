using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDIO4_BE.Domain.Entities
{
    [Table("THELOAI")]
    public class TheLoai
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("ten")]
        [MaxLength(50)] 
        public string Ten { get; set; } = string.Empty;

        [Column("trangthai")]
        public bool TrangThai { get; set; } = true;

        // 🔹 Navigation
        public ICollection<TacPham>? TacPhams { get; set; } = new List<TacPham>();
    }
}
