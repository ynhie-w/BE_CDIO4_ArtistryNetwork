using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Helper;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

public class TaiKhoanService : ITaiKhoanService
{
    private readonly AppDbContext _context;
    public TaiKhoanService(AppDbContext context) => _context = context;
    //ĐĂNG NHẬP
    public async Task<DangNhapResponseDto?> DangNhap(DangNhapDto yeuCau)
    {
        var nguoiDung = await _context.NguoiDungs
            .Include(u => u.Quyen)
            .FirstOrDefaultAsync(u =>
                (u.Email == yeuCau.EmailSdt || u.Sdt == yeuCau.EmailSdt) && u.TrangThai);

        if (nguoiDung == null || !MatKhauHelper.KiemTraMatKhau(yeuCau.MatKhau, nguoiDung.MatKhau))
            return null;

        var token = JwtHelper.TaoToken(nguoiDung, nguoiDung.Quyen.Ten, 60);

        var nguoiDungDto = new CapNhatNguoiDungDto
        {
            Id = nguoiDung.Id,
            Ten = nguoiDung.Ten,
            Email = nguoiDung.Email,
            Sdt = nguoiDung.Sdt,
            AnhDaiDien = nguoiDung.AnhDaiDien,
            Id_Quyen = nguoiDung.Quyen.Id,
            TrangThai = nguoiDung.TrangThai,
            NgayTao = nguoiDung.NgayTao
        };

        return new DangNhapResponseDto { Token = token, NguoiDung = nguoiDungDto };
    }
//ĐĂNG XUẤT
    public Task<bool> DangXuat() => Task.FromResult(true);
    //ĐĂNG KÝ


    public async Task<int> DangKy(DangKyDto dto)
    {
        // 1️⃣ Kiểm tra thông tin trống
        if (string.IsNullOrWhiteSpace(dto.Ten))
            throw new ArgumentException("Tên không được để trống");
        if (string.IsNullOrWhiteSpace(dto.EmailSdt))
            throw new ArgumentException("Email hoặc số điện thoại không được để trống");
        if (string.IsNullOrWhiteSpace(dto.MatKhau) || string.IsNullOrWhiteSpace(dto.NhapLaiMatKhau))
            throw new ArgumentException("Mật khẩu không được để trống");

        // 2️⃣ Kiểm tra mật khẩu nhập lại
        if (dto.MatKhau != dto.NhapLaiMatKhau)
            throw new ArgumentException("Mật khẩu nhập lại không khớp");

        // 3️⃣ Kiểm tra mật khẩu mạnh (>=6 ký tự, có chữ + số)
        if (dto.MatKhau.Length < 6 || !Regex.IsMatch(dto.MatKhau, @"[A-Za-z]") || !Regex.IsMatch(dto.MatKhau, @"\d"))
            throw new ArgumentException("Mật khẩu phải ít nhất 6 ký tự, bao gồm chữ cái và số");

        // 4️⃣ Xác định Email/SĐT
        string email = dto.EmailSdt.Contains("@") ? dto.EmailSdt.Trim().ToLower() : "";
        string sdt = string.IsNullOrEmpty(email) ? dto.EmailSdt.Trim() : null;

        // 5️⃣ Validate Email
        if (!string.IsNullOrEmpty(email))
        {
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailRegex))
                throw new ArgumentException("Email không hợp lệ");
        }

        // 6️⃣ Validate SĐT
        if (!string.IsNullOrEmpty(sdt))
        {
            var sdtRegex = @"^\d{10}$";
            if (!Regex.IsMatch(sdt, sdtRegex))
                throw new ArgumentException("Số điện thoại phải đúng 10 chữ số");
        }

        // 7️⃣ Kiểm tra trùng dữ liệu
        if (await _context.NguoiDungs.AnyAsync(u => u.Ten == dto.Ten.Trim()))
            throw new InvalidOperationException("Tên đã tồn tại trong hệ thống");
        if (!string.IsNullOrEmpty(email) && await _context.NguoiDungs.AnyAsync(u => u.Email == email))
            throw new InvalidOperationException("Email đã tồn tại trong hệ thống");
        if (!string.IsNullOrEmpty(sdt) && await _context.NguoiDungs.AnyAsync(u => u.Sdt == sdt))
            throw new InvalidOperationException("Số điện thoại đã tồn tại trong hệ thống");

        // 8️⃣ Hash mật khẩu
        var hashedMatKhau = MatKhauHelper.HashTheoSQL(dto.MatKhau);

        // 9️⃣ Tạo entity mới
        var nguoiDung = new NguoiDung
        {
            Ten = dto.Ten.Trim(),
            Email = string.IsNullOrEmpty(email) ? null : email,
            Sdt = sdt,
            MatKhau = hashedMatKhau,
            AnhDaiDien = null,
            DiemThuong = 1,
            Id_CapDo = 2,
            Id_PhanQuyen = 1,
            NgayTao = DateTime.Now,
            TrangThai = true
        };

        _context.NguoiDungs.Add(nguoiDung);
        await _context.SaveChangesAsync();

        return nguoiDung.Id;
    }


