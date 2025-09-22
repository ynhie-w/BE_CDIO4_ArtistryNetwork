using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDIO4_BE.Services.Interfaces
{
    public interface IAdminService
    {
        // ===== Đăng nhập =====
        Task<string> DangNhapAdmin(DangNhapDto yeuCau);

        // ===== Người dùng =====
        Task<List<NguoiDung>> LayDanhSachNguoiDung(int trang, int soLuong);
        Task<NguoiDung> LayThongTinNguoiDung(int id);
        Task<int> TaoNguoiDung(TaoNguoiDungDto yeuCau);
        Task<bool> CapNhatNguoiDung(int id, CapNhatNguoiDungDto yeuCau);
        Task<bool> XoaNguoiDung(int id);
        Task<bool> KhoaTaiKhoan(int id, bool biKhoa);

        // ===== Sản phẩm =====
        Task<List<TacPham>> LayDanhSachSanPham(int trang, int soLuong);
        Task<int> TaoSanPham(TaoSanPhamDto yeuCau);
        Task<bool> CapNhatSanPham(int id, CapNhatSanPhamDto yeuCau);
        Task<bool> XoaSanPham(int id);

        // ===== Bộ sưu tập =====
        Task<List<BoSuuTap>> LayDanhSachBoSuuTap(int trang, int soLuong);
        Task<object> TaoBoSuuTap(int idNguoiDung, int idTacPham);
        Task<bool> CapNhatBoSuuTap(int idNguoiDung, int idTacPham, bool? trangThai = null);
        Task<bool> XoaBoSuuTap(int id);

        // ===== Thống kê =====
        Task<object> LayThongKeTongQuan();
        Task<List<DonHang>> LayDanhSachDonHangTheoNgay(DateTime tuNgay, DateTime denNgay);
        Task<object> ThongKeNguoiDung(DateTime tuNgay, DateTime denNgay);
        Task<object> LayTopSanPhamBanChay(int top);

        // ===== Đơn hàng =====
        Task<List<DonHang>> LayDanhSachDonHang(int trang, int soLuong, string trangThai);
        Task<DonHang> LayChiTietDonHang(int id);
        Task<bool> CapNhatTrangThaiDonHang(int id, CapNhatTrangThaiDonHangDto yeuCau);
        Task<bool> HuyDonHang(int id, string lyDoHuy);
    }
}
