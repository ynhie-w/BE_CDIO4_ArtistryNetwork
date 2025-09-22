using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Services.Interfaces;
using CDIO4_BE.Helper;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using CDIO4_BE.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CDIO4_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền truy cập
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ITaiKhoanService _taiKhoanService;

        public AdminController(IAdminService adminService, ITaiKhoanService taiKhoanService)
        {
            _adminService = adminService;
            _taiKhoanService = taiKhoanService;
        }

        // ===== ĐĂNG NHẬP ADMIN =====
        [HttpPost("dangnhap")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Đăng nhập admin bằng email hoặc số điện thoại, trả về JWT Token")]
        public async Task<IActionResult> DangNhapAdmin([FromBody] DangNhapDto yeuCau)
        {
            var token = await _adminService.DangNhapAdmin(yeuCau);
            if (token == null)
                return Unauthorized(new { ThongBao = "Sai email/sdt hoặc mật khẩu, hoặc không có quyền admin" });

            return Ok(new { Token = token, ThongBao = "Đăng nhập admin thành công" });
        }

        // ===== QUẢN LÝ NGƯỜI DÙNG =====
        [HttpGet("nguoidung")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả người dùng (có phân trang)")]
        public async Task<IActionResult> LayDanhSachNguoiDung([FromQuery] int trang = 1, [FromQuery] int soLuong = 10)
        {
            var danhSach = await _adminService.LayDanhSachNguoiDung(trang, soLuong);
            return Ok(danhSach);
        }

        [HttpGet("nguoidung/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lấy thông tin chi tiết một người dùng")]
        public async Task<IActionResult> LayThongTinNguoiDung(int id)
        {
            var nguoiDung = await _adminService.LayThongTinNguoiDung(id);
            if (nguoiDung == null)
                return NotFound(new { ThongBao = "Không tìm thấy người dùng" });

            return Ok(nguoiDung);
        }

        [HttpPost("nguoidung")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Tạo tài khoản người dùng mới (admin tạo)")]
        public async Task<IActionResult> TaoNguoiDung([FromBody] TaoNguoiDungDto yeuCau)
        {
            var newUserId = await _adminService.TaoNguoiDung(yeuCau);
            if (newUserId == 0)
                return BadRequest(new { ThongBao = "Không thể tạo người dùng. Email/SĐT có thể đã tồn tại" });

            return Ok(new { NewUserId = newUserId, ThongBao = "Tạo người dùng thành công" });
        }

        [HttpPut("nguoidung/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Cập nhật thông tin người dùng")]
        public async Task<IActionResult> CapNhatNguoiDung(int id, [FromBody] CapNhatNguoiDungDto yeuCau)
        {
            var ketQua = await _adminService.CapNhatNguoiDung(id, yeuCau);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể cập nhật người dùng" });

            return Ok(new { ThongBao = "Cập nhật thông tin người dùng thành công" });
        }

        [HttpDelete("nguoidung/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Xóa người dùng")]
        public async Task<IActionResult> XoaNguoiDung(int id)
        {
            var ketQua = await _adminService.XoaNguoiDung(id);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể xóa người dùng" });

            return Ok(new { ThongBao = "Xóa người dùng thành công" });
        }

        [HttpPut("nguoidung/{id}/khoa")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Khóa/Mở khóa tài khoản người dùng")]
        public async Task<IActionResult> KhoaTaiKhoan(int id, [FromBody] KhoaTaiKhoanDto yeuCau)
        {
            var ketQua = await _adminService.KhoaTaiKhoan(id, yeuCau.BiKhoa);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể thay đổi trạng thái tài khoản" });

            return Ok(new { ThongBao = yeuCau.BiKhoa ? "Khóa tài khoản thành công" : "Mở khóa tài khoản thành công" });
        }

        // ===== QUẢN LÝ NỘI DUNG =====
        [HttpGet("noidung/sanpham")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả sản phẩm")]
        public async Task<IActionResult> LayDanhSachSanPham([FromQuery] int trang = 1, [FromQuery] int soLuong = 10)
        {
            var danhSach = await _adminService.LayDanhSachSanPham(trang, soLuong);
            return Ok(danhSach);
        }

        [HttpPost("noidung/sanpham")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Tạo sản phẩm mới")]
        public async Task<IActionResult> TaoSanPham([FromBody] TaoSanPhamDto yeuCau)
        {
            var newProductId = await _adminService.TaoSanPham(yeuCau);
            return Ok(new { NewProductId = newProductId, ThongBao = "Tạo sản phẩm thành công" });
        }

        [HttpPut("noidung/sanpham/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Cập nhật sản phẩm")]
        public async Task<IActionResult> CapNhatSanPham(int id, [FromBody] CapNhatSanPhamDto yeuCau)
        {
            var ketQua = await _adminService.CapNhatSanPham(id, yeuCau);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể cập nhật sản phẩm" });

            return Ok(new { ThongBao = "Cập nhật sản phẩm thành công" });
        }

        [HttpDelete("noidung/sanpham/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Xóa sản phẩm")]
        public async Task<IActionResult> XoaSanPham(int id)
        {
            var ketQua = await _adminService.XoaSanPham(id);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể xóa sản phẩm" });

            return Ok(new { ThongBao = "Xóa sản phẩm thành công" });
        }

        [HttpGet("noidung/bosuutap")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả bộ sưu tập")]
        public async Task<IActionResult> LayDanhSachBoSuuTap([FromQuery] int trang = 1, [FromQuery] int soLuong = 10)
        {
            var danhSach = await _adminService.LayDanhSachBoSuuTap(trang, soLuong);
            return Ok(danhSach);
        }
        [HttpPost("noidung/bosuutap")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Tạo bộ sưu tập mới")]
        public async Task<IActionResult> TaoBoSuuTap([FromQuery] int idNguoiDung, [FromQuery] int idTacPham)
        {
            var newCollectionId = await _adminService.TaoBoSuuTap(idNguoiDung, idTacPham);
            return Ok(new { NewCollectionId = newCollectionId, ThongBao = "Tạo bộ sưu tập thành công" });
        }

        [HttpPut("noidung/bosuutap")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái bộ sưu tập")]
        public async Task<IActionResult> CapNhatBoSuuTap([FromQuery] int idNguoiDung, [FromQuery] int idTacPham, [FromQuery] bool trangThai)
        {
            var ketQua = await _adminService.CapNhatBoSuuTap(idNguoiDung, idTacPham, trangThai);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể cập nhật bộ sưu tập" });

            return Ok(new { ThongBao = "Cập nhật bộ sưu tập thành công" });
        }

        [HttpDelete("noidung/bosuutap/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Xóa bộ sưu tập")]
        public async Task<IActionResult> XoaBoSuuTap(int id)
        {
            var ketQua = await _adminService.XoaBoSuuTap(id);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể xóa bộ sưu tập" });

            return Ok(new { ThongBao = "Xóa bộ sưu tập thành công" });
        }

        // ===== THỐNG KÊ =====
        [HttpGet("thongke/tong-quan")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Thống kê tổng quan hệ thống")]
        public async Task<IActionResult> ThongKeTongQuan()
        {
            var thongKe = await _adminService.LayThongKeTongQuan();
            return Ok(thongKe);
        }

        // Thống kê người dùng theo thời gian
        [HttpGet("thongke/donhang-theo-thoi-gian")]
        public async Task<IActionResult> ThongKeDonHangTheoThoiGian([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay)
        {
            var donHangs = await _adminService.LayDanhSachDonHangTheoNgay(tuNgay, denNgay);
            return Ok(donHangs);
        }



        [HttpGet("thongke/nguoi-dung")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Thống kê người dùng mới theo thời gian")]
        public async Task<IActionResult> ThongKeNguoiDung([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay)
        {
            var thongKe = await _adminService.ThongKeNguoiDung(tuNgay, denNgay);
            return Ok(thongKe);
        }

        [HttpGet("thongke/san-pham-ban-chay")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Top sản phẩm bán chạy nhất")]
        public async Task<IActionResult> ThongKeSanPhamBanChay([FromQuery] int top = 10)
        {
            var thongKe = await _adminService.LayTopSanPhamBanChay(top);
            return Ok(thongKe);
        }

        // ===== QUẢN LÝ ĐỖN HÀNG =====
        [HttpGet("donhang")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả đơn hàng")]
        public async Task<IActionResult> LayDanhSachDonHang([FromQuery] int trang = 1, [FromQuery] int soLuong = 10, [FromQuery] string trangThai = "")
        {
            var danhSach = await _adminService.LayDanhSachDonHang(trang, soLuong, trangThai);
            return Ok(danhSach);
        }

        [HttpGet("donhang/{id}")]
        [SwaggerOperation(Summary = "Lấy chi tiết đơn hàng")]
        public async Task<IActionResult> LayChiTietDonHang(int id)
        {
            var donHang = await _adminService.LayChiTietDonHang(id);
            if (donHang == null)
                return NotFound(new { ThongBao = "Không tìm thấy đơn hàng" });

            return Ok(donHang);
        }

        [HttpPut("donhang/{id}/trang-thai")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái đơn hàng")]
        public async Task<IActionResult> CapNhatTrangThaiDonHang(int id, [FromBody] CapNhatTrangThaiDonHangDto yeuCau)
        {
            var ketQua = await _adminService.CapNhatTrangThaiDonHang(id, yeuCau);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể cập nhật trạng thái đơn hàng" });

            return Ok(new { ThongBao = "Cập nhật trạng thái đơn hàng thành công" });
        }

        [HttpDelete("donhang/{id}")]
        public async Task<IActionResult> HuyDonHang(int id, [FromBody] HuyDonHangDto yeuCau)
        {
            var ketQua = await _adminService.HuyDonHang(id, yeuCau.LyDoHuy);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể hủy đơn hàng" });

            return Ok(new { ThongBao = "Hủy đơn hàng thành công" });
        }

    }
}