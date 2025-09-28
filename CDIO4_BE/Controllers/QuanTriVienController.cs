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
    [Route("api/quan-tri-vien")]
    public class QuanTriVienController : ControllerBase
    {
        private readonly IQuanTriVienService _adminService;
        private readonly ITaiKhoanService _taiKhoanService;

        public QuanTriVienController(IQuanTriVienService adminService, ITaiKhoanService taiKhoanService)
        {
            _adminService = adminService;
            _taiKhoanService = taiKhoanService;
        }
        private bool LaQuanTriVien()
        {
            var permissionId = User.FindFirst("id_phanquyen")?.Value;
            return permissionId == "2";
        }

        // ===== QUẢN LÝ NGƯỜI DÙNG =====
        [HttpGet("nguoidung")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả người dùng (có phân trang)")]
        public async Task<IActionResult> LayDanhSachNguoiDung([FromQuery] int trang = 1, [FromQuery] int soLuong = 10)
        {
            if (!LaQuanTriVien()) return Forbid("Không đủ quyền truy cập");
            var danhSach = await _adminService.LayDanhSachNguoiDung(trang, soLuong);
            return Ok(danhSach);
        }

        [HttpGet("nguoidung/{id}")]
        [SwaggerOperation(Summary = "Lấy thông tin chi tiết một người dùng")]
        public async Task<IActionResult> LayThongTinNguoiDung(int id)
        {
            var nguoiDung = await _adminService.LayThongTinNguoiDung(id);
            if (nguoiDung == null)
                return NotFound(new { ThongBao = "Không tìm thấy người dùng" });

            return Ok(nguoiDung);
        }
        [HttpPut("nguoidung/{id}")]
        [SwaggerOperation(Summary = "Cập nhật thông tin người dùng")]
        public async Task<IActionResult> CapNhatNguoiDung(int id, [FromBody] CapNhatNguoiDungDto yeuCau)
        {
            var ketQua = await _adminService.CapNhatNguoiDung(id, yeuCau);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể cập nhật người dùng" });

            return Ok(new { ThongBao = "Cập nhật thông tin người dùng thành công" });
        }

        [HttpDelete("nguoidung/{id}")]
        [SwaggerOperation(Summary = "Xóa người dùng")]
        public async Task<IActionResult> XoaNguoiDung(int id)
        {
            var ketQua = await _adminService.XoaNguoiDung(id);
            if (!ketQua)
                return BadRequest(new { ThongBao = "Không thể xóa người dùng" });

            return Ok(new { ThongBao = "Xóa người dùng thành công" });
        }

        [HttpPut("nguoidung/{id}/khoa")]
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
        [SwaggerOperation(Summary = "Lấy danh sách tất cả sản phẩm")]
        public async Task<IActionResult> LayDanhSachSanPham([FromQuery] int trang = 1, [FromQuery] int soLuong = 10)
        {
            var danhSach = await _adminService.LayDanhSachSanPham(trang, soLuong);
            return Ok(danhSach);
        }

        [HttpGet("noidung/bosuutap")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả bộ sưu tập")]
        public async Task<IActionResult> LayDanhSachBoSuuTap([FromQuery] int trang = 1, [FromQuery] int soLuong = 10)
        {
            var danhSach = await _adminService.LayDanhSachBoSuuTap(trang, soLuong);
            return Ok(danhSach);
        }

        // ===== THỐNG KÊ =====
        [HttpGet("thongke/tong-quan")]
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
        [SwaggerOperation(Summary = "Thống kê người dùng mới theo thời gian")]
        public async Task<IActionResult> ThongKeNguoiDung([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay)
        {
            var thongKe = await _adminService.ThongKeNguoiDung(tuNgay, denNgay);
            return Ok(thongKe);
        }

        [HttpGet("thongke/san-pham-ban-chay")]
        [SwaggerOperation(Summary = "Top sản phẩm bán chạy nhất")]
        public async Task<IActionResult> ThongKeSanPhamBanChay([FromQuery] int top = 10)
        {
            var thongKe = await _adminService.LayTopSanPhamBanChay(top);
            return Ok(thongKe);
        }

        // ===== QUẢN LÝ ĐƠN HÀNG =====
        [HttpGet("donhang")]
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

    }
}