using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Helper;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class TaiKhoanService : ITaiKhoanService
{
    private readonly AppDbContext _context;

    public TaiKhoanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> DangNhap(DangNhapDto yeuCau)
    {
        var nguoiDung = await _context.NguoiDungs
            .Include(u => u.Quyen)
            .FirstOrDefaultAsync(u =>
                (u.Email == yeuCau.EmailSdt || u.Sdt == yeuCau.EmailSdt)
                && u.TrangThai
            );

        if (nguoiDung == null) return null;

        if (!MatKhauHelper.KiemTraMatKhau(yeuCau.MatKhau, nguoiDung.MatKhau))
            return null;

        // ✅ Gọi TaoToken mới
        return JwtHelper.TaoToken(nguoiDung, nguoiDung.Quyen.Ten, 60);
    }


    public Task<bool> DangXuat()
    {
        // Nếu dùng JWT, client chỉ cần xoá token phía client
        return Task.FromResult(true);
    }

    public async Task<int> DangKy(DangKyDto dto)
    {

        if (string.IsNullOrWhiteSpace(dto.MatKhau))
            throw new InvalidOperationException("Mật khẩu không được trống");
        if (string.IsNullOrWhiteSpace(dto.EmailSdt))
            throw new InvalidOperationException("Số điện thoại hoặc email không được trống");

        // Kiểm tra trùng tên, email hoặc số điện thoại
        bool daTonTai = await _context.NguoiDungs.AnyAsync(u =>
            u.Ten == dto.Ten ||
            (!string.IsNullOrEmpty(u.Email) && u.Email == dto.EmailSdt) ||
            (!string.IsNullOrEmpty(u.Sdt) && u.Sdt == dto.EmailSdt)
        );

        if (daTonTai)
            throw new InvalidOperationException("Tên, email hoặc số điện thoại đã tồn tại trong hệ thống");

        // Hash mật khẩu
        var hashedMatKhau = MatKhauHelper.HashTheoSQL(dto.MatKhau);

        // Gán Email hoặc SĐT
        string email = null;
        string sdt = null;

        if (dto.EmailSdt.Contains("@"))
            email = dto.EmailSdt.Trim();
        else
            sdt = dto.EmailSdt.Trim();

        var nguoiDung = new NguoiDung
        {
            Ten = dto.Ten.Trim(),
            Email = email,
            Sdt = sdt,
            MatKhau = hashedMatKhau,
            AnhDaiDien = null,
            DiemThuong = 0,
            Id_CapDo = 1,
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
        // Lấy Id người dùng từ token JWT
        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return false;

        var nguoiDung = await _context.NguoiDungs.FindAsync(userId);
        if (nguoiDung == null) return false;

        // Kiểm tra mật khẩu cũ
        if (!MatKhauHelper.KiemTraMatKhau(dto.MatKhauCu, nguoiDung.MatKhau))
            return false;

        // Cập nhật mật khẩu mới (đã hash)
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

        // Tạo token reset
        var token = Guid.NewGuid().ToString("N");
        var expire = DateTime.Now.AddMinutes(15);

        var resetToken = new DatLaiMatKhauToken
        {
            Id_NguoiDung = nguoiDung.Id,
            Token = token,
            Han = expire
        };

        _context.DatLaiMatKhauTokens.Add(resetToken);
        await _context.SaveChangesAsync();

        // TODO: Gửi email/SMS với link reset
        var linkReset = $"https://yourdomain.com/resetpassword?token={token}";
        Console.WriteLine($"Link reset mật khẩu: {linkReset}"); // test console, sau này gửi email thực

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

        // Xóa token sau khi dùng
        _context.DatLaiMatKhauTokens.Remove(reset);

        await _context.SaveChangesAsync();
        return true;
    }

}
