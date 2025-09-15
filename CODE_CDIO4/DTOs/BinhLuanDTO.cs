using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CODE_CDIO4.DTOs
{
    public class BinhLuanViewDTO
    {
        public int Id { get; set; }
        public int IdTacPham { get; set; }
        public int IdNguoiDung { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }

        // Thông tin người bình luận
        public string TenNguoiBinhLuan { get; set; } = string.Empty;
        public string? AnhDaiDien { get; set; }
    }

    public class BinhLuanInsertDTO
    {
        [Required]
        [SwaggerSchema("ID tác phẩm mà bình luận thuộc về")]
        public int IdTacPham { get; set; }

        [Required]
        [SwaggerSchema("ID người dùng tạo bình luận")]
        public int IdNguoiDung { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Nội dung tối đa 500 ký tự")]
        [SwaggerSchema("Nội dung bình luận")]
        public string NoiDung { get; set; }

        [Required]
        [Range(0, 5, ErrorMessage = "Level từ 0 đến 5")]
        [SwaggerSchema("Level của bình luận")]
        public int Level { get; set; }
    }
  
    public class BinhLuanUpdateDTO
    {
        [Required]
        [StringLength(500, ErrorMessage = "Nội dung tối đa 500 ký tự")]
        [SwaggerSchema("Nội dung bình luận mới")]
        public string NoiDungMoi { get; set; }

        [Required]
        [SwaggerSchema("ID người dùng yêu cầu sửa")]
        public int IdNguoiDung { get; set; }
    }

    public class BinhLuanDeleteDTO
    {
        public int IdNguoiDung { get; set; }
    }
}
