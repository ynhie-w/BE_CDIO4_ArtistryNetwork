using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Repository;
using CODE_CDIO4.Models;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DuAnCongDongController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DuAnCongDongController(AppDbContext context)
        {
            _context = context;
        }

        // =================== DỰ ÁN ===================

        // GET: api/DuAnCongDong
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DuAnCongDong>>> GetDuAnCongDongs()
        {
            var duAn = await _context.DuAnCongDongs
                                     .Include(da => da.QuanLy)
                                     .ToListAsync();

            return Ok(duAn);
        }

        // GET: api/DuAnCongDong/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DuAnCongDong>> GetDuAnCongDong(int id)
        {
            var duAn = await _context.DuAnCongDongs
                                     .Include(da => da.QuanLy)
                                     .Include(da => da.DuAn_TacPhams)
                                        .ThenInclude(datp => datp.TacPham)
                                     .Include(da => da.ThamGiaDuAns) // ✅ lấy cả thành viên tham gia
                                        .ThenInclude(tg => tg.NguoiDung)
                                     .FirstOrDefaultAsync(da => da.Id == id);

            if (duAn == null)
                return NotFound("Không tìm thấy dự án này.");

            return Ok(duAn);
        }

        // POST: api/DuAnCongDong
        [HttpPost]
        public async Task<ActionResult<DuAnCongDong>> PostDuAnCongDong(DuAnCongDong duAn)
        {
            _context.DuAnCongDongs.Add(duAn);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDuAnCongDong), new { id = duAn.Id }, duAn);
        }

        // PUT: api/DuAnCongDong/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDuAnCongDong(int id, DuAnCongDong duAn)
        {
            if (id != duAn.Id)
                return BadRequest("ID trong URL không khớp với ID của đối tượng.");

            _context.Entry(duAn).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DuAnCongDongExists(id))
                    return NotFound("Không tìm thấy dự án để cập nhật.");
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/DuAnCongDong/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDuAnCongDong(int id)
        {
            var duAn = await _context.DuAnCongDongs.FindAsync(id);
            if (duAn == null)
                return NotFound("Không tìm thấy dự án để xóa.");

            _context.DuAnCongDongs.Remove(duAn);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DuAnCongDongExists(int id)
        {
            return _context.DuAnCongDongs.Any(e => e.Id == id);
        }

        // =================== THAM GIA DỰ ÁN ===================

        // GET: api/DuAnCongDong/5/ThanhVien
        [HttpGet("{idDuAn}/ThanhVien")]
        public async Task<ActionResult<IEnumerable<ThamGiaDuAn>>> GetThanhVien(int idDuAn)
        {
            var thanhVien = await _context.ThamGiaDuAns
                                          .Where(tg => tg.Id_DuAn == idDuAn)
                                          .Include(tg => tg.NguoiDung)
                                          .ToListAsync();

            if (!thanhVien.Any())
                return NotFound("Chưa có ai tham gia dự án này.");

            return Ok(thanhVien);
        }

        // POST: api/DuAnCongDong/ThamGia
        [HttpPost("ThamGia")]
        public async Task<ActionResult<ThamGiaDuAn>> ThamGiaDuAn([FromBody] ThamGiaDuAn thamGia)
        {
            // kiểm tra trùng
            var exists = await _context.ThamGiaDuAns
                                       .AnyAsync(tg => tg.Id_DuAn == thamGia.Id_DuAn
                                                    && tg.Id_NguoiDung == thamGia.Id_NguoiDung);
            if (exists)
                return Conflict("Người dùng đã tham gia dự án này.");

            _context.ThamGiaDuAns.Add(thamGia);
            await _context.SaveChangesAsync();

            return Ok(thamGia);
        }

        // DELETE: api/DuAnCongDong/ThamGia/1/5
        [HttpDelete("ThamGia/{idDuAn}/{idNguoiDung}")]
        public async Task<IActionResult> HuyThamGia(int idDuAn, int idNguoiDung)
        {
            var thamGia = await _context.ThamGiaDuAns.FindAsync(idDuAn, idNguoiDung);
            if (thamGia == null)
                return NotFound("Không tìm thấy mối quan hệ tham gia.");

            _context.ThamGiaDuAns.Remove(thamGia);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
