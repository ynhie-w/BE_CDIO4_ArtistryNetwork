// File: DTOs/DanhGiaDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace CODE_CDIO4.DTOs
{
    public class DanhGiaDTO
    {
        public int Id { get; set; }
        public int Id_TacPham { get; set; }
        public int Id_NguoiDung { get; set; }
        public int Diem { get; set; }
        public DateTime NgayTao { get; set; }
        public string TenNguoiDung { get; set; } = string.Empty;
    }

    public class DanhGiaInsertDTO
    {
        [Required]
        public int Id_TacPham { get; set; }

        [Required]
        public int Id_NguoiDung { get; set; }

        [Range(1, 5)]
        public int Diem { get; set; }
    }

    public class DanhGiaUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int Id_NguoiDung { get; set; }

        [Range(1, 5)]
        public int Diem { get; set; }
    }
}
