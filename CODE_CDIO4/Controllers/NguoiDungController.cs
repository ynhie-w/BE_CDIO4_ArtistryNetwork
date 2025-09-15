using CODE_CDIO4.Models;
using CODE_CDIO4.DTOs;
using CODE_CDIO4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;

namespace CODE_CDIO4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NguoiDungController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== GET ====================
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy tất cả người dùng")]
        public async Task<ActionResult<IEnumerable<NguoiDung>>> GetAll()
        {
            return await _context.NguoiDungs
                .Include(x => x.Quyen)
                .Include(x => x.CapDo)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy người dùng theo ID")]
        public async Task<ActionResult<NguoiDung>> GetById(int id)
        {
            var user = await _context.NguoiDungs
                .Include(x => x.Quyen)
                .Include(x => x.CapDo)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) return NotFound();
            return user;
        }

        // ==================== CREATE ====================
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo người dùng mới")]
        public async Task<ActionResult<NguoiDung>> Create([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) && string.IsNullOrEmpty(dto.Sdt))
                return BadRequest("Phải có ít nhất Email hoặc SĐT");

            var user = new NguoiDung
            {
                Ten = dto.Ten,
                Sdt = string.IsNullOrWhiteSpace(dto.Sdt) ? null : dto.Sdt,
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email,
                MatKhau = HashPassword(dto.MatKhau),
                DiemThuong = dto.DiemThuong ?? 0,
                Id_CapDo = dto.Id_CapDo,
                Id_PhanQuyen = dto.Id_PhanQuyen ?? 1,
                TrangThai = true,
                AnhDaiDien = dto.AnhDaiDien
            };

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // ==================== UPDATE ====================
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật người dùng")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(dto.Email) && string.IsNullOrEmpty(dto.Sdt))
                return BadRequest("Phải có ít nhất Email hoặc SĐT");

            user.Ten = dto.Ten ?? user.Ten;
            user.Sdt = dto.Sdt ?? user.Sdt;
            user.Email = dto.Email ?? user.Email;
            user.AnhDaiDien = dto.AnhDaiDien ?? user.AnhDaiDien;
            user.DiemThuong = dto.DiemThuong ?? user.DiemThuong;
            user.Id_CapDo = dto.Id_CapDo ?? user.Id_CapDo;
            user.Id_PhanQuyen = dto.Id_PhanQuyen ?? user.Id_PhanQuyen;
            user.TrangThai = dto.TrangThai ?? user.TrangThai;

            if (!string.IsNullOrEmpty(dto.MatKhau))
                user.MatKhau = HashPassword(dto.MatKhau);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ==================== DELETE ====================
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa người dùng (mềm)")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            user.TrangThai = false;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ==================== LOGIN ====================
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Đăng nhập")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(x => x.Email == dto.Email || x.Sdt == dto.Sdt);

            if (user == null) return Unauthorized("Không tìm thấy tài khoản");

            if (!user.MatKhau.SequenceEqual(HashPassword(dto.MatKhau)))
                return Unauthorized("Sai mật khẩu");

            return Ok(new LoginResponseDto
            {
                Id = user.Id,
                Ten = user.Ten,
                Email = user.Email,
                Sdt = user.Sdt,
                Id_PhanQuyen = user.Id_PhanQuyen,
                TrangThai = user.TrangThai
            });
        }

        // ==================== HELPER ====================
        private byte[] HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

}
