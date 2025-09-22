using CDIO4_BE.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("DatLaiMatKhauTokens")]
public class DatLaiMatKhauToken
{
    [Key]
    public int Id { get; set; }

    [Column("Id_NguoiDung")]
    public int Id_NguoiDung { get; set; }

    [Column("Token")]
    public string Token { get; set; }

    [Column("Han")]
    public DateTime Han { get; set; }

    [ForeignKey(nameof(Id_NguoiDung))]
    public NguoiDung NguoiDung { get; set; }
}
