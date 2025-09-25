using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace CDIO4_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TacPhamController : ControllerBase
    {
        private readonly ITacPhamService _tacPhamService;
        private readonly AppDbContext _context;

        public TacPhamController(ITacPhamService tacPhamService, AppDbContext context)
        {
            _tacPhamService = tacPhamService;
            _context = context;
        }

        // GET: /api/tacPham
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách tác phẩm (có phân trang, không cần đăng nhập)")]
        public async Task<IActionResult> LayDanhSachTacPham([FromQuery] int trang = 1, [FromQuery] int soLuong = 10)
        {
            var list = await _tacPhamService.LayDanhSachTacPham(trang, soLuong);
            return Ok(list);
        }


        // GET: /api/tacPham/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Xem chi tiết tác phẩm, sẽ tăng lượt xem (không cần đăng nhập) ")]
        public async Task<IActionResult> LayChiTietTacPham(int id)
        {
            var tp = await _tacPhamService.LayChiTietTacPham(id);
            if (tp == null) return NotFound("Không tìm thấy tác phẩm");
            return Ok(tp);
        }

        // GET: /api/tacPham/search
        [HttpGet("Search")]
        [SwaggerOperation(Summary = "Tìm kiếm tác phẩm theo từ khóa  (không cần đăng nhập)")]
        public async Task<IActionResult> TimKiemTacPham(string keyword)
        {
            var list = await _tacPhamService.TimKiemTacPham(keyword);
            return Ok(list);
        }

        // GET: /api/tacPham/BoSuuTap
        [HttpGet("BoSuuTap")]
        [Authorize]
        [SwaggerOperation(Summary = "Lấy danh sách bộ sưu tập của người dùng đã đăng nhập (cần đăng nhập)")]
        public async Task<IActionResult> LayBoSuuTap()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var list = await _tacPhamService.LayBoSuuTap(userId);
            return Ok(list);
        }

        // GET: /api/tacPham/My
        [HttpGet("My")]
        [Authorize]
        [SwaggerOperation(Summary = "Lấy danh sách tác phẩm của chính mình  (cần đăng nhập)")]
        public async Task<IActionResult> LayTacPhamCuaToi()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var list = await _tacPhamService.LayTacPhamCuaToi(userId);
            return Ok(list);
        }

        // POST: /api/tacPham/ThemVaoBoSuuTap/{idTacPham}
        [HttpPost("ThemVaoBoSuuTap/{idTacPham}")]
        [Authorize]
        [SwaggerOperation(Summary = "Thêm tác phẩm vào bộ sưu tập của người dùng  (cần đăng nhập)")]
        public async Task<IActionResult> ThemVaoBoSuuTap(int idTacPham)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _tacPhamService.ThemVaoBoSuuTap(userId, idTacPham);
            return Ok("Đã thêm vào bộ sưu tập");
        }
        // Trong file TacPhamController.cs
        [HttpDelete("BoSuuTap/{idTacPham}")]
        [Authorize]
        [SwaggerOperation(Summary = "Xóa tác phẩm khỏi bộ sưu tập của người dùng  (cần đăng nhập)")]
        public async Task<IActionResult> XoaKhoiBoSuuTap(int idTacPham)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _tacPhamService.XoaKhoiBoSuuTap(userId, idTacPham);
                return Ok("Đã xóa tác phẩm khỏi bộ sưu tập.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: /api/tacPham/Mua/{idTacPham}
        [HttpPost("Mua/{idTacPham}")]
        [Authorize]
        [SwaggerOperation(Summary = "Mua tác phẩm (demo chỉ ghi nhận đánh giá 5 sao)  (cần đăng nhập)")]
        public async Task<IActionResult> MuaTacPham(int idTacPham)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _tacPhamService.MuaTacPham(userId, idTacPham);
            return Ok("Đã mua tác phẩm");
        }

        [HttpGet("tacpham/{idTacPham}")]
        [SwaggerOperation(Summary = "Xem danh sách bình luận của tác phẩm (bao gồm trả lời)")]
        public async Task<IActionResult> GetBinhLuansByTacPham(int idTacPham)
        {
            var binhLuans = await _tacPhamService.XemDanhSachBinhLuanCuaTacPham(idTacPham);
            return Ok(binhLuans);
        }

        // POST: /api/tacPham/{idTacPham}/binhluan
        [HttpPost("{idTacPham}/binhluan")]
        [SwaggerOperation(Summary = "Thêm bình luận hoặc trả lời bình luận  (cần đăng nhập)")]
        [Authorize]
        public async Task<IActionResult> ThemBinhLuan(int idTacPham, [FromBody] ThemBinhLuanRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (request.IdBinhLuanCha.HasValue)
                await _tacPhamService.ThemTraLoiBinhLuan(userId, request.IdBinhLuanCha, request.NoiDung);
            else
                await _tacPhamService.ThemBinhLuan(userId, idTacPham, request.NoiDung);

            return Ok("Bình luận đã được thêm");
        }
        [HttpPut("binhluan/sua")]
        [Authorize]
        [SwaggerOperation(Summary = "Sửa nội dung bình luận  (cần đăng nhập)")]
        public async Task<IActionResult> SuaBinhLuan([FromBody] SuaBinhLuanDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _tacPhamService.SuaBinhLuan(userId, request.IdBinhLuan, request.NoiDungMoi);
                return Ok("Bình luận đã được cập nhật thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("binhluan/{idBinhLuan}")]
        [Authorize]
        [SwaggerOperation(Summary = "Xóa bình luận  (cần đăng nhập)")]
        public async Task<IActionResult> XoaBinhLuan(int idBinhLuan)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _tacPhamService.XoaBinhLuan(userId, idBinhLuan);
                return Ok("Bình luận đã được xóa thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("{idTacPham}/camxuc")]
        [SwaggerOperation(Summary = "Thêm/cập nhật cảm xúc  (cần đăng nhập)")]
        [Authorize]
        public async Task<IActionResult> UpsertCamXuc(int idTacPham, [FromBody] int idCamXuc)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _tacPhamService.UpsertCamXuc(userId, idTacPham, idCamXuc);
                return Ok(new { message = "Cảm xúc đã được ghi nhận/cập nhật" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("camxuc/{idTacPham}")]
        [SwaggerOperation(Summary = "Xóa cảm xúc (cần đăng nhập)")]
        [Authorize]
        public async Task<IActionResult> XoaCamXuc(int idTacPham)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _tacPhamService.XoaCamXuc(userId, idTacPham);
                return Ok("Cảm xúc đã được xóa thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

   
}
