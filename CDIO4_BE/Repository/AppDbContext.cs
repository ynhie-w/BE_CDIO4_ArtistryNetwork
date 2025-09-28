using CDIO4_BE.Domain.DTOs;
using CDIO4_BE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CDIO4_BE.Repository
{
    public static class SqlServerIdentityColumnTriggerExtensions
    {
        public static EntityTypeBuilder<TEntity> UseSqlServerIdentityColumnTriggerWorkaround<TEntity>(
            this EntityTypeBuilder<TEntity> builder) where TEntity : class
        {
            builder.Property<int>("__ef_tmp_id")
                .Metadata.SetAfterSaveBehavior(
                    Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

            return builder;
        }
    }
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Đã xóa null! khỏi các DbSet không cần thiết nếu bạn đã cấu hình chúng đúng cách
        // hoặc nếu chúng không thể null.
        public DbSet<NguoiDung> NguoiDungs { get; set; } = null!;
        public DbSet<TacPham> TacPhams { get; set; } = null!;
        public DbSet<GioHang> GioHangs { get; set; } = null!;
        public DbSet<BoSuuTap> BoSuuTaps { get; set; } = null!;
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

        public DbSet<DatLaiMatKhauToken> DatLaiMatKhauTokens { get; set; } = null!; // Thêm = null!
        public DbSet<GiamGia> GiamGias { get; set; } = null!; // Thêm = null!


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Gọi base class trước khi cấu hình các entity của bạn
            base.OnModelCreating(modelBuilder);

            // ------------------ Composite Keys ------------------
            modelBuilder.Entity<GioHang>().HasKey(g => new { g.Id_NguoiMua, g.Id_TacPham });
            modelBuilder.Entity<BoSuuTap>().HasKey(b => new { b.Id_NguoiDung, b.Id_TacPham });
            modelBuilder.Entity<ThamGiaDuAn>().HasKey(tg => new { tg.Id_DuAn, tg.Id_NguoiDung });
            modelBuilder.Entity<TacPham_Hashtags>().HasKey(th => new { th.Id_TacPham, th.Id_Hashtag });
            modelBuilder.Entity<DuAn_TacPham>().HasKey(dt => new { dt.Id_DuAn, dt.Id_TacPham });
            modelBuilder.Entity<DonHang_ChiTiet>().HasKey(dhct => new { dhct.Id_DonHang, dhct.Id_TacPham });

            // Cần quyết định khóa chính của TacPham_CamXuc là gì:
            // Phương án 1: Khóa chính bao gồm Id_NguoiDung, Id_TacPham, Id_CamXuc (nếu một người có thể có nhiều cảm xúc cho 1 tác phẩm)
            // modelBuilder.Entity<TacPham_CamXuc>().HasKey(tc => new { tc.Id_NguoiDung, tc.Id_TacPham, tc.Id_CamXuc });
            // Phương án 2: Khóa chính chỉ bao gồm Id_NguoiDung và Id_TacPham, đảm bảo mỗi user chỉ có MỘT cảm xúc cho MỘT tác phẩm (phù hợp với IsUnique bên dưới)
            modelBuilder.Entity<TacPham_CamXuc>()
                 .HasKey(tc => new { tc.Id_NguoiDung, tc.Id_TacPham }); // Chọn phương án này nếu một user chỉ có 1 cảm xúc/tác phẩm


            // Unique: 1 user chỉ có 1 cảm xúc duy nhất cho tác phẩm đó
            // Điều này có vẻ phù hợp với phương án 2 của khóa chính trên
            modelBuilder.Entity<TacPham_CamXuc>()
                .HasIndex(tc => new { tc.Id_NguoiDung, tc.Id_TacPham })
                .IsUnique();

            modelBuilder.Entity<GiamGia>().ToTable("GiamGia"); // Mapping tên bảng nếu cần

            // Cấu hình NguoiDung
            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.ToTable("NGUOIDUNG"); // Xóa ExcludeFromMigrations nếu đây là bảng chính
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd() // Id sinh tự động (identity)
                    .UseIdentityColumn();  // SQL Server identity
                // Nếu cần dùng workaround:
                // .UseSqlServerIdentityColumnTriggerWorkaround();
            });


            // ------------------ Relationships ------------------

            // GioHang
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

            // BoSuuTap
            modelBuilder.Entity<BoSuuTap>()
                .HasOne(b => b.NguoiDung)
                .WithMany(u => u.BoSuuTaps)
                .HasForeignKey(b => b.Id_NguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BoSuuTap>()
                .HasOne(b => b.TacPham)
                .WithMany(tp => tp.BoSuuTaps)
                .HasForeignKey(b => b.Id_TacPham)
                .OnDelete(DeleteBehavior.Cascade);

            // DonHang_ChiTiet
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

            // BinhLuan
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

            // TacPham_CamXuc (Relationships)
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

            modelBuilder.Entity<TacPham_CamXuc>()
                .HasOne(tc => tc.CamXuc)
                .WithMany(cx => cx.TacPham_CamXucs)
                .HasForeignKey(tc => tc.Id_CamXuc)
                .OnDelete(DeleteBehavior.Restrict); // Rất quan trọng: Không xóa cảm xúc nếu có người dùng nó

            // ThongBao
            modelBuilder.Entity<ThongBao>()
                .HasOne(tb => tb.NguoiDung)
                .WithMany(u => u.ThongBaos)
                .HasForeignKey(tb => tb.Id_NguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // ThamGiaDuAn
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

            // TacPham_Hashtags
            modelBuilder.Entity<TacPham_Hashtags>()
                .HasOne(th => th.TacPham)
                .WithMany(tp => tp.TacPham_Hashtags)
                .HasForeignKey(th => th.Id_TacPham)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TacPham_Hashtags>()
                .HasOne(th => th.Hashtag)
                .WithMany(h => h.TacPham_Hashtags)
                .HasForeignKey(th => th.Id_Hashtag)
                .OnDelete(DeleteBehavior.Restrict);

            // DuAn_TacPham
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

            // DanhGia
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

            // TacPham - NguoiTao
            modelBuilder.Entity<TacPham>()
                .HasOne(tp => tp.NguoiTao)
                .WithMany(u => u.TacPhams)
                .HasForeignKey(tp => tp.Id_NguoiTao)
                .OnDelete(DeleteBehavior.Restrict); // Giữ lại người tạo ngay cả khi tác phẩm bị xóa

            // HoaDon - HoaDon_ChiTiet
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

            // NguoiDung - Quyen / CapDo
            modelBuilder.Entity<NguoiDung>()
                .HasOne(nd => nd.Quyen)
                .WithMany(q => q.NguoiDungs)
                .HasForeignKey(nd => nd.Id_PhanQuyen)
                .OnDelete(DeleteBehavior.Restrict); // Rất quan trọng: Không xóa quyền nếu có người dùng

            modelBuilder.Entity<NguoiDung>()
                .HasOne(nd => nd.CapDo)
                .WithMany(c => c.NguoiDungs)
                .HasForeignKey(nd => nd.Id_CapDo)
                .OnDelete(DeleteBehavior.Restrict); // Rất quan trọng: Không xóa cấp độ nếu có người dùng

            // DonHang - GiamGia
            modelBuilder.Entity<DonHang>()
                .HasOne(dh => dh.GiamGias) // GiamGias là navigation property đến GiamGia entity
                .WithMany(g => g.DonHangs)
                .HasForeignKey(dh => dh.IdGiamGia)
                .IsRequired(false) // Allow null if a discount is optional
                .OnDelete(DeleteBehavior.SetNull); // Nếu giảm giá bị xóa, IdGiamGia trong DonHang sẽ là null
        }
    }
}