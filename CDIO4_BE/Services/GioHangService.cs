using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDIO4_BE.Services
{
    public class GioHangService : IGioHangService
    {
        private readonly AppDbContext _dbContext;
        public GioHangService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<GioHangDto>> LayGioHang(int userId)
        {
            var gioHang = await _dbContext.GioHangs
                .Include(g => g.TacPham)
                .Where(g => g.Id_NguoiMua == userId && g.TrangThai)
                .ToListAsync();

            return gioHang.Select(g => new GioHangDto
            {
                Id_TacPham = g.Id_TacPham,
                TenTacPham = g.TacPham?.Ten ?? "",
                NgayThem = g.NgayThem
            }).ToList();
        }


        public async Task<bool> ThemSanPham(int userId, ThemGioHangDto dto)
        {
            // Kiểm tra xem sản phẩm đã tồn tại trong giỏ chưa
            var item = await _dbContext.GioHangs
                .FirstOrDefaultAsync(g => g.Id_NguoiMua == userId
                                      && g.Id_TacPham == dto.SanPhamId
                                      && g.TrangThai);

            if (item != null)
            {
                // Nếu muốn tăng số lượng, cần thêm cột SoLuong (hiện tại bỏ qua)
                return true; // Sản phẩm đã có, coi như thành công
            }

            // Thêm sản phẩm mới
            _dbContext.GioHangs.Add(new GioHang
            {
                Id_NguoiMua = userId,
                Id_TacPham = dto.SanPhamId,
                NgayThem = DateTime.Now,
                TrangThai = true
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> XoaSanPham(int userId, int sanPhamId)
        {
            var item = await _dbContext.GioHangs
                .FirstOrDefaultAsync(g => g.Id_NguoiMua == userId
                                      && g.Id_TacPham == sanPhamId);

            if (item == null) return false;

            _dbContext.GioHangs.Remove(item);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
