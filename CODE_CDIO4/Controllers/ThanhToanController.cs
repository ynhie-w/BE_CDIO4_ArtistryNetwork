using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Models;
using Swashbuckle.AspNetCore.Annotations;
using CODE_CDIO4.Repository;
using CODE_CDIO4.DTOs;
using Microsoft.Data.SqlClient;
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

        private static readonly string[] AllowedPhuongThuc = { "momo", "vnpay", "paypal" };
        private static readonly string[] AllowedTrangThai = { "Chờ xử lý", "Thành công", "Thất bại", "Hoàn tiền" };

        [HttpPost("Upsert")]
        [SwaggerOperation(Summary = "Thêm hoặc cập nhật thanh toán (gọi proc upsert_ThanhToan)")]
        public async Task<IActionResult> UpsertThanhToan([FromBody] ThanhToanDTO dto)
        {
            if (!AllowedPhuongThuc.Contains(dto.PhuongThuc.ToLower()))
                return BadRequest("Phương thức không hợp lệ (momo, vnpay, paypal).");

            if (!AllowedTrangThai.Contains(dto.TrangThai ?? "Chờ xử lý"))
                return BadRequest("Trạng thái không hợp lệ.");

            var parameters = new[]
            {
                new SqlParameter("@id_donhang", dto.Id_DonHang),
                new SqlParameter("@phuongthuc", dto.PhuongThuc),
                new SqlParameter("@trangthai", dto.TrangThai ?? "Chờ xử lý")
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC upsert_ThanhToan @id_donhang, @phuongthuc, @trangthai", parameters);
                return Ok(new { message = "Thêm/cập nhật thanh toán thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi gọi stored procedure", error = ex.Message });
            }
        }

        // GET: api/ThanhToan/DonHang/5
        [HttpGet("DonHang/{idDonHang}")]
        [SwaggerOperation(
            Summary = "Lấy tất cả giao dịch thanh toán theo đơn hàng",
            Description = "Truy xuất danh sách tất cả các giao dịch thanh toán của một đơn hàng dựa trên ID đơn hàng."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Truy xuất thành công.", typeof(IEnumerable<ThanhToan>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy giao dịch thanh toán nào.")]
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
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Lấy thông tin chi tiết một giao dịch thanh toán",
            Description = "Truy xuất thông tin chi tiết của một giao dịch thanh toán dựa trên ID."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Truy xuất thành công.", typeof(ThanhToan))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy giao dịch thanh toán.")]
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
        [HttpPost]
        [SwaggerOperation(
            Summary = "Tạo giao dịch thanh toán mới",
            Description = "Tạo một giao dịch thanh toán mới cho một đơn hàng đã tồn tại."
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Tạo thành công.", typeof(ThanhToan))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "ID đơn hàng hoặc dữ liệu không hợp lệ.")]
        public async Task<ActionResult<ThanhToan>> PostThanhToan(ThanhToan thanhToan)
        {
            var donHangExists = await _context.DonHangs.AnyAsync(dh => dh.Id == thanhToan.Id_DonHang);
            if (!donHangExists)
            {
                return BadRequest("ID đơn hàng không hợp lệ.");
            }

            if (!AllowedPhuongThuc.Contains(thanhToan.PhuongThuc.ToLower()))
            {
                return BadRequest("Phương thức thanh toán không hợp lệ. Chỉ chấp nhận: momo, vnpay, paypal.");
            }

            if (string.IsNullOrEmpty(thanhToan.TrangThai))
                thanhToan.TrangThai = "Chờ xử lý";

            if (!AllowedTrangThai.Contains(thanhToan.TrangThai))
            {
                return BadRequest("Trạng thái không hợp lệ. Chỉ chấp nhận: Chờ xử lý, Thành công, Thất bại, Hoàn tiền.");
            }

            thanhToan.NgayTT = DateTime.Now;

            _context.ThanhToans.Add(thanhToan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThanhToanById), new { id = thanhToan.Id }, thanhToan);
        }

        // PUT: api/ThanhToan/5/TrangThai
        [HttpPut("{id}/TrangThai")]
        [SwaggerOperation(
            Summary = "Cập nhật trạng thái giao dịch thanh toán",
            Description = "Cập nhật trạng thái của một giao dịch thanh toán dựa trên ID."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Cập nhật thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Trạng thái không hợp lệ.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy giao dịch thanh toán.")]
        public async Task<IActionResult> UpdateTrangThaiThanhToan(int id, [FromBody] string trangThai)
        {
            var thanhToan = await _context.ThanhToans.FindAsync(id);
            if (thanhToan == null)
            {
                return NotFound("Không tìm thấy giao dịch để cập nhật.");
            }

            if (!AllowedTrangThai.Contains(trangThai))
            {
                return BadRequest("Trạng thái không hợp lệ. Chỉ chấp nhận: Chờ xử lý, Thành công, Thất bại, Hoàn tiền.");
            }

            thanhToan.TrangThai = trangThai;
            _context.Entry(thanhToan).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ThanhToan/5
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Xóa giao dịch thanh toán",
            Description = "Xóa một giao dịch thanh toán khỏi cơ sở dữ liệu dựa trên ID."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Đã xóa giao dịch thanh toán thành công.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy giao dịch thanh toán để xóa.")]
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
