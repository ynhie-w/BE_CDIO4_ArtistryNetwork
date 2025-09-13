using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThanhToanController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ThanhToanController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ThanhToan/DonHang/5
        // Lấy tất cả các giao dịch thanh toán của một đơn hàng cụ thể
        [HttpGet("DonHang/{idDonHang}")]
        public async Task<ActionResult<IEnumerable<ThanhToan>>> GetThanhToanByDonHang(int idDonHang)
        {
            var thanhToans = await _context.ThanhToans
                                           .Where(tt => tt.Id_DonHang == idDonHang)
                                           .OrderByDescending(tt => tt.NgayTT)
                                           .ToListAsync();

            if (!thanhToans.Any())
            {
                return NotFound("Không tìm thấy giao dịch thanh toán nào cho đơn hàng này.");
            }

            return Ok(thanhToans);
        }

        // GET: api/ThanhToan/5
        // Lấy thông tin chi tiết của một giao dịch thanh toán
        [HttpGet("{id}")]
        public async Task<ActionResult<ThanhToan>> GetThanhToanById(int id)
        {
            var thanhToan = await _context.ThanhToans.FindAsync(id);

            if (thanhToan == null)
            {
                return NotFound("Không tìm thấy giao dịch thanh toán này.");
            }

            return Ok(thanhToan);
        }

        // POST: api/ThanhToan
        // Tạo một giao dịch thanh toán mới
        [HttpPost]
        public async Task<ActionResult<ThanhToan>> PostThanhToan(ThanhToan thanhToan)
        {
            // Có thể thêm logic kiểm tra ở đây, ví dụ: kiểm tra xem DonHang có tồn tại không
            var donHangExists = await _context.DonHangs.AnyAsync(dh => dh.Id == thanhToan.Id_DonHang);
            if (!donHangExists)
            {
                return BadRequest("ID đơn hàng không hợp lệ.");
            }

            _context.ThanhToans.Add(thanhToan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThanhToanById), new { id = thanhToan.Id }, thanhToan);
        }

        // PUT: api/ThanhToan/5/TrangThai
        // Cập nhật trạng thái của một giao dịch thanh toán
        [HttpPut("{id}/TrangThai")]
        public async Task<IActionResult> UpdateTrangThaiThanhToan(int id, [FromBody] string trangThai)
        {
            var thanhToan = await _context.ThanhToans.FindAsync(id);
            if (thanhToan == null)
            {
                return NotFound("Không tìm thấy giao dịch để cập nhật.");
            }

            thanhToan.TrangThai = trangThai;
            _context.Entry(thanhToan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThanhToanExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThanhToan(int id)
        {
            var thanhToan = await _context.ThanhToans.FindAsync(id);
            if (thanhToan == null)
            {
                return NotFound("Không tìm thấy giao dịch để xóa.");
            }

            _context.ThanhToans.Remove(thanhToan);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa giao dịch thanh toán thành công." });
        }

        private bool ThanhToanExists(int id)
        {
            return _context.ThanhToans.Any(e => e.Id == id);
        }
    }
}
