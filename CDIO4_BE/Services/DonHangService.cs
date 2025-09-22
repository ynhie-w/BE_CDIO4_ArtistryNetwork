using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CDIO4_BE.Services
{
    public class DonHangService : IDonHangService
    {
        private readonly AppDbContext _dbContext;

        public DonHangService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DonHangDto>> LayDanhSachDonHang(int userId)
        {
            var donHangs = await _dbContext.DonHangs
                .Include(d => d.DonHang_ChiTiets)
                    .ThenInclude(ct => ct.TacPham)
                .Where(d => d.Id_NguoiMua == userId)
                .ToListAsync();

            return donHangs.Select(d => new DonHangDto
            {
                Id = d.Id,
                NgayMua = d.NgayMua,
                TrangThai = d.TrangThai,
                TongTien = d.TongTien,
                GiamGia = d.GiamGia,
                ChiTiets = d.DonHang_ChiTiets?.Select(ct => new DonHangChiTietDto
                {
                    Id_TacPham = ct.Id_TacPham,
                    TenTacPham = ct.TacPham?.Ten ?? "",
                }).ToList() ?? new List<DonHangChiTietDto>()
            }).ToList();
        }

        public async Task<int> TaoDonHang(int userId, TaoDonHangDto dto)
        {
            var newOrder = new DonHang
            {
                Id_NguoiMua = userId,
                NgayMua = DateTime.Now,
                TrangThai = "Đang xử lý",
                TongTien = 0,
                GiamGia = 0,
            };

            _dbContext.DonHangs.Add(newOrder);
            await _dbContext.SaveChangesAsync();

            return newOrder.Id;
        }
    }
}
