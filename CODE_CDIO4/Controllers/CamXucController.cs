using CODE_CDIO4.Models;
using CODE_CDIO4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CODE_CDIO4.DTOs;

namespace CODE_CDIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CamXucController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CamXucController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== LẤY DANH SÁCH ====================
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy tất cả cảm xúc", Description = "Trả về danh sách tất cả loại cảm xúc (chỉ Id và Tên).")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công.", typeof(IEnumerable<CamXucDTO>))]
        public async Task<ActionResult<IEnumerable<CamXucDTO>>> GetAll()
        {
            var camXucs = await _context.CamXucs
                                        .AsNoTracking()
                                        .Select(c => new CamXucDTO
                                        {
                                            Id = c.Id,
                                            Ten = c.Ten
                                        })
                                        .ToListAsync();
            return Ok(camXucs);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy cảm xúc theo ID", Description = "Trả về thông tin chi tiết của một cảm xúc.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công.", typeof(CamXucDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy cảm xúc.")]
        public async Task<ActionResult<CamXucDTO>> GetById(int id)
        {
            var camXuc = await _context.CamXucs
                                       .AsNoTracking()
                                       .Where(c => c.Id == id)
                                       .Select(c => new CamXucDTO
                                       {
                                           Id = c.Id,
                                           Ten = c.Ten
                                       })
                                       .FirstOrDefaultAsync();

            if (camXuc == null)
                return NotFound("Không tìm thấy loại cảm xúc.");

            return Ok(camXuc);
        }

        // ==================== HÀM CHUẨN HÓA ====================
        private string? NormalizeTen(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Xóa khoảng trắng thừa
            input = input.Trim();

            // Nếu không có ký tự chữ cái thì coi như không hợp lệ
            if (!input.Any(char.IsLetter))
                return null;

            // Viết hoa chữ cái đầu, còn lại giữ nguyên (kể cả tiếng Việt có dấu)
            string normalized = char.ToUpper(input[0]) + input.Substring(1).ToLower();

            return normalized;
        }

        // ==================== TẠO MỚI ====================
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo cảm xúc mới", Description = "Thêm một cảm xúc mới vào cơ sở dữ liệu.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Tạo thành công.", typeof(CamXucDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ hoặc tên bị trùng.")]
        public async Task<ActionResult<CamXucDTO>> Create([FromBody] CamXucInsertDTO dto)
        {
            var normalizedTen = NormalizeTen(dto.Ten);
            if (normalizedTen == null)
                return BadRequest("Tên cảm xúc không hợp lệ.");

            // check trùng tên
            bool exists = await _context.CamXucs
                                        .AnyAsync(c => c.Ten.ToLower() == normalizedTen.ToLower());
            if (exists)
                return BadRequest("Tên cảm xúc đã tồn tại.");

            var camXuc = new CamXuc { Ten = normalizedTen };
            _context.CamXucs.Add(camXuc);
            await _context.SaveChangesAsync();

            var result = new CamXucDTO { Id = camXuc.Id, Ten = camXuc.Ten };
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // ==================== CẬP NHẬT ====================
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật cảm xúc", Description = "Cập nhật tên của một cảm xúc.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Cập nhật thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ hoặc tên bị trùng.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy cảm xúc.")]
        public async Task<IActionResult> Update(int id, [FromBody] CamXucUpdateDTO dto)
        {
            var camXuc = await _context.CamXucs.FindAsync(id);
            if (camXuc == null)
                return NotFound("Không tìm thấy loại cảm xúc.");

            var normalizedTen = NormalizeTen(dto.Ten);
            if (normalizedTen == null)
                return BadRequest("Tên cảm xúc không hợp lệ.");

            // check trùng tên (ngoại trừ chính nó)
            bool exists = await _context.CamXucs
                                        .AnyAsync(c => c.Ten.ToLower() == normalizedTen.ToLower() && c.Id != id);
            if (exists)
                return BadRequest("Tên cảm xúc đã tồn tại.");

            camXuc.Ten = normalizedTen;
            _context.Entry(camXuc).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // ==================== XÓA ====================
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa cảm xúc", Description = "Xóa cảm xúc và các liên kết với tác phẩm nếu có.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Xóa thành công.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy cảm xúc.")]
        public async Task<IActionResult> Delete(int id)
        {
            var camXuc = await _context.CamXucs
                                       .Include(c => c.TacPham_CamXucs) // load liên kết N-N
                                       .SingleOrDefaultAsync(c => c.Id == id);

            if (camXuc == null)
                return NotFound("Không tìm thấy loại cảm xúc.");

            _context.TacPham_CamXucs.RemoveRange(camXuc.TacPham_CamXucs);
            _context.CamXucs.Remove(camXuc);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
