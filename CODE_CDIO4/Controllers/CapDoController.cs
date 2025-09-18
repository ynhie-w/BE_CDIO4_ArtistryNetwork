using CODE_CDIO4.Models;
using CODE_CDIO4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CODE_CDIO4.DTOs;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CapDoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CapDoController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== LẤY DANH SÁCH ====================
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy tất cả cấp độ", Description = "Trả về danh sách tất cả các cấp độ (chỉ Id, ĐiểmTừ, ĐiểmĐến, Tên).")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công.", typeof(IEnumerable<CapDoDTO>))]
        public async Task<ActionResult<IEnumerable<CapDoDTO>>> GetAll()
        {
            var capDos = await _context.CapDos
                                       .AsNoTracking()
                                       .Select(c => new CapDoDTO
                                       {
                                           Id = c.Id,
                                           DiemTu = c.DiemTu,
                                           DiemDen = c.DiemDen,
                                           Ten = c.Ten
                                       })
                                       .ToListAsync();
            return Ok(capDos);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy cấp độ theo ID", Description = "Trả về thông tin chi tiết của một cấp độ.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công.", typeof(CapDoDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy cấp độ.")]
        public async Task<ActionResult<CapDoDTO>> GetById(int id)
        {
            var capDo = await _context.CapDos
                                      .AsNoTracking()
                                      .Where(c => c.Id == id)
                                      .Select(c => new CapDoDTO
                                      {
                                          Id = c.Id,
                                          DiemTu = c.DiemTu,
                                          DiemDen = c.DiemDen,
                                          Ten = c.Ten
                                      })
                                      .FirstOrDefaultAsync();
            if (capDo == null)
                return NotFound("Không tìm thấy cấp độ.");
            return Ok(capDo);
        }

        // ==================== TẠO MỚI ====================
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo cấp độ mới", Description = "Thêm một cấp độ mới vào cơ sở dữ liệu.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Tạo thành công.", typeof(CapDoDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ.")]
        public async Task<ActionResult<CapDoDTO>> Create([FromBody] CapDoInsertDTO dto)
        {
            if (dto.DiemTu >= dto.DiemDen)
                return BadRequest("Điểm 'Từ' phải nhỏ hơn Điểm 'Đến'.");

            if (await _context.CapDos.AnyAsync(c => c.Ten == dto.Ten))
                return BadRequest("Tên cấp độ đã tồn tại.");

            if (await _context.CapDos.AnyAsync(c =>
                   (dto.DiemTu >= c.DiemTu && dto.DiemTu <= c.DiemDen) ||
                   (dto.DiemDen >= c.DiemTu && dto.DiemDen <= c.DiemDen) ||
                   (dto.DiemTu <= c.DiemTu && dto.DiemDen >= c.DiemDen)))
            {
                return BadRequest("Khoảng điểm này đã nằm trong hoặc chồng lấn với cấp độ khác.");
            }

            var capDo = new CapDo
            {
                Ten = dto.Ten,
                DiemTu = dto.DiemTu,
                DiemDen = dto.DiemDen
            };

            _context.CapDos.Add(capDo);
            await _context.SaveChangesAsync();

            var result = new CapDoDTO
            {
                Id = capDo.Id,
                Ten = capDo.Ten,
                DiemTu = capDo.DiemTu,
                DiemDen = capDo.DiemDen
            };

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // ==================== CẬP NHẬT ====================
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật cấp độ", Description = "Cập nhật thông tin một cấp độ.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Cập nhật thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy cấp độ.")]
        public async Task<IActionResult> Update(int id, [FromBody] CapDoUpdateDTO dto)
        {
            var capDo = await _context.CapDos.FindAsync(id);
            if (capDo == null)
                return NotFound("Không tìm thấy cấp độ.");

            if (dto.DiemTu >= dto.DiemDen)
                return BadRequest("Điểm 'Từ' phải nhỏ hơn Điểm 'Đến'.");

            if (await _context.CapDos.AnyAsync(c => c.Id != id && c.Ten == dto.Ten))
                return BadRequest("Tên cấp độ đã tồn tại.");

            if (await _context.CapDos.AnyAsync(c => c.Id != id &&
                   ((dto.DiemTu >= c.DiemTu && dto.DiemTu <= c.DiemDen) ||
                    (dto.DiemDen >= c.DiemTu && dto.DiemDen <= c.DiemDen) ||
                    (dto.DiemTu <= c.DiemTu && dto.DiemDen >= c.DiemDen))))
            {
                return BadRequest("Khoảng điểm này đã nằm trong hoặc chồng lấn với cấp độ khác.");
            }

            capDo.Ten = dto.Ten;
            capDo.DiemTu = dto.DiemTu;
            capDo.DiemDen = dto.DiemDen;

            _context.Entry(capDo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ==================== XÓA ====================
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa cấp độ", Description = "Xóa một cấp độ.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Xóa thành công.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy cấp độ.")]
        public async Task<IActionResult> Delete(int id)
        {
            var capDo = await _context.CapDos.FindAsync(id);
            if (capDo == null)
                return NotFound("Không tìm thấy cấp độ.");

            _context.CapDos.Remove(capDo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }


}