public async Task<bool> DoiMatKhau(ClaimsPrincipal user, DoiMatKhauDto dto)
    {
        if (!int.TryParse(user.FindFirstValue("userId"), out var userId)) return false;
        var nguoiDung = await _context.NguoiDungs.FindAsync(userId);
        if (nguoiDung == null || !MatKhauHelper.KiemTraMatKhau(dto.MatKhauCu, nguoiDung.MatKhau)) return false;
        if (dto.MatKhauMoi != dto.MatKhauMoiNhapLai) return false;

        nguoiDung.MatKhau = MatKhauHelper.HashTheoSQL(dto.MatKhauMoi);
        _context.NguoiDungs.Update(nguoiDung);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> QuenMatKhau(QuenMatKhauDto dto)
    {
        var nguoiDung = await _context.NguoiDungs
            .FirstOrDefaultAsync(u => u.Email == dto.EmailSdt || u.Sdt == dto.EmailSdt);
        if (nguoiDung == null) return false;

        var token = Guid.NewGuid().ToString("N");
        var resetToken = new DatLaiMatKhauToken
        {
            Id_NguoiDung = nguoiDung.Id,
            Token = token,
            Han = DateTime.Now.AddMinutes(15)
        };

        _context.DatLaiMatKhauTokens.Add(resetToken);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Link reset mật khẩu: https://yourdomain.com/resetpassword?token={token}");
        return true;
    }

    public async Task<bool> DatLaiMatKhau(DatLaiMatKhauDto dto)
    {
        var reset = await _context.DatLaiMatKhauTokens
            .Include(r => r.NguoiDung)
            .FirstOrDefaultAsync(r => r.Token == dto.Token && r.Han > DateTime.Now);

        if (reset == null) return false;

        reset.NguoiDung.MatKhau = MatKhauHelper.HashTheoSQL(dto.MatKhauMoi);
        _context.NguoiDungs.Update(reset.NguoiDung);
        _context.DatLaiMatKhauTokens.Remove(reset);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<CapNhatNguoiDungDto?> LayThongTin(ClaimsPrincipal user)
    {
        var userIdStr = user.FindFirstValue("userId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return null;

        return await _context.NguoiDungs
            .Where(u => u.Id == userId)
            .Select(u => new CapNhatNguoiDungDto
            {
                Id = u.Id,
                Ten = u.Ten,
                Email = u.Email,
                Sdt = u.Sdt,
                AnhDaiDien = u.AnhDaiDien,
                Id_Quyen = u.Id_PhanQuyen,
                TrangThai = u.TrangThai,
                NgayTao =  u.NgayTao
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CapNhatEmail(ClaimsPrincipal user, string email)
    {
        if (string.IsNullOrEmpty(email)) return false;

        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailPattern)) return false;

        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return false;

        var nguoiDung = await _context.NguoiDungs.FindAsync(userId);
        if (nguoiDung == null) return false;

        nguoiDung.Email = email;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CapNhatSdt(ClaimsPrincipal user, string sdt)
    {
        if (string.IsNullOrEmpty(sdt)) return false;

        var sdtPattern = @"^\d{10}$";
        if (!Regex.IsMatch(sdt, sdtPattern)) return false;

        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return false;

        var nguoiDung = await _context.NguoiDungs.FindAsync(userId);
        if (nguoiDung == null) return false;

        nguoiDung.Sdt = sdt;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CapNhatAnhDaiDien(ClaimsPrincipal user, string anhDaiDien)
    {
        if (string.IsNullOrEmpty(anhDaiDien)) return false;

        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return false;

        var nguoiDung = await _context.NguoiDungs.FindAsync(userId);
        if (nguoiDung == null) return false;

        nguoiDung.AnhDaiDien = anhDaiDien;
        await _context.SaveChangesAsync();
        return true;
    }
}
