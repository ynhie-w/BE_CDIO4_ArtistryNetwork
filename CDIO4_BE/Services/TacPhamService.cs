using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Repository;
using CDIO4_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CDIO4_BE.Helper;
namespace CDIO4_BE.Services
{

    public class TacPhamService : ITacPhamService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public TacPhamService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<TacPhamDto>> LayDanhSachTacPham(int trang, int soLuong)
        {
            var tacPhams = await _context.TacPhams
                .Include(tp => tp.NguoiTao)
                .Include(tp => tp.BinhLuans)
                .Include(tp => tp.TacPham_CamXucs)
                .Include(tp => tp.TheLoai)
                .OrderByDescending(tp => tp.NgayTao)
                .Skip((trang - 1) * soLuong)
                .Take(soLuong)
                .ToListAsync();

            return tacPhams.Select(tp => Mapper.MapToTacPhamDto(tp)).ToList();
        }

        public async Task<List<TacPhamDto>> LayDanhSachTacPhamNoiBat(int soLuong)
        {
            var tacPhams = await _context.TacPhams
                .Include(tp => tp.NguoiTao)
                .Include(tp => tp.BinhLuans)
                .Include(tp => tp.TacPham_CamXucs)
                .OrderByDescending(tp => tp.TacPham_CamXucs.Count(c => c.TrangThai))
                .ThenByDescending(tp => tp.LuotXem)
                .Take(soLuong)
                .ToListAsync();

            return tacPhams.Select(tp => Mapper.MapToTacPhamDto(tp)).ToList();
        }



        public async Task<int> TaoTacPham(TaoTacPhamDto dto, int userId)
        {
            var tacPham = new TacPham
            {
                Ten = dto.Ten,
                MoTa = dto.MoTa,
                DanhSachAnh = dto.Anh,
                Id_NguoiTao = userId,
                NgayTao = DateTime.UtcNow,
                Gia = dto.Gia,
                TrangThai = true
            };

            _context.TacPhams.Add(tacPham);
            await _context.SaveChangesAsync();
            return tacPham.Id;
        }

        public async Task<string> UploadAnhTacPham(int idTacPham, IFormFile file)
        {
            var tacPham = await _context.TacPhams.FindAsync(idTacPham);
            if (tacPham == null)
                throw new Exception("Không tìm thấy tác phẩm");

            if (file == null || file.Length == 0)
                throw new Exception("File ảnh không hợp lệ");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Lưu tên file vào DB
            tacPham.Anh = fileName;
            await _context.SaveChangesAsync();

            return fileName;
        }


        public async Task<bool> SuaTacPham(int id, SuaTacPhamDto dto, int userId)
        {
            var tacPham = await _context.TacPhams
                .FirstOrDefaultAsync(t => t.Id == id && t.Id_NguoiTao == userId);

            if (tacPham == null) return false;

            // cập nhật có điều kiện
            if (!string.IsNullOrWhiteSpace(dto.Ten)) tacPham.Ten = dto.Ten;
            if (!string.IsNullOrWhiteSpace(dto.MoTa)) tacPham.MoTa = dto.MoTa;
            if (dto.Anh != null && dto.Anh.Any()) tacPham.DanhSachAnh = dto.Anh;
            if (dto.Gia.HasValue) tacPham.Gia = dto.Gia.Value;
            if (dto.Id_TheLoai.HasValue) tacPham.Id_TheLoai = dto.Id_TheLoai;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> XoaTacPham(int id, int userId)
        {
            var tacPham = await _context.TacPhams
                .FirstOrDefaultAsync(t => t.Id == id && t.Id_NguoiTao == userId);

            if (tacPham == null) return false;

            // Soft delete (ẩn thay vì xóa hẳn)
            tacPham.TrangThai = false;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<TacPhamDto?> LayChiTietTacPham(int id)
        {
            var t = await _context.TacPhams
                .Include(tp => tp.NguoiTao)
                .Include(tp => tp.TacPham_CamXucs).ThenInclude(tc => tc.CamXuc)
                .Include(tp => tp.BinhLuans)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (t == null) return null;

            // Tăng lượt xem
            t.LuotXem += 1;
            await _context.SaveChangesAsync();

            // Lấy bình luận + trả lời (có thể override số lượng bình luận)
            var binhLuans = await LayBinhLuanVaTraLoi(t.Id);

            // Lấy userId từ token
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdStr = user?.FindFirstValue("userId");
            int? Id_NguoiDung = int.TryParse(userIdStr, out var parsedId) ? parsedId : (int?)null;

            // Dùng Mapper
            var dto = Mapper.MapToTacPhamDto(t, Id_NguoiDung);

            // Override lại số lượng bình luận bằng list đã load kèm trả lời
            dto.ThongKe.LuotBinhLuan = binhLuans.Count;

            return dto;
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
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            int? currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : (int?)null;

            var list = await _context.TacPhams
                .Include(tp => tp.NguoiTao)
                .Include(tp => tp.BinhLuans)
                .Include(tp => tp.TacPham_CamXucs).ThenInclude(tc => tc.CamXuc)
                .Where(tp => tp.Id_NguoiTao == idNguoiDung)
                .OrderByDescending(tp => tp.NgayTao)
                .ToListAsync();

            return list.Select(tp => Mapper.MapToTacPhamDto(tp, currentUserId)).ToList();
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

        public async Task ThemSuaCamXuc(int userId, int idTacPham, CamXucRequest cx)
        {
            var existing = await _context.TacPham_CamXucs
                .FirstOrDefaultAsync(tc => tc.Id_NguoiDung == userId && tc.Id_TacPham == idTacPham);

            if (existing == null)
            {
                var newEntity = new TacPham_CamXuc
                {
                    Id_NguoiDung = userId,
                    Id_TacPham = idTacPham,
                    Id_CamXuc = cx.IdCamXuc, 
                    NgayTao = DateTime.Now,
                    TrangThai = true
                };
                _context.TacPham_CamXucs.Add(newEntity);
            }
            else
            {
                existing.Id_CamXuc = cx.IdCamXuc;
                existing.NgayTao = DateTime.Now;
                existing.TrangThai = true;
            }

            await _context.SaveChangesAsync();
        }


        public async Task<TacPham_CamXuc?> GetCamXuc(int userId, int idTacPham)
        {
            return await _context.TacPham_CamXucs
                .Include(tc => tc.CamXuc)
                .FirstOrDefaultAsync(tc => tc.Id_NguoiDung == userId && tc.Id_TacPham == idTacPham && tc.TrangThai);
        }

        public async Task<TacPham_CamXuc?> LayCamXuc(int userId, int idTacPham)
        {
            return await _context.TacPham_CamXucs
                .Include(tc => tc.CamXuc)
                .FirstOrDefaultAsync(tc => tc.Id_NguoiDung == userId && tc.Id_TacPham == idTacPham && tc.TrangThai);
        }

        public async Task XoaCamXuc(int userId, int idTacPham)
        {
            var existing = await _context.TacPham_CamXucs
                .FirstOrDefaultAsync(tc => tc.Id_NguoiDung == userId && tc.Id_TacPham == idTacPham);

            if (existing != null)
            {
                existing.TrangThai = false; // soft delete
                await _context.SaveChangesAsync();
            }
        }
    }
}
