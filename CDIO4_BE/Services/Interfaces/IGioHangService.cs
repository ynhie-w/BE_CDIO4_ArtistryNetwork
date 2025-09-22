using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDIO4_BE.Services.Interfaces
{
    public interface IGioHangService
    {
        Task<List<GioHangDto>> LayGioHang(int userId);
        Task<bool> ThemSanPham(int userId, ThemGioHangDto dto);
        Task<bool> XoaSanPham(int userId, int sanPhamId);
    }

    public interface IDonHangService
    {
        Task<List<DonHangDto>> LayDanhSachDonHang(int userId);
        Task<int> TaoDonHang(int userId, TaoDonHangDto dto);
    }

}
