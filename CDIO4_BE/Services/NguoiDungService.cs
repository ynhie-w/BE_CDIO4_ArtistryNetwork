using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class NguoiDungService : INguoiDungService 
{
    private readonly AppDbContext _context;

    public NguoiDungService(AppDbContext context)
    {
        _context = context;
    }

    // Lấy danh sách tất cả người dùng
    public async Task<IEnumerable<NguoiDung>> LayTatCaNguoiDung()
    {
        return await _context.NguoiDungs.ToListAsync();
    }
    // Cập nhật thông tin người dùng theo Id
    public async Task<bool> CapNhatNguoiDung(int idNguoiDung, CapNhatNguoiDungDto dto)
    {
        var nguoiDung = await _context.NguoiDungs.FindAsync(idNguoiDung);
        if (nguoiDung == null) return false;

        nguoiDung.Ten = dto.Ten ?? nguoiDung.Ten;
        // Có thể cập nhật thêm email, sdt,... nếu muốn
        await _context.SaveChangesAsync();
        return true;
    }

    // Xóa người dùng theo Id
    public async Task<bool> XoaNguoiDung(int idNguoiDung)
    {
        var nguoiDung = await _context.NguoiDungs.FindAsync(idNguoiDung);
        if (nguoiDung == null) return false;

        _context.NguoiDungs.Remove(nguoiDung);
        await _context.SaveChangesAsync();
        return true;
    }
}
