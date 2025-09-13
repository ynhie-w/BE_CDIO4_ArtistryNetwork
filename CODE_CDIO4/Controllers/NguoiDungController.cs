using CODE_CDIO4.Models;
using CODE_CDIO4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Swashbuckle.AspNetCore.Annotations;

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

        // ==================== CRUD ====================

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy tất cả người dùng")]
        public async Task<ActionResult<IEnumerable<NguoiDung>>> GetAll()
        {
            return await _context.NguoiDungs.Include(nd => nd.Quyen).ToListAsync();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy người dùng theo ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(NguoiDung))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy người dùng")]
        public async Task<ActionResult<NguoiDung>> GetById(int id)
        {
            var nguoiDung = await _context.NguoiDungs.Include(nd => nd.Quyen).FirstOrDefaultAsync(nd => nd.Id == id);
            if (nguoiDung == null) return NotFound();
            return nguoiDung;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo người dùng mới")]
        [SwaggerResponse(StatusCodes.Status201Created, "Tạo thành công", typeof(NguoiDung))]
        public async Task<ActionResult<NguoiDung>> Create(CreateUserDto dto)
        {
            var nguoiDung = new NguoiDung
            {
                Ten = dto.Ten,
                Email = dto.Email,
                Sdt = dto.Sdt,
                Id_PhanQuyen = dto.Id_PhanQuyen,
                MatKhau = HashPasswordToBytes(dto.MatKhau)
            };

            _context.NguoiDungs.Add(nguoiDung);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = nguoiDung.Id }, nguoiDung);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật thông tin người dùng")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Cập nhật thành công")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy người dùng")]
        public async Task<IActionResult> Update(int id, UpdateUserDto dto)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null) return NotFound();

            nguoiDung.Ten = dto.Ten;
            nguoiDung.Email = dto.Email;
            nguoiDung.Sdt = dto.Sdt;
            nguoiDung.Id_PhanQuyen = dto.Id_PhanQuyen ?? nguoiDung.Id_PhanQuyen;

            if (!string.IsNullOrEmpty(dto.MatKhau))
            {
                nguoiDung.MatKhau = HashPasswordToBytes(dto.MatKhau);
            }

            _context.Entry(nguoiDung).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa người dùng")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Xóa thành công")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy người dùng")]
        public async Task<IActionResult> Delete(int id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null) return NotFound();

            _context.NguoiDungs.Remove(nguoiDung);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==================== Đăng nhập ====================
        [HttpPost("Login")]
        [SwaggerOperation(Summary = "Đăng nhập")]
        [SwaggerResponse(StatusCodes.Status200OK, "Đăng nhập thành công", typeof(LoginResponseDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Email hoặc mật khẩu không đúng")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _context.NguoiDungs.SingleOrDefaultAsync(u => u.Email == model.Email);
            if (user == null) return Unauthorized("Email hoặc mật khẩu không đúng.");

            byte[] hashedPasswordBytes = HashPasswordToBytes(model.MatKhau);

            if (!hashedPasswordBytes.SequenceEqual(user.MatKhau))
                return Unauthorized("Email hoặc mật khẩu không đúng.");

            var loginResponse = new LoginResponseDto
            {
                Id = user.Id,
                Ten = user.Ten,
                Email = user.Email,
                Id_PhanQuyen = user.Id_PhanQuyen
            };

            return Ok(loginResponse);
        }

        // ==================== PHÂN QUYỀN ====================
        [HttpGet("PhanQuyen/{id}")]
        [SwaggerOperation(Summary = "Lấy quyền người dùng")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thành công", typeof(PhanQuyenResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy người dùng")]
        public async Task<IActionResult> GetPhanQuyen(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            var phanQuyenDto = new PhanQuyenResponseDto
            {
                Id = user.Id,
                Id_PhanQuyen = user.Id_PhanQuyen
            };

            return Ok(phanQuyenDto);
        }

        [HttpPut("PhanQuyen/{id}")]
        [SwaggerOperation(Summary = "Cập nhật quyền người dùng")]
        [SwaggerResponse(StatusCodes.Status200OK, "Cập nhật thành công", typeof(PhanQuyenResponseDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Không tìm thấy người dùng")]
        public async Task<IActionResult> UpdatePhanQuyen(int id, [FromBody] PhanQuyenUpdateDto dto)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            user.Id_PhanQuyen = dto.Id_PhanQuyen;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var phanQuyenDto = new PhanQuyenResponseDto
            {
                Id = user.Id,
                Id_PhanQuyen = user.Id_PhanQuyen
            };

            return Ok(phanQuyenDto);
        }

        // ==================== HASHING HELPERS ====================
        private byte[] HashPasswordToBytes(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }

    // ==================== DTOs ĐÃ CẬP NHẬT ====================

    public class CreateUserDto
    {
        public string Ten { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Sdt { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public int Id_PhanQuyen { get; set; } = 1; // Mặc định là thành viên
    }

    public class UpdateUserDto
    {
        public string Ten { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Sdt { get; set; } = null!;
        public string? MatKhau { get; set; }
        public int? Id_PhanQuyen { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
    }

    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Ten { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Id_PhanQuyen { get; set; }
    }

    public class PhanQuyenResponseDto
    {
        public int Id { get; set; }
        public int Id_PhanQuyen { get; set; }
    }

    public class PhanQuyenUpdateDto
    {
        public int Id_PhanQuyen { get; set; }
    }
}