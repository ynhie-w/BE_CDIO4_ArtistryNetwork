using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDIO4_BE.Services
{
    public class TacPhamService : ITacPhamService
    {
        private readonly AppDbContext _context;

        public TacPhamService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<TacPhamListDto>> LayDanhSachTacPham(int trang, int soLuong)
        {
            var query = _context.TacPhams
                .Include(tp => tp.NguoiTao)
                .Include(tp => tp.BinhLuans)
                .Include(tp => tp.TacPham_CamXucs)
                .Include(tp => tp.TheLoai)
                .AsQueryable();

            return await query
                .OrderByDescending(tp => tp.NgayTao)
                .Skip((trang - 1) * soLuong)
                .Take(soLuong)
                .Select(tp => new TacPhamListDto
                {
                    Id = tp.Id,
                    Ten = tp.Ten,
                    MoTa = tp.MoTa,
                    Anh = tp.Anh,
                    NgayTao = tp.NgayTao,
                    NguoiTao = tp.NguoiTao == null ? null : new NguoiDungDto
                    {
                        Id = tp.NguoiTao.Id,
                        Ten = tp.NguoiTao.Ten,
                        AnhDaiDien = tp.NguoiTao.AnhDaiDien
                    },
                    SoLuongBinhLuan = tp.BinhLuans.Count(b => b.TrangThai),
                    SoLuongCamXuc = tp.TacPham_CamXucs.Count(c => c.TrangThai),
                    LuotXem = tp.LuotXem
                }).ToListAsync();
        }


        public async Task<TacPhamDto?> LayChiTietTacPham(int id)
        {
            var t = await _context.TacPhams
                .Include(tp => tp.NguoiTao)
                .Include(tp => tp.TacPham_CamXucs)
                    .ThenInclude(tc => tc.CamXuc)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (t == null) return null;

            // Tăng lượt xem
            t.LuotXem += 1;
            await _context.SaveChangesAsync();

            // Lấy bình luận + trả lời
            var binhLuans = await LayBinhLuanVaTraLoi(t.Id);


            return new TacPhamDto
            {
                Id = t.Id,
                Ten = t.Ten,
                MoTa = t.MoTa,
                Anh = t.Anh,
                Gia = t.Gia,
                LuotXem = t.LuotXem,
                NgayTao = t.NgayTao,
                NguoiTao = t.NguoiTao == null ? null : new NguoiDungDto
                {
                    Id = t.NguoiTao.Id,
                    Ten = t.NguoiTao.Ten,
                    AnhDaiDien = t.NguoiTao.AnhDaiDien
                },
                  
            };
        }


        public async Task<List<TacPhamDto>> TimKiemTacPham(string keyword)
        {
            return await _context.TacPhams
                .Where(tp => tp.Ten.Contains(keyword) || (tp.MoTa != null && tp.MoTa.Contains(keyword)))
                .Select(tp => new TacPhamDto
                {
                    Id = tp.Id,
                    Ten = tp.Ten,
                    MoTa = tp.MoTa,
                    Anh = tp.Anh,
                    Gia = tp.Gia,
                    LuotXem = tp.LuotXem,
                    NgayTao = tp.NgayTao
                }).ToListAsync();
        }

        public async Task<List<TacPhamDto>> LayBoSuuTap(int idNguoiDung)
        {
            return await _context.BoSuuTaps
                .Where(b => b.Id_NguoiDung == idNguoiDung && b.TrangThai)
                .Include(b => b.TacPham)
                .Select(b => new TacPhamDto
                {
                    Id = b.TacPham.Id,
                    Ten = b.TacPham.Ten,
                    MoTa = b.TacPham.MoTa,
                    Anh = b.TacPham.Anh,
                    Gia = b.TacPham.Gia,
                    LuotXem = b.TacPham.LuotXem,
                    NgayTao = b.TacPham.NgayTao
                }).ToListAsync();
        }

        public async Task<List<TacPhamDto>> LayTacPhamCuaToi(int idNguoiDung)
        {
            return await _context.TacPhams
                .Where(tp => tp.Id_NguoiTao == idNguoiDung)
                .Select(tp => new TacPhamDto
                {
                    Id = tp.Id,
                    Ten = tp.Ten,
                    MoTa = tp.MoTa,
                    Anh = tp.Anh,
                    Gia = tp.Gia,
                    LuotXem = tp.LuotXem,
                    NgayTao = tp.NgayTao
                }).ToListAsync();
        }

        public async Task ThemVaoBoSuuTap(int idNguoiDung, int idTacPham)
        {
            if (!await _context.BoSuuTaps.AnyAsync(b => b.Id_NguoiDung == idNguoiDung && b.Id_TacPham == idTacPham))
            {
                _context.BoSuuTaps.Add(new BoSuuTap
                {
                    Id_NguoiDung = idNguoiDung,
                    Id_TacPham = idTacPham,
                    NgayThem = DateTime.Now,
                    TrangThai = true
                });
                await _context.SaveChangesAsync();
            }
        }
        public async Task XoaKhoiBoSuuTap(int idNguoiDung, int idTacPham)
        {
            var boSuuTap = await _context.BoSuuTaps.FirstOrDefaultAsync(b => b.Id_NguoiDung == idNguoiDung && b.Id_TacPham == idTacPham);
            if (boSuuTap == null)
            {
                // Hoặc ném ngoại lệ, tùy vào logic bạn muốn
                throw new Exception("Không tìm thấy tác phẩm trong bộ sưu tập của bạn.");
            }

            // Soft delete bằng cách cập nhật trạng thái
            boSuuTap.TrangThai = false;
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<BinhLuanDto>> XemDanhSachBinhLuanCuaTacPham(int idTacPham, int? currentUserId = null)
        {
            var binhLuans = await _context.BinhLuans
                .Where(b => b.Id_TacPham == idTacPham && b.TrangThai == true)
                .Include(b => b.NguoiBinhLuan)
                .OrderByDescending(b => b.NgayTao)
                .ToListAsync();

            var result = binhLuans.Select(bl => new BinhLuanDto
            {
                Id = bl.Id,
                NoiDung = bl.NoiDung,
                NgayTao = bl.NgayTao,
                Level = bl.Level,
                NguoiBinhLuan = new NguoiDungDto
                {
                    Id = bl.NguoiBinhLuan.Id,
                    Ten = bl.NguoiBinhLuan.Ten
                },
                // ChuSoHuu = true nếu user hiện tại chính là người viết bình luận
                ChuSoHuu = currentUserId != null && bl.NguoiBinhLuan.Id == currentUserId
            });

            return result;
        }


        public async Task ThemBinhLuan(int idNguoiDung, int idTacPham, string noiDung)
        {
            _context.BinhLuans.Add(new BinhLuan
            {
                Id_NguoiDung = idNguoiDung,
                Id_TacPham = idTacPham,
                NoiDung = noiDung,
                NgayTao = DateTime.Now,
                TrangThai = true
            });
            await _context.SaveChangesAsync();
        }

  
        public async Task ThemTraLoiBinhLuan(int idNguoiDung, int? idBinhLuanCha, string noiDung)
        {
            if (idBinhLuanCha.HasValue)
            {
                var binhLuanCha = await _context.BinhLuans.FindAsync(idBinhLuanCha.Value);
                if (binhLuanCha != null && binhLuanCha.Level == 0)
                {
                    // Kiểm tra người dùng chỉ trả lời 1 lần
                    bool daTraLoi = await _context.BinhLuans
                        .AnyAsync(b => b.Id_BinhLuanCha == idBinhLuanCha.Value && b.Id_NguoiDung == idNguoiDung);

                    if (!daTraLoi)
                    {
                        _context.BinhLuans.Add(new BinhLuan
                        {
                            Id_TacPham = binhLuanCha.Id_TacPham,
                            Id_NguoiDung = idNguoiDung,
                            NoiDung = noiDung,
                            NgayTao = DateTime.Now,
                            Level = 1,
                            Id_BinhLuanCha = idBinhLuanCha,
                            TrangThai = true
                        });
                        await _context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                // Bình luận mới
                // Cần truyền Id tác phẩm
                throw new ArgumentException("idBinhLuanCha null, cần dùng ThemBinhLuan trực tiếp hoặc truyền Id tác phẩm");
            }
        }


        public async Task<List<BinhLuanDto>> LayBinhLuanVaTraLoi(int idTacPham)
        {
            // Load tất cả bình luận cho tác phẩm
            var binhLuans = await _context.BinhLuans
                .Where(b => b.Id_TacPham == idTacPham && b.TrangThai)
                .OrderBy(b => b.NgayTao)
                .ToListAsync();

            // Lọc level 0, gắn trả lời
            var binhLuanDtos = binhLuans
                .Where(b => b.Level == 0)
                .Select(b => new BinhLuanDto
                {
                    Id = b.Id,
                    NoiDung = b.NoiDung,
                    NgayTao = b.NgayTao,
                    NguoiBinhLuan = b.NguoiBinhLuan == null ? null : new NguoiDungDto
                    {
                        Id = b.NguoiBinhLuan.Id,
                        Ten = b.NguoiBinhLuan.Ten,
                        AnhDaiDien = b.NguoiBinhLuan.AnhDaiDien
                    },
                    TraLoi = binhLuans
                        .Where(t => t.Id_BinhLuanCha == b.Id)
                        .Select(t => new BinhLuanDto
                        {
                            Id = t.Id,
                            NoiDung = t.NoiDung,
                            NgayTao = t.NgayTao,
                            NguoiBinhLuan = t.NguoiBinhLuan == null ? null : new NguoiDungDto
                            {
                                Id = t.NguoiBinhLuan.Id,
                                Ten = t.NguoiBinhLuan.Ten,
                                AnhDaiDien = t.NguoiBinhLuan.AnhDaiDien
                            }
                        }).ToList()
                }).ToList();

            return binhLuanDtos;
        }

        public async Task MuaTacPham(int idNguoiDung, int idTacPham)
        {
            // Demo chỉ ghi nhận đánh giá 5 sao
            _context.DanhGias.Add(new DanhGia
            {
                Id_NguoiDung = idNguoiDung,
                Id_TacPham = idTacPham,
                Diem = 5,
                NgayTao = DateTime.Now,
                TrangThai = true
            });
            await _context.SaveChangesAsync();
        }
        // --- Start of code to add ---

        public async Task SuaBinhLuan(int idNguoiDung, int idBinhLuan, string noiDungMoi)
        {
            var binhLuan = await _context.BinhLuans.FirstOrDefaultAsync(b => b.Id == idBinhLuan && b.Id_NguoiDung == idNguoiDung);
            if (binhLuan == null)
            {
                throw new Exception("Không tìm thấy bình luận hoặc bạn không có quyền sửa bình luận này.");
            }

            binhLuan.NoiDung = noiDungMoi;
            await _context.SaveChangesAsync();
        }

        public async Task XoaBinhLuan(int idNguoiDung, int idBinhLuan)
        {
            var binhLuan = await _context.BinhLuans.Include(b => b.TacPham).FirstOrDefaultAsync(b => b.Id == idBinhLuan);
            if (binhLuan == null)
            {
                throw new Exception("Không tìm thấy bình luận.");
            }

            // Check if the user is the owner of the comment or the owner of the artwork.
            if (binhLuan.Id_NguoiDung != idNguoiDung && binhLuan.TacPham.Id_NguoiTao != idNguoiDung)
            {
                throw new Exception("Bạn không có quyền xóa bình luận này.");
            }

            binhLuan.TrangThai = false; // Soft delete
            await _context.SaveChangesAsync();
        }

        public async Task UpsertCamXuc(int idNguoiDung, int idTacPham, int idCamXuc)
        {
            var camXucHienTai = await _context.TacPham_CamXucs.FirstOrDefaultAsync(tc => tc.Id_TacPham == idTacPham && tc.Id_NguoiDung == idNguoiDung);

            if (camXucHienTai == null)
            {
                // No existing reaction, so add a new one
                _context.TacPham_CamXucs.Add(new TacPham_CamXuc
                {
                    Id_NguoiDung = idNguoiDung,
                    Id_TacPham = idTacPham,
                    Id_CamXuc = idCamXuc,
                    NgayTao = DateTime.Now,
                    TrangThai = true
                });
            }
            else
            {
                // Existing reaction found, update it
                camXucHienTai.Id_CamXuc = idCamXuc;
                camXucHienTai.TrangThai = true; // Ensure it's active
            }

            await _context.SaveChangesAsync();
        }

        public async Task XoaCamXuc(int idNguoiDung, int idTacPham)
        {
            var camXuc = await _context.TacPham_CamXucs.FirstOrDefaultAsync(tc => tc.Id_TacPham == idTacPham && tc.Id_NguoiDung == idNguoiDung);
            if (camXuc == null)
            {
                throw new Exception("Không tìm thấy cảm xúc để xóa.");
            }

            camXuc.TrangThai = false; // Soft delete
            await _context.SaveChangesAsync();
        }
        // --- End of code to add ---
    }
}
