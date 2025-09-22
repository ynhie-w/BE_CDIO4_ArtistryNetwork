using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CDIO4_BE.Domain.Entities
{
    [Table("CAMXUC")]
    public class CamXuc
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("ten")]
        [StringLength(50)]
        public string Ten { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<TacPham_CamXuc>? TacPham_CamXucs { get; set; }
    }
}