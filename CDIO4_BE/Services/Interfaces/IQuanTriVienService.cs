using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDIO4_BE.Services.Interfaces
{
    public interface IQuanTriVienService
    {


        // ===== Người dùng =====
        Task<object> LayDanhSachNguoiDung(int trang, int soLuong);
        Task<object?> LayThongTinNguoiDung(int id);
        Task<int> TaoNguoiDung(TaoNguoiDungDto yeuCau);
        Task<bool> CapNhatNguoiDung(int id, CapNhatNguoiDungDto yeuCau);
        Task<bool> XoaNguoiDung(int id);
        Task<bool> KhoaTaiKhoan(int id, bool biKhoa);

        // ===== Sản phẩm =====
        Task<List<TacPham>> LayDanhSachSanPham(int trang, int soLuong);

        // ===== Bộ sưu tập =====
        Task<List<BoSuuTap>> LayDanhSachBoSuuTap(int trang, int soLuong);

        // ===== Thống kê =====
        Task<object> LayThongKeTongQuan();
        Task<List<DonHang>> LayDanhSachDonHangTheoNgay(DateTime tuNgay, DateTime denNgay);
        Task<object> ThongKeNguoiDung(DateTime tuNgay, DateTime denNgay);
        Task<object> LayTopSanPhamBanChay(int top);

        // ===== Đơn hàng =====
        Task<object> LayDanhSachDonHang(int trang, int soLuong, string trangThai);
        Task<DonHangDto?> LayChiTietDonHang(int id);
        Task<bool> CapNhatTrangThaiDonHang(int id, CapNhatTrangThaiDonHangDto yeuCau);
        Task<bool> HuyDonHang(int id, string lyDoHuy);
    }
}