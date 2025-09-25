using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Helper;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDIO4_BE.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AdminService> _logger;
        // TODO: Inject DbContext, mapper, etc.
        public AdminService(AppDbContext dbContext, ILogger<AdminService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<string> DangNhapAdmin(DangNhapDto yeuCau)
        {
            // 1. Lấy người dùng theo email
            var user = await _dbContext.NguoiDungs
                .FirstOrDefaultAsync(u => u.Email == yeuCau.EmailSdt);

            if (user == null)
            {
                _logger.LogInformation("Không tìm thấy người dùng với email: {Email}", yeuCau.EmailSdt);
                return null; // trả về null nếu không tìm thấy user
            }

            // 2. Hash mật khẩu nhập vào
            var hashedInput = HashHelper.HashPassword(yeuCau.MatKhau);

            // Debug: hiển thị hash nhập vào và hash lưu trong DB
            _logger.LogInformation("Hash nhập vào: {Hash}", HashHelper.HashPasswordHex(yeuCau.MatKhau));
            _logger.LogInformation("Hash DB: {DbHash}", Convert.ToHexString(user.MatKhau));

            // 3. So sánh hash
            if (!user.MatKhau.SequenceEqual(hashedInput))
            {
                _logger.LogInformation("Mật khẩu không khớp");
                return null; // sai mật khẩu
            }

            if (user.Id_PhanQuyen != 2)
            {
                _logger.LogInformation("Người dùng không phải admin: Id_PhanQuyen={Id}", user.Id_PhanQuyen);
                return null; // không phải admin
            }

            // 5. Tạo JWT
            var token = JwtHelper.TaoToken(user.Id,user.Ten, "Admin", 60); // token 60 phút
            _logger.LogInformation("Đăng nhập thành công, token đã được tạo cho user Id={Id}", user.Id);

            return token;
        }

        public async Task<List<NguoiDung>> LayDanhSachNguoiDung(int trang, int soLuong)
        {
            return await _dbContext.NguoiDungs
                .OrderBy(u => u.Id)
                .Skip((trang - 1) * soLuong)
                .Take(soLuong)
                .ToListAsync();
        }

        public async Task<NguoiDung> LayThongTinNguoiDung(int id)
        {
            return await _dbContext.NguoiDungs.FindAsync(id);
        }

        public async Task<int> TaoNguoiDung(TaoNguoiDungDto yeuCau)
        {
            var exists = await _dbContext.NguoiDungs.AnyAsync(u => u.Email == yeuCau.Email);
            if (exists) return 0;

            var nguoiDung = new NguoiDung
            {
                Ten = yeuCau.Ten,
                Email = yeuCau.Email,
                MatKhau = HashHelper.HashPassword(yeuCau.MatKhau),
                Id_PhanQuyen = 1, // User bình thường
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

            user.TrangThai = !biKhoa; // true = mở, false = khóa
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // ===== SẢN PHẨM =====
        public async Task<List<TacPham>> LayDanhSachSanPham(int trang, int soLuong)
        {
            return await _dbContext.TacPhams
                .OrderBy(p => p.Id)
                .Skip((trang - 1) * soLuong)
                .Take(soLuong)
                .ToListAsync();
        }

        public async Task<List<BoSuuTap>> LayDanhSachBoSuuTap(int trang, int soLuong)
        {
            return await _dbContext.BoSuuTaps
                .OrderBy(b => b.NgayThem)
                .Skip((trang - 1) * soLuong)
                .Take(soLuong)
                .ToListAsync();
        }

        public async Task<object> LayThongKeTongQuan()
        {
            var tongNguoiDung = await _dbContext.NguoiDungs.CountAsync();
            var tongSanPham = await _dbContext.TacPhams.CountAsync();
            var tongDonHang = await _dbContext.DonHangs.CountAsync();

            return new
            {
                TongNguoiDung = tongNguoiDung,
                TongSanPham = tongSanPham,
                TongDonHang = tongDonHang
            };
        }

        public async Task<List<DonHang>> LayDanhSachDonHangTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            return await _dbContext.DonHangs
                .Where(dh => dh.NgayMua >= tuNgay && dh.NgayMua <= denNgay)
                .ToListAsync();
        }


        public async Task<object> ThongKeNguoiDung(DateTime tuNgay, DateTime denNgay)
        {
            var soNguoiDung = await _dbContext.NguoiDungs
                .Where(u => u.NgayTao >= tuNgay && u.NgayTao <= denNgay)
                .CountAsync();

            return new { SoNguoiDung = soNguoiDung };
        }

        public async Task<object> LayTopSanPhamBanChay(int top)
        {
            var topSp = await _dbContext.DonHang_ChiTiets
            .GroupBy(d => d.Id_TacPham)
            .Select(g => new { Id = g.Key, SoDonHang = g.Count() }) // count dòng là số đơn
            .OrderByDescending(x => x.SoDonHang)
            .Take(top)
            .ToListAsync();

            return topSp;

        }

        // ===== ĐƠN HÀNG =====
        public async Task<List<DonHang>> LayDanhSachDonHang(int trang, int soLuong, string trangThai)
        {
            var query = _dbContext.DonHangs.AsQueryable();
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            return await query
                .OrderBy(d => d.Id)
                .Skip((trang - 1) * soLuong)
                .Take(soLuong)
                .ToListAsync();
        }

        public async Task<DonHangDto?> LayChiTietDonHang(int id)
        {
            var donHang = await _dbContext.DonHangs
                .Include(d => d.DonHang_ChiTiets)
                .ThenInclude(ct => ct.TacPham)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang == null) return null;

            return new DonHangDto
            {
                Id = donHang.Id,
                NgayMua = donHang.NgayMua,
                TrangThai = donHang.TrangThai,
                TongTien = donHang.TongTien,
                GiamGia = donHang.GiamGia,
                ChiTiets = donHang.DonHang_ChiTiets.Select(ct => new DonHangChiTietDto
                {
                    Id_TacPham = ct.Id_TacPham,
                    TenTacPham = ct.TacPham.Ten,
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
            // dh.LyDoHuy = lyDoHuy; // nếu có cột lưu
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
