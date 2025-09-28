﻿using CDIO4_BE.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDIO4_BE.Services.Interfaces
{
    public interface ITacPhamService
    {
        Task<int> TaoTacPham(TaoTacPhamDto dto, int userId);
        Task<bool> SuaTacPham(int id, SuaTacPhamDto dto, int userId);
        Task<bool> XoaTacPham(int id, int userId);
        Task<List<TacPhamListDto>> LayDanhSachTacPham(int trang, int soLuong);

        Task<TacPhamDto?> LayChiTietTacPham(int id);
        Task<List<TacPhamDto>> TimKiemTacPham(string keyword);
        Task<List<TacPhamDto>> LayBoSuuTap(int idNguoiDung);
        Task<List<TacPhamDto>> LayTacPhamCuaToi(int idNguoiDung);
        

        Task ThemVaoBoSuuTap(int idNguoiDung, int idTacPham);
        Task XoaKhoiBoSuuTap(int idNguoiDung, int idTacPham);
        Task<IEnumerable<BinhLuanDto>> XemDanhSachBinhLuanCuaTacPham(int idTacPham, int? currentUserId = null);
        Task ThemBinhLuan(int idNguoiDung, int idTacPham, string noiDung);
        Task ThemTraLoiBinhLuan(int idNguoiDung, int? idBinhLuanCha, string noiDung);
        Task SuaBinhLuan(int idNguoiDung, int idBinhLuan, string noiDungMoi);
        Task XoaBinhLuan(int idNguoiDung, int idBinhLuan);

        // Cảm xúc
        Task UpsertCamXuc(int idNguoiDung, int idTacPham, int idCamXuc);
        Task XoaCamXuc(int idNguoiDung, int idTacPham);
        Task MuaTacPham(int idNguoiDung, int idTacPham);
    }
}
