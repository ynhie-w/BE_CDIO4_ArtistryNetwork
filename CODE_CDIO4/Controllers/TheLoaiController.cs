using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CODE_CDIO4.Models;
using Swashbuckle.AspNetCore.Annotations;
using CODE_CDIO4.Repository;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheLoaiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TheLoaiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TheLoai
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lấy danh sách thể loại",
            Description = "Truy xuất tất cả thể loại có trong cơ sở dữ liệu."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Truy xuất thành công.", typeof(IEnumerable<TheLoai>))]
        public async Task<ActionResult<IEnumerable<TheLoai>>> GetTheLoais()
        {
            var theLoais = await _context.TheLoais.ToListAsync();
            return Ok(theLoais);
        }

        // GET: api/TheLoai/5
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Lấy thông tin chi tiết thể loại",
            Description = "Truy xuất thông tin chi tiết của một thể loại dựa trên ID."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Truy xuất thành công.", typeof(TheLoai))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy thể loại.")]
        public async Task<ActionResult<TheLoai>> GetTheLoai(int id)
        {
            var theLoai = await _context.TheLoais.FindAsync(id);

            if (theLoai == null)
            {
                return NotFound("Không tìm thấy thể loại này.");
            }

            return Ok(theLoai);
        }

        // POST: api/TheLoai
        [HttpPost]
        [SwaggerOperation(
            Summary = "Tạo thể loại mới",
            Description = "Thêm một thể loại mới vào cơ sở dữ liệu. Tránh trùng lặp tên thể loại."
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Tạo thành công.", typeof(TheLoai))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Thể loại đã tồn tại.")]
        public async Task<ActionResult<TheLoai>> PostTheLoai(TheLoai theLoai)
        {
            var existingTheLoai = await _context.TheLoais
                .FirstOrDefaultAsync(t => t.Ten.ToLower() == theLoai.Ten.ToLower());

            if (existingTheLoai != null)
            {
                return Conflict("Thể loại này đã tồn tại.");
            }

            // Nếu không truyền trạng thái -> mặc định true
            if (theLoai.TrangThai == false)
                theLoai.TrangThai = true;

            _context.TheLoais.Add(theLoai);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTheLoai), new { id = theLoai.Id }, theLoai);
        }

        // PUT: api/TheLoai/5
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Cập nhật thể loại",
            Description = "Cập nhật thông tin của một thể loại dựa trên ID."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Cập nhật thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "ID không khớp.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy thể loại.")]
        public async Task<IActionResult> PutTheLoai(int id, TheLoai theLoai)
        {
            if (id != theLoai.Id)
            {
                return BadRequest("ID trong URL không khớp với ID của đối tượng.");
            }

            _context.Entry(theLoai).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TheLoaiExists(id))
                {
                    return NotFound("Không tìm thấy thể loại để cập nhật.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/TheLoai/5
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Xóa thể loại",
            Description = "Xóa một thể loại khỏi cơ sở dữ liệu dựa trên ID."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Đã xóa thành công.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy thể loại.")]
        public async Task<IActionResult> DeleteTheLoai(int id)
        {
            var theLoai = await _context.TheLoais.FindAsync(id);
            if (theLoai == null)
            {
                return NotFound("Không tìm thấy thể loại để xóa.");
            }

            _context.TheLoais.Remove(theLoai);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã xóa thể loại '{theLoai.Ten}' thành công." });
        }

        private bool TheLoaiExists(int id)
        {
            return _context.TheLoais.Any(e => e.Id == id);
        }
    }
}
