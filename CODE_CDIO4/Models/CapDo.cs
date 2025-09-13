using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE_CDIO4.Models
{
    [Table("CAPDO")]
    public class CapDo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("ten")]
        public string Ten { get; set; } = string.Empty;

        [Required]
        [Column("diemtu")]
        public int DiemTu { get; set; }

        [Required]
        [Column("diemden")]
        public int DiemDen { get; set; }
        public ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
    }
}

