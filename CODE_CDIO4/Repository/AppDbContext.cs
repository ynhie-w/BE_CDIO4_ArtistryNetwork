    using CODE_CDIO4.Models;
    using Microsoft.EntityFrameworkCore;

    namespace CODE_CDIO4.Repository
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options) { }


            public DbSet<NguoiDung> NguoiDungs { get; set; } = null!;
            public DbSet<TacPham> TacPhams { get; set; } = null!;
            public DbSet<GioHang> GioHangs { get; set; } = null!;
            public DbSet<DonHang> DonHangs { get; set; } = null!;
            public DbSet<DonHang_ChiTiet> DonHang_ChiTiets { get; set; } = null!;
            public DbSet<ThanhToan> ThanhToans { get; set; } = null!;
            public DbSet<BinhLuan> BinhLuans { get; set; } = null!;
            public DbSet<CamXuc> CamXucs { get; set; } = null!;
            public DbSet<ThongBao> ThongBaos { get; set; } = null!;
            public DbSet<DuAnCongDong> DuAnCongDongs { get; set; } = null!;
            public DbSet<ThamGiaDuAn> ThamGiaDuAns { get; set; } = null!;
            public DbSet<Hashtag> Hashtags { get; set; } = null!;
            public DbSet<TacPham_Hashtags> TacPham_Hashtags { get; set; } = null!;
            public DbSet<TheLoai> TheLoais { get; set; } = null!;
            public DbSet<DuAn_TacPham> DuAn_TacPhams { get; set; } = null!;
            public DbSet<DanhGia> DanhGias { get; set; } = null!;
            public DbSet<Quyen> Quyens { get; set; } = null!;
            public DbSet<CapDo> CapDos { get; set; } = null!;
            public DbSet<HoaDon> HoaDons { get; set; } = null!;
            public DbSet<HoaDon_ChiTiet> HoaDon_ChiTiets { get; set; } = null!;
            public DbSet<TacPham_CamXuc> TacPham_CamXucs { get; set; } = null!;


            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Composite Keys for Many-to-Many Relationships and Dependent Entities
                modelBuilder.Entity<GioHang>().HasKey(g => new { g.Id_NguoiMua, g.Id_TacPham });
                modelBuilder.Entity<ThamGiaDuAn>().HasKey(tg => new { tg.Id_DuAn, tg.Id_NguoiDung });
                modelBuilder.Entity<TacPham_Hashtags>().HasKey(th => new { th.Id_TacPham, th.Id_Hashtag });
                modelBuilder.Entity<DuAn_TacPham>().HasKey(dt => new { dt.Id_DuAn, dt.Id_TacPham });
                modelBuilder.Entity<DonHang_ChiTiet>().HasKey(dhct => new { dhct.Id_DonHang, dhct.Id_TacPham });
            modelBuilder.Entity<TacPham_CamXuc>().HasKey(tc => new { tc.Id_NguoiDung, tc.Id_TacPham, tc.Id_CamXuc });

            // ------------------ Relationships ------------------

            // ==================== GioHang (N-N) ====================
            modelBuilder.Entity<GioHang>()
                    .HasOne(g => g.NguoiMua)
                    .WithMany(u => u.GioHangs)
                    .HasForeignKey(g => g.Id_NguoiMua)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<GioHang>()
                    .HasOne(g => g.TacPham)
                    .WithMany(tp => tp.GioHangs)
                    .HasForeignKey(g => g.Id_TacPham)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==================== DonHang - DonHang_ChiTiet ====================
                modelBuilder.Entity<DonHang_ChiTiet>()
                    .HasOne(ct => ct.DonHang)
                    .WithMany(dh => dh.DonHang_ChiTiets)
                    .HasForeignKey(ct => ct.Id_DonHang)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<DonHang_ChiTiet>()
                    .HasOne(ct => ct.TacPham)
                    .WithMany(tp => tp.DonHang_ChiTiets)
                    .HasForeignKey(ct => ct.Id_TacPham)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==================== BinhLuan ====================
                modelBuilder.Entity<BinhLuan>()
                    .HasOne(bl => bl.TacPham)
                    .WithMany(tp => tp.BinhLuans)
                    .HasForeignKey(bl => bl.Id_TacPham)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<BinhLuan>()
                    .HasOne(bl => bl.NguoiBinhLuan)
                    .WithMany(u => u.BinhLuans)
                    .HasForeignKey(bl => bl.Id_NguoiDung)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==================== TacPham_CamXuc ====================
                // Note: This replaces the CamXuc entity configuration
                modelBuilder.Entity<TacPham_CamXuc>()
                    .HasOne(tc => tc.NguoiDung)
                    .WithMany(u => u.TacPham_CamXucs)
                    .HasForeignKey(tc => tc.Id_NguoiDung)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<TacPham_CamXuc>()
                    .HasOne(tc => tc.TacPham)
                    .WithMany(tp => tp.TacPham_CamXucs)
                    .HasForeignKey(tc => tc.Id_TacPham)
                    .OnDelete(DeleteBehavior.Cascade);
            // Thêm cấu hình cho mối quan hệ với CamXuc
            modelBuilder.Entity<TacPham_CamXuc>()
                .HasOne(tc => tc.CamXuc)
                .WithMany(cx => cx.TacPham_CamXucs) 
                .HasForeignKey(tc => tc.Id_CamXuc)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== ThongBao ====================
            modelBuilder.Entity<ThongBao>()
                    .HasOne(tb => tb.NguoiDung)
                    .WithMany(u => u.ThongBaos)
                    .HasForeignKey(tb => tb.Id_NguoiDung)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==================== ThamGiaDuAn (N-N) ====================
                modelBuilder.Entity<ThamGiaDuAn>()
                    .HasOne(tg => tg.DuAnCongDong)
                    .WithMany(da => da.ThamGiaDuAns)
                    .HasForeignKey(tg => tg.Id_DuAn)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<ThamGiaDuAn>()
                    .HasOne(tg => tg.NguoiDung)
                    .WithMany(u => u.ThamGiaDuAns)
                    .HasForeignKey(tg => tg.Id_NguoiDung)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==================== TacPham_Hashtags (N-N) ====================
                modelBuilder.Entity<TacPham_Hashtags>()
                    .HasOne(th => th.TacPham)
                    .WithMany(tp => tp.TacPham_Hashtags)
                    .HasForeignKey(th => th.Id_TacPham)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<TacPham_Hashtags>()
                    .HasOne(th => th.Hashtag)
                    .WithMany(h => h.TacPham_hashtags)
                    .HasForeignKey(th => th.Id_Hashtag)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==================== DuAn_TacPham (N-N) ====================
                modelBuilder.Entity<DuAn_TacPham>()
                    .HasOne(dt => dt.DuAn)
                    .WithMany(da => da.DuAn_TacPhams)
                    .HasForeignKey(dt => dt.Id_DuAn)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<DuAn_TacPham>()
                    .HasOne(dt => dt.TacPham)
                    .WithMany(tp => tp.DuAn_TacPhams)
                    .HasForeignKey(dt => dt.Id_TacPham)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==================== DanhGia ====================
                modelBuilder.Entity<DanhGia>()
                    .HasOne(dg => dg.NguoiDanhGia)
                    .WithMany(nd => nd.DanhGias)
                    .HasForeignKey(dg => dg.Id_NguoiDung)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<DanhGia>()
                    .HasOne(dg => dg.TacPham)
                    .WithMany(tp => tp.DanhGias)
                    .HasForeignKey(dg => dg.Id_TacPham)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==================== TacPham - NguoiTao ====================
                modelBuilder.Entity<TacPham>()
                    .HasOne(tp => tp.NguoiTao)
                    .WithMany(u => u.TacPhams)
                    .HasForeignKey(tp => tp.Id_NguoiTao)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==================== HoaDon - HoaDon_ChiTiet ====================
                modelBuilder.Entity<HoaDon_ChiTiet>()
                    .HasOne(hdct => hdct.HoaDon)
                    .WithMany(hd => hd.HoaDon_ChiTiets)
                    .HasForeignKey(hdct => hdct.Id_HoaDon)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<HoaDon_ChiTiet>()
                    .HasOne(hdct => hdct.TacPham)
                    .WithMany(tp => tp.HoaDon_ChiTiets)
                    .HasForeignKey(hdct => hdct.Id_TacPham)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==================== NguoiDung - Quyen / CapDo ====================
                modelBuilder.Entity<NguoiDung>()
                    .HasOne(nd => nd.Quyen)
                    .WithMany(q => q.NguoiDungs)
                    .HasForeignKey(nd => nd.Id_PhanQuyen)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<NguoiDung>()
                    .HasOne(nd => nd.CapDo)
                    .WithMany(c => c.NguoiDungs)
                    .HasForeignKey(nd => nd.Id_CapDo)
                    .OnDelete(DeleteBehavior.Restrict);

                // Note: The ThanhToan entity lacks a relationship to NguoiDung or a related entity.
                // It's linked to DonHang, which is fine, but it might be useful to link it to the user who paid.
            }
    
        }
    }