using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE_CDIO4.Models
{
    [Table("TACPHAM_HASHTAGS")]
    public class TacPham_Hashtags
    {
        [Key, Column("id_tacpham", Order = 0)]
        public int Id_TacPham { get; set; }

        [Key, Column("id_hashtag", Order = 1)]
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
