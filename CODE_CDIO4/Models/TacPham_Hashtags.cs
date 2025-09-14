using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE_CDIO4.Models
{
    [Table("TACPHAM_HASHTAGS")]
    public class TacPham_Hashtags
    {
        [Required]
        [Column("id_tacpham")]
        public int Id_TacPham { get; set; }

        [Required]
        [Column("id_hashtag")]
        public int Id_Hashtag { get; set; }
        [Column("trangthai")]
        public bool TrangThai { get; set; } = true;
        // 🔹 Navigation
        [ForeignKey(nameof(Id_TacPham))]
        public TacPham? TacPham { get; set; }

        [ForeignKey(nameof(Id_Hashtag))]
        public Hashtag? Hashtag { get; set; }
    }
}
