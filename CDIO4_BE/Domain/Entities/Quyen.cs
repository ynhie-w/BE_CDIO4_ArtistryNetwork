using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDIO4_BE.Domain.Entities
{
    [Table("QUYEN")]
    public class Quyen
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("ten")]
        public string Ten { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("mota")]
        public string? MoTa { get; set; }

        public ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
    }
}
