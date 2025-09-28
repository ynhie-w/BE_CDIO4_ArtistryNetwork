using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Helper;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDIO4_BE.Services
{
    public class QuanTriVienService : IQuanTriVienService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<QuanTriVienService> _logger;

        public QuanTriVienService(AppDbContext dbContext, ILogger<QuanTriVienService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // ===== NGƯỜI DÙNG =====
        public async Task<object> LayDanhSachNguoiDung(int trang, int soLuong) =>
            await _dbContext.NguoiDungs
                .Include(u => u.Quyen)
                .Select(u => new
                {
                    u.Ten,
                    LienHe = !string.IsNullOrEmpty(u.Email) ? u.Email : u.Sdt,
                    Quyen = u.Quyen != null ? u.Quyen.Ten : (u.Id_PhanQuyen == 2 ? "Admin" : "User"),
                    TrangThai = u.TrangThai ? "Hoạt động" : "Bị khóa",
                    NgayThamGia = u.NgayTao,
                    SoTacPham = _dbContext.TacPhams.Count(tp => tp.Id_NguoiTao == u.Id)
                })
                .OrderBy(x => x.Ten)
                .Skip((trang - 1) * soLuong)
                .Take(soLuong)
                .ToListAsync();

        public async Task<object?> LayThongTinNguoiDung(int id)
        {
            var user = await _dbContext.NguoiDungs.FindAsync(id);
            if (user == null) return null;

            return new
            {
                Id = user.Id,
                Ten = user.Ten,
                Email = user.Email,
                Sdt = user.Sdt,
                AnhDaiDien = user.AnhDaiDien,
                TrangThai = user.TrangThai,
                NgayThamGia = user.NgayTao
            };
        }
        public async Task<int> TaoNguoiDung(TaoNguoiDungDto yeuCau)
        {
            if (await _dbContext.NguoiDungs.AnyAsync(u => u.Email == yeuCau.Email))
                return 0;

            var nguoiDung = new NguoiDung
            {
                Ten = yeuCau.Ten,
                Email = yeuCau.Email,
                MatKhau = HashHelper.HashPassword(yeuCau.MatKhau),
                Id_PhanQuyen = 1,
                TrangThai = true
            };
            _dbContext.NguoiDungs.Add(nguoiDung);
            await _dbContext.SaveChangesAsync();
            return nguoiDung.Id;
        }

        public async Task<bool> CapNhatNguoiDung(int id, CapNhatNguoiDungDto yeuCau)
        {
            var user = await _dbContext.NguoiDungs.FindAsync(id);
            if (user == null) return false;
            user.Ten = yeuCau.Ten ?? user.Ten;
            user.Email = yeuCau.Email ?? user.Email;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> XoaNguoiDung(int id)
        {
            var user = await _dbContext.NguoiDungs.FindAsync(id);
            if (user == null) return false;
            _dbContext.NguoiDungs.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> KhoaTaiKhoan(int id, bool biKhoa)
        {
            var user = await _dbContext.NguoiDungs.FindAsync(id);
            if (user == null) return false;
            user.TrangThai = !biKhoa;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // ===== SẢN PHẨM =====
        public Task<List<TacPham>> LayDanhSachSanPham(int trang, int soLuong) =>
            _dbContext.TacPhams
                .OrderBy(p => p.Id)
                .Skip((trang - 1) * soLuong)
                .Take(soLuong)
                .ToListAsync();

        public Task<List<BoSuuTap>> LayDanhSachBoSuuTap(int trang, int soLuong) =>
            _dbContext.BoSuuTaps
                .OrderBy(b => b.NgayThem)
                .Skip((trang - 1) * soLuong)
                .Take(soLuong)
                .ToListAsync();

        public async Task<object> LayThongKeTongQuan() => new
        {
            TongNguoiDung = await _dbContext.NguoiDungs.CountAsync(),
            TongSanPham = await _dbContext.TacPhams.CountAsync(),
            TongDonHang = await _dbContext.DonHangs.CountAsync()
        };

        public Task<List<DonHang>> LayDanhSachDonHangTheoNgay(DateTime tuNgay, DateTime denNgay) =>
            _dbContext.DonHangs
                .Where(dh => dh.NgayMua >= tuNgay && dh.NgayMua <= denNgay)
                .ToListAsync();

        public async Task<object> ThongKeNguoiDung(DateTime tuNgay, DateTime denNgay) =>
            new { SoNguoiDung = await _dbContext.NguoiDungs.CountAsync(u => u.NgayTao >= tuNgay && u.NgayTao <= denNgay) };

        public async Task<object> LayTopSanPhamBanChay(int top) =>
            await _dbContext.DonHang_ChiTiets
                .GroupBy(d => d.Id_TacPham)
                .Select(g => new { Id = g.Key, SoDonHang = g.Count() })
                .OrderByDescending(x => x.SoDonHang)
                .Take(top)
                .ToListAsync();

        // ===== ĐƠN HÀNG =====
        public async Task<object> LayDanhSachDonHang(int trang, int soLuong, string trangThai)
        {
            var query = _dbContext.DonHangs
                .Include(d => d.DonHang_ChiTiets)
                    .ThenInclude(ct => ct.TacPham)
                .Include(d => d.NguoiMua)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            return await query
                .OrderByDescending(d => d.NgayMua)
                .Skip((trang - 1) * soLuong)
                .Take(soLuong)
                .Select(d => new
                {
                    MaHoaDon = d.Id,
                     KhachHang = d.NguoiMua.Ten ?? "N/A",
                    TacPham = string.Join(", ", d.DonHang_ChiTiets.Select(ct => ct.TacPham.Ten)),
                    SoTien = d.TongTien,
                    NgayMua = d.NgayMua,
                    TrangThai = d.TrangThai
                })
                .ToListAsync();
        }

        public async Task<DonHangDto?> LayChiTietDonHang(int id)
        {
            var dh = await _dbContext.DonHangs
                .Include(d => d.DonHang_ChiTiets)
                    .ThenInclude(ct => ct.TacPham)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dh == null) return null;

            return new DonHangDto
            {
                Id = dh.Id,
                NgayMua = dh.NgayMua,
                TrangThai = dh.TrangThai,
                TongTien = dh.TongTien,
                GiamGia = dh.GiamGia,
                ChiTiets = dh.DonHang_ChiTiets.Select(ct => new DonHangChiTietDto
                {
                    Id_TacPham = ct.Id_TacPham,
                    TenTacPham = ct.TacPham.Ten
                }).ToList()
            };
        }

        public async Task<bool> CapNhatTrangThaiDonHang(int id, CapNhatTrangThaiDonHangDto yeuCau)
        {
            var dh = await _dbContext.DonHangs.FindAsync(id);
            if (dh == null) return false;
            dh.TrangThai = yeuCau.TrangThai;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HuyDonHang(int id, string lyDoHuy)
        {
            var dh = await _dbContext.DonHangs.FindAsync(id);
            if (dh == null) return false;
            dh.TrangThai = "Đã hủy";
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
