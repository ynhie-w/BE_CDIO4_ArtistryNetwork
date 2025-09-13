using CODE_CDIO4.Models;
using CODE_CDIO4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class GioHangController : ControllerBase
{
    private readonly AppDbContext _context;

    public GioHangController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/GioHang/NguoiMua/5
    [HttpGet("NguoiMua/{idNguoiMua}")]
    public async Task<ActionResult<IEnumerable<GioHangDto>>> GetGioHang(int idNguoiMua)
    {
        var gioHangs = await _context.GioHangs
                                     .Where(gh => gh.Id_NguoiMua == idNguoiMua)
                                     .Include(gh => gh.TacPham)
                                     .Select(gh => new GioHangDto
                                     {
                                         IdNguoiMua = gh.Id_NguoiMua,
                                         IdTacPham = gh.Id_TacPham,
                                         Loai = gh.Loai,
                                         TenTacPham = gh.TacPham.Ten,
                                         Gia = gh.TacPham.Gia,
                                         Anh = gh.TacPham.Anh,
                                         NgayThem = gh.NgayThem
                                     })
                                     .ToListAsync();

        if (!gioHangs.Any())
            return NotFound("Giỏ hàng của người dùng này trống.");

        return Ok(gioHangs);
    }

    // POST: api/GioHang
    [HttpPost]
    public async Task<ActionResult<GioHangDto>> AddToCart([FromBody] GioHang gioHangItem)
    {
        // Kiểm tra trùng
        var existingItem = await _context.GioHangs
                                         .FindAsync(gioHangItem.Id_NguoiMua, gioHangItem.Id_TacPham);

        if (existingItem != null)
            return Conflict("Tác phẩm này đã tồn tại trong giỏ hàng hoặc bộ sưu tập.");

        // Kiểm tra tác phẩm tồn tại
        var tacPham = await _context.TacPhams.FindAsync(gioHangItem.Id_TacPham);
        if (tacPham == null)
            return BadRequest("Tác phẩm không tồn tại.");

        gioHangItem.NgayThem = DateTime.Now;

        _context.GioHangs.Add(gioHangItem);
        await _context.SaveChangesAsync();

        var dto = new GioHangDto
        {
            IdNguoiMua = gioHangItem.Id_NguoiMua,
            IdTacPham = gioHangItem.Id_TacPham,
            Loai = gioHangItem.Loai,
            TenTacPham = tacPham.Ten,
            Gia = tacPham.Gia,
            Anh = tacPham.Anh,
            NgayThem = gioHangItem.NgayThem
        };

        return CreatedAtAction(nameof(GetGioHang), new { idNguoiMua = gioHangItem.Id_NguoiMua }, dto);
    }

    // DELETE: api/GioHang/NguoiMua/5/TacPham/10
    [HttpDelete("NguoiMua/{idNguoiMua}/TacPham/{idTacPham}")]
    public async Task<IActionResult> RemoveFromCart(int idNguoiMua, int idTacPham)
    {
        var item = await _context.GioHangs.FindAsync(idNguoiMua, idTacPham);
        if (item == null)
            return NotFound("Không tìm thấy mục này trong giỏ hàng.");

        _context.GioHangs.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Xóa thành công khỏi giỏ hàng." });
    }

    // PUT: api/GioHang/NguoiMua/5/TacPham/10/Loai
    [HttpPut("NguoiMua/{idNguoiMua}/TacPham/{idTacPham}/Loai")]
    public async Task<IActionResult> UpdateLoai(int idNguoiMua, int idTacPham, [FromBody] string loaiMoi)
    {
        var item = await _context.GioHangs.FindAsync(idNguoiMua, idTacPham);
        if (item == null)
            return NotFound("Không tìm thấy mục này.");

        item.Loai = loaiMoi;
        _context.GioHangs.Update(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cập nhật loại thành công." });
    }

    // ================= DTO =================
    public class GioHangDto
    {
        public int IdNguoiMua { get; set; }
        public int IdTacPham { get; set; }
        public string Loai { get; set; } = null!;
        public string TenTacPham { get; set; } = null!;
        public decimal Gia { get; set; }
        public string? Anh { get; set; }
        public DateTime NgayThem { get; set; }
    }
}
