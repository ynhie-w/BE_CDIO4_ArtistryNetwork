using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonHangController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DonHangController(AppDbContext context)
        {
            _context = context;
        }

        // ================= DTO =================
        public class DonHangChiTietDTO
        {
            public int Id_TacPham { get; set; }
            public string TenTacPham { get; set; } = "";
            public decimal Gia { get; set; }
        }

        public class DonHangDTO
        {
            public int Id { get; set; }
            public DateTime NgayMua { get; set; }
            public string TrangThai { get; set; } = "";
            public decimal TongTien { get; set; }
            public List<DonHangChiTietDTO> DonHang_ChiTiets { get; set; } = new();
        }

        // ================== API ==================
        // GET: api/DonHang // lấy tất cả đơn hàng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonHangDTO>>> GetAllDonHangs()
        {
            var donHangs = await _context.DonHangs
                .Include(dh => dh.DonHang_ChiTiets)
                    .ThenInclude(ct => ct.TacPham)
                .OrderByDescending(dh => dh.NgayMua)
                .ToListAsync();

            var dtos = donHangs.Select(dh => new DonHangDTO
            {
                Id = dh.Id,
                NgayMua = dh.NgayMua,
                TrangThai = dh.TrangThai,
                TongTien = dh.DonHang_ChiTiets.Sum(ct => ct.TacPham.Gia),
                DonHang_ChiTiets = dh.DonHang_ChiTiets.Select(ct => new DonHangChiTietDTO
                {
                    Id_TacPham = ct.Id_TacPham,
                    TenTacPham = ct.TacPham.Ten,
                    Gia = ct.TacPham.Gia
                }).ToList()
            }).ToList();

            return Ok(dtos);
        }

        // GET: api/DonHang/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DonHangDTO>> GetDonHang(int id)
        {
            var donHang = await _context.DonHangs
                .Include(dh => dh.DonHang_ChiTiets)
                    .ThenInclude(ct => ct.TacPham)
                .FirstOrDefaultAsync(dh => dh.Id == id);

            if (donHang == null)
                return NotFound("Không tìm thấy đơn hàng.");

            var dto = new DonHangDTO
            {
                Id = donHang.Id,
                NgayMua = donHang.NgayMua,
                TrangThai = donHang.TrangThai,
                TongTien = donHang.DonHang_ChiTiets.Sum(ct => ct.TacPham.Gia),
                DonHang_ChiTiets = donHang.DonHang_ChiTiets.Select(ct => new DonHangChiTietDTO
                {
                    Id_TacPham = ct.Id_TacPham,
                    TenTacPham = ct.TacPham.Ten,
                    Gia = ct.TacPham.Gia
                }).ToList()
            };

            return Ok(dto);
        }

        // GET: api/DonHang/NguoiMua/5
        [HttpGet("NguoiMua/{idNguoiMua}")]
        public async Task<ActionResult<IEnumerable<DonHangDTO>>> GetDonHangsByNguoiMua(int idNguoiMua)
        {
            var donHangs = await _context.DonHangs
                .Include(dh => dh.DonHang_ChiTiets)
                    .ThenInclude(ct => ct.TacPham)
                .Where(dh => dh.Id_NguoiMua == idNguoiMua)
                .OrderByDescending(dh => dh.NgayMua)
                .ToListAsync();

            if (!donHangs.Any())
                return NotFound("Người dùng này chưa có đơn hàng nào.");

            var dtos = donHangs.Select(dh => new DonHangDTO
            {
                Id = dh.Id,
                NgayMua = dh.NgayMua,
                TrangThai = dh.TrangThai,
                TongTien = dh.DonHang_ChiTiets.Sum(ct => ct.TacPham.Gia),
                DonHang_ChiTiets = dh.DonHang_ChiTiets.Select(ct => new DonHangChiTietDTO
                {
                    Id_TacPham = ct.Id_TacPham,
                    TenTacPham = ct.TacPham.Ten,
                    Gia = ct.TacPham.Gia
                }).ToList()
            }).ToList();

            return Ok(dtos);
        }

        // POST: api/DonHang
        [HttpPost]
        public async Task<ActionResult<DonHangDTO>> CreateDonHang(DonHang donHang)
        {
            donHang.NgayMua = DateTime.Now;
            donHang.TrangThai = "choxuly";

            // Tính tổng tiền dựa trên các tác phẩm
            donHang.TongTien = donHang.DonHang_ChiTiets.Sum(ct =>
                _context.TacPhams.FirstOrDefault(tp => tp.Id == ct.Id_TacPham)!.Gia
            );

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            // Trả về DTO
            var dto = await _context.DonHangs
                .Include(dh => dh.DonHang_ChiTiets)
                    .ThenInclude(ct => ct.TacPham)
                .Where(dh => dh.Id == donHang.Id)
                .Select(dh => new DonHangDTO
                {
                    Id = dh.Id,
                    NgayMua = dh.NgayMua,
                    TrangThai = dh.TrangThai,
                    TongTien = dh.DonHang_ChiTiets.Sum(ct => ct.TacPham.Gia),
                    DonHang_ChiTiets = dh.DonHang_ChiTiets.Select(ct => new DonHangChiTietDTO
                    {
                        Id_TacPham = ct.Id_TacPham,
                        TenTacPham = ct.TacPham.Ten,
                        Gia = ct.TacPham.Gia
                    }).ToList()
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetDonHang), new { id = dto.Id }, dto);
        }

        // PUT: api/DonHang/5/TrangThai
        [HttpPut("{id}/TrangThai")]
        public async Task<IActionResult> UpdateTrangThai(int id, [FromBody] string trangThai)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
                return NotFound("Không tìm thấy đơn hàng để cập nhật.");

            donHang.TrangThai = trangThai;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/DonHang/5/ChiTiet/10
        [HttpDelete("{id}/ChiTiet/{idTacPham}")]
        public async Task<IActionResult> DeleteChiTiet(int id, int idTacPham)
        {
            var chiTiet = await _context.DonHang_ChiTiets
                .FirstOrDefaultAsync(ct => ct.Id_DonHang == id && ct.Id_TacPham == idTacPham);

            if (chiTiet == null)
                return NotFound("Không tìm thấy chi tiết đơn hàng.");

            _context.DonHang_ChiTiets.Remove(chiTiet);

            // Cập nhật lại tổng tiền
            var donHang = await _context.DonHangs
                .Include(dh => dh.DonHang_ChiTiets)
                .ThenInclude(ct => ct.TacPham)
                .FirstOrDefaultAsync(dh => dh.Id == id);

            if (donHang != null)
            {
                donHang.TongTien = donHang.DonHang_ChiTiets.Sum(ct => ct.TacPham.Gia);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
