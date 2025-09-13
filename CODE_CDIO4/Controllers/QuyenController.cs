using CODE_CDIO4.Models;
using CODE_CDIO4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuyenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuyenController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== LẤY DANH SÁCH ====================

        // GET: api/Quyen
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy tất cả các quyền", Description = "Trả về danh sách tất cả các quyền có trong hệ thống.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công, trả về danh sách quyền.", typeof(IEnumerable<Quyen>))]
        public async Task<ActionResult<IEnumerable<Quyen>>> GetAll()
        {
            return await _context.Quyens.ToListAsync();
        }

        // GET: api/Quyen/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy quyền theo ID", Description = "Trả về thông tin chi tiết của một quyền dựa trên ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công, trả về quyền cụ thể.", typeof(Quyen))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy quyền.")]
        public async Task<ActionResult<Quyen>> GetById(int id)
        {
            var quyen = await _context.Quyens.FindAsync(id);

            if (quyen == null)
            {
                return NotFound("Không tìm thấy quyền.");
            }

            return Ok(quyen);
        }

        // ==================== TẠO MỚI ====================

        // POST: api/Quyen
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo quyền mới", Description = "Thêm một quyền mới vào cơ sở dữ liệu.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Tạo thành công, trả về quyền đã tạo.", typeof(Quyen))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu đầu vào không hợp lệ.")]
        public async Task<ActionResult<Quyen>> Create([FromBody] Quyen quyen)
        {
            _context.Quyens.Add(quyen);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = quyen.Id }, quyen);
        }

        // ==================== CẬP NHẬT ====================

        // PUT: api/Quyen/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật quyền", Description = "Cập nhật thông tin của một quyền đã có dựa trên ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Cập nhật thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "ID không khớp hoặc dữ liệu không hợp lệ.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy quyền để cập nhật.")]
        public async Task<IActionResult> Update(int id, [FromBody] Quyen quyen)
        {
            if (id != quyen.Id)
            {
                return BadRequest("ID không khớp.");
            }

            _context.Entry(quyen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuyenExists(id))
                {
                    return NotFound("Không tìm thấy quyền để cập nhật.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // ==================== XÓA ====================

        // DELETE: api/Quyen/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa quyền", Description = "Xóa một quyền khỏi cơ sở dữ liệu dựa trên ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Xóa thành công.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy quyền để xóa.")]
        public async Task<IActionResult> Delete(int id)
        {
            var quyen = await _context.Quyens.FindAsync(id);
            if (quyen == null)
            {
                return NotFound("Không tìm thấy quyền để xóa.");
            }

            _context.Quyens.Remove(quyen);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ==================== HỖ TRỢ ====================

        private bool QuyenExists(int id)
        {
            return _context.Quyens.Any(e => e.Id == id);
        }
    }
}