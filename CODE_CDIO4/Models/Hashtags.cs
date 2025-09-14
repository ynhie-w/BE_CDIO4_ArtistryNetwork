using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE_CDIO4.Models
{
    [Table("HASHTAGS")]
    public class Hashtag
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("ten")]
        [StringLength(50)]
        public string Ten { get; set; } = string.Empty;
        [Column("trangthai")]
        public bool TrangThai { get; set; } = true;
        // 🔹 Navigation
        public ICollection<TacPham_Hashtags>? TacPham_hashtags { get; set; } = new List<TacPham_Hashtags>();
    }
}
