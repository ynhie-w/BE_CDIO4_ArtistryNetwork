using CODE_CDIO4.Models;
using CODE_CDIO4.Repository;

namespace CODE_CDIO4.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateThongBaoAsync(int idNguoiNhan, string noiDung)
        {
            var thongBao = new ThongBao
            {
                Id_NguoiDung = idNguoiNhan,
                NoiDung = noiDung,
                DaDoc = false,
                NgayTao = DateTime.Now
            };

            _context.ThongBaos.Add(thongBao);
            await _context.SaveChangesAsync();
        }
    }
}
