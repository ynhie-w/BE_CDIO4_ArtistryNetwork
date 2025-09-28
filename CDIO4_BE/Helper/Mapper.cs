using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;

namespace CDIO4_BE.Helper
{
    public static class Mapper
    {
        public static TacPhamDto MapToTacPhamDto(TacPham tp, int? currentUserId = null)
        {
            return new TacPhamDto
            {
                Id = tp.Id,
                Ten = tp.Ten,
                MoTa = tp.MoTa,
                Anh = tp.Anh,
                Gia = tp.Gia,
                LuotXem = tp.LuotXem,
                NgayTao = tp.NgayTao,

                NguoiTao = tp.NguoiTao == null ? null : new NguoiDungDto
                {
                    Id = tp.NguoiTao.Id,
                    Ten = tp.NguoiTao.Ten,
                    AnhDaiDien = tp.NguoiTao.AnhDaiDien
                },

                ThongKe = new ThongKeDto
                {
                    LuotThich = tp.TacPham_CamXucs.Count(c => c.TrangThai),
                    LuotBinhLuan = tp.BinhLuans.Count(b => b.TrangThai)
                },

                CamXucCuaToi = currentUserId != null
                    ? tp.TacPham_CamXucs
                        .Where(tc => tc.Id_NguoiDung == currentUserId && tc.TrangThai)
                        .Select(tc => tc.CamXuc.Ten)
                        .FirstOrDefault()
                    : null
            };
        }
    }
}
