using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE_CDIO4.Models
{
    [Table("THELOAI")]
    public class TheLoai
    {
        [Key]
        [Column("id")]
        public int Id{ get; set; }

        [Required]
        [Column("ten")]
        [MaxLength(100)]
        public string Ten { get; set; } = string.Empty;

        // 🔹 Navigation
        public ICollection<TacPham>? TacPhams { get; set; } = new List<TacPham>();
    }
}
