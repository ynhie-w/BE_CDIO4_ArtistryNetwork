using CODE_CDIO4.Models;
using CODE_CDIO4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [SwaggerOperation(Summary = "Lấy tất cả cảm xúc", Description = "Trả về danh sách tất cả loại cảm xúc (chỉ Id và Tên)")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công, trả về danh sách cảm xúc.", typeof(IEnumerable<CamXucDTO>))]
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
        [SwaggerOperation(Summary = "Lấy cảm xúc theo ID", Description = "Trả về thông tin chi tiết của một cảm xúc dựa trên ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công, trả về cảm xúc.", typeof(CamXucDTO))]
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

        // ==================== TẠO MỚI ====================
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo cảm xúc mới", Description = "Thêm một cảm xúc mới vào cơ sở dữ liệu.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Tạo thành công.", typeof(CamXucDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ.")]
        public async Task<ActionResult<CamXucDTO>> Create([FromBody] CamXucDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Ten))
                return BadRequest("Tên cảm xúc không được để trống.");

            var camXuc = new CamXuc { Ten = dto.Ten };
            _context.CamXucs.Add(camXuc);
            await _context.SaveChangesAsync();

            dto.Id = camXuc.Id;
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        // ==================== CẬP NHẬT ====================
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật cảm xúc", Description = "Cập nhật tên của một cảm xúc.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Cập nhật thành công.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy cảm xúc.")]
        public async Task<IActionResult> Update(int id, [FromBody] CamXucDTO dto)
        {
            if (id != dto.Id || string.IsNullOrWhiteSpace(dto.Ten))
                return BadRequest("ID không khớp hoặc tên cảm xúc không hợp lệ.");

            var camXuc = await _context.CamXucs.FindAsync(id);
            if (camXuc == null)
                return NotFound("Không tìm thấy loại cảm xúc.");

            camXuc.Ten = dto.Ten;
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

            // Xóa tất cả liên kết với TacPham_CamXuc trước
            _context.TacPham_CamXucs.RemoveRange(camXuc.TacPham_CamXucs);
            _context.CamXucs.Remove(camXuc);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // ==================== DTO ====================
    public class CamXucDTO
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
    }
}
