using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CODE_CDIO4.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HASHTAGS",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HASHTAGS", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MAUSAC",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAUSAC", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "NGUOIDUNG",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    sdt = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    matkhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    diemthuong = table.Column<int>(type: "int", nullable: false),
                    capdo = table.Column<int>(type: "int", nullable: false),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NGUOIDUNG", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "THELOAI",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THELOAI", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "DONHANG",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_nguoimua = table.Column<int>(type: "int", nullable: false),
                    NgayMua = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiamGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DONHANG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DONHANG_NGUOIDUNG_id_nguoimua",
                        column: x => x.id_nguoimua,
                        principalTable: "NGUOIDUNG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DUANCONGDONG",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenduan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_quanly = table.Column<int>(type: "int", nullable: false),
                    trangthai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DUANCONGDONG", x => x.id);
                    table.ForeignKey(
                        name: "FK_DUANCONGDONG_NGUOIDUNG_id_quanly",
                        column: x => x.id_quanly,
                        principalTable: "NGUOIDUNG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THONGBAO",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_nguoidung = table.Column<int>(type: "int", nullable: false),
                    noidung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dadoc = table.Column<bool>(type: "bit", nullable: false),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THONGBAO", x => x.id);
                    table.ForeignKey(
                        name: "FK_THONGBAO_NGUOIDUNG_id_nguoidung",
                        column: x => x.id_nguoidung,
                        principalTable: "NGUOIDUNG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TACPHAM",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    duongdan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    id_theloai = table.Column<int>(type: "int", nullable: true),
                    id_mausac = table.Column<int>(type: "int", nullable: true),
                    trangthai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    id_nguoitao = table.Column<int>(type: "int", nullable: false),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TACPHAM", x => x.id);
                    table.ForeignKey(
                        name: "FK_TACPHAM_MAUSAC_id_mausac",
                        column: x => x.id_mausac,
                        principalTable: "MAUSAC",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_TACPHAM_NGUOIDUNG_id_nguoitao",
                        column: x => x.id_nguoitao,
                        principalTable: "NGUOIDUNG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TACPHAM_THELOAI_id_theloai",
                        column: x => x.id_theloai,
                        principalTable: "THELOAI",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "THANHTOAN",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_donhang = table.Column<int>(type: "int", nullable: false),
                    phuongthuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    trangthai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ngaytt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THANHTOAN", x => x.id);
                    table.ForeignKey(
                        name: "FK_THANHTOAN_DONHANG_id_donhang",
                        column: x => x.id_donhang,
                        principalTable: "DONHANG",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THAMGIADUAN",
                columns: table => new
                {
                    id_duan = table.Column<int>(type: "int", nullable: false),
                    id_nguoidung = table.Column<int>(type: "int", nullable: false),
                    vaitro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ngaythamgia = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THAMGIADUAN", x => new { x.id_duan, x.id_nguoidung });
                    table.ForeignKey(
                        name: "FK_THAMGIADUAN_DUANCONGDONG_id_duan",
                        column: x => x.id_duan,
                        principalTable: "DUANCONGDONG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THAMGIADUAN_NGUOIDUNG_id_nguoidung",
                        column: x => x.id_nguoidung,
                        principalTable: "NGUOIDUNG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BINHLUAN",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_tacpham = table.Column<int>(type: "int", nullable: false),
                    id_nguoidung = table.Column<int>(type: "int", nullable: false),
                    noidung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BINHLUAN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BINHLUAN_NGUOIDUNG_id_nguoidung",
                        column: x => x.id_nguoidung,
                        principalTable: "NGUOIDUNG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BINHLUAN_TACPHAM_id_tacpham",
                        column: x => x.id_tacpham,
                        principalTable: "TACPHAM",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAMXUC",
                columns: table => new
                {
                    id_nguoidung = table.Column<int>(type: "int", nullable: false),
                    id_tacpham = table.Column<int>(type: "int", nullable: false),
                    CamXucValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAMXUC", x => new { x.id_nguoidung, x.id_tacpham });
                    table.ForeignKey(
                        name: "FK_CAMXUC_NGUOIDUNG_id_nguoidung",
                        column: x => x.id_nguoidung,
                        principalTable: "NGUOIDUNG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CAMXUC_TACPHAM_id_tacpham",
                        column: x => x.id_tacpham,
                        principalTable: "TACPHAM",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DANHGIA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_tacpham = table.Column<int>(type: "int", nullable: false),
                    id_nguoidung = table.Column<int>(type: "int", nullable: false),
                    Diem = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANHGIA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DANHGIA_NGUOIDUNG_id_nguoidung",
                        column: x => x.id_nguoidung,
                        principalTable: "NGUOIDUNG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DANHGIA_TACPHAM_id_tacpham",
                        column: x => x.id_tacpham,
                        principalTable: "TACPHAM",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DONHANG_CHITIET",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_donhang = table.Column<int>(type: "int", nullable: false),
                    id_tacpham = table.Column<int>(type: "int", nullable: false),
                    id_donhang1 = table.Column<int>(type: "int", nullable: true),
                    id_tacpham1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DONHANG_CHITIET", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DONHANG_CHITIET_DONHANG_id_donhang",
                        column: x => x.id_donhang,
                        principalTable: "DONHANG",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DONHANG_CHITIET_TACPHAM_id_tacpham",
                        column: x => x.id_tacpham,
                        principalTable: "TACPHAM",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DUAN_TACPHAM",
                columns: table => new
                {
                    id_duan = table.Column<int>(type: "int", nullable: false),
                    id_tacpham = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DUAN_TACPHAM", x => new { x.id_duan, x.id_tacpham });
                    table.ForeignKey(
                        name: "FK_DUAN_TACPHAM_DUANCONGDONG_id_duan",
                        column: x => x.id_duan,
                        principalTable: "DUANCONGDONG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DUAN_TACPHAM_TACPHAM_id_tacpham",
                        column: x => x.id_tacpham,
                        principalTable: "TACPHAM",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GIOHANG",
                columns: table => new
                {
                    id_nguoimua = table.Column<int>(type: "int", nullable: false),
                    id_tacpham = table.Column<int>(type: "int", nullable: false),
                    NgayThem = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GIOHANG", x => new { x.id_nguoimua, x.id_tacpham });
                    table.ForeignKey(
                        name: "FK_GIOHANG_NGUOIDUNG_id_nguoimua",
                        column: x => x.id_nguoimua,
                        principalTable: "NGUOIDUNG",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GIOHANG_TACPHAM_id_tacpham",
                        column: x => x.id_tacpham,
                        principalTable: "TACPHAM",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TACPHAM_HASHTAGS",
                columns: table => new
                {
                    id_tacpham = table.Column<int>(type: "int", nullable: false),
                    id_hashtag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TACPHAM_HASHTAGS", x => new { x.id_tacpham, x.id_hashtag });
                    table.ForeignKey(
                        name: "FK_TACPHAM_HASHTAGS_HASHTAGS_id_hashtag",
                        column: x => x.id_hashtag,
                        principalTable: "HASHTAGS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TACPHAM_HASHTAGS_TACPHAM_id_tacpham",
                        column: x => x.id_tacpham,
                        principalTable: "TACPHAM",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BINHLUAN_id_nguoidung",
                table: "BINHLUAN",
                column: "id_nguoidung");

            migrationBuilder.CreateIndex(
                name: "IX_BINHLUAN_id_tacpham",
                table: "BINHLUAN",
                column: "id_tacpham");

            migrationBuilder.CreateIndex(
                name: "IX_CAMXUC_id_tacpham",
                table: "CAMXUC",
                column: "id_tacpham");

            migrationBuilder.CreateIndex(
                name: "IX_DANHGIA_id_nguoidung",
                table: "DANHGIA",
                column: "id_nguoidung");

            migrationBuilder.CreateIndex(
                name: "IX_DANHGIA_id_tacpham",
                table: "DANHGIA",
                column: "id_tacpham");

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_id_nguoimua",
                table: "DONHANG",
                column: "id_nguoimua");

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_CHITIET_id_donhang",
                table: "DONHANG_CHITIET",
                column: "id_donhang");

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_CHITIET_id_tacpham",
                table: "DONHANG_CHITIET",
                column: "id_tacpham");

            migrationBuilder.CreateIndex(
                name: "IX_DUAN_TACPHAM_id_tacpham",
                table: "DUAN_TACPHAM",
                column: "id_tacpham");

            migrationBuilder.CreateIndex(
                name: "IX_DUANCONGDONG_id_quanly",
                table: "DUANCONGDONG",
                column: "id_quanly");

            migrationBuilder.CreateIndex(
                name: "IX_GIOHANG_id_tacpham",
                table: "GIOHANG",
                column: "id_tacpham");

            migrationBuilder.CreateIndex(
                name: "IX_TACPHAM_id_mausac",
                table: "TACPHAM",
                column: "id_mausac");

            migrationBuilder.CreateIndex(
                name: "IX_TACPHAM_id_nguoitao",
                table: "TACPHAM",
                column: "id_nguoitao");

            migrationBuilder.CreateIndex(
                name: "IX_TACPHAM_id_theloai",
                table: "TACPHAM",
                column: "id_theloai");

            migrationBuilder.CreateIndex(
                name: "IX_TACPHAM_HASHTAGS_id_hashtag",
                table: "TACPHAM_HASHTAGS",
                column: "id_hashtag");

            migrationBuilder.CreateIndex(
                name: "IX_THAMGIADUAN_id_nguoidung",
                table: "THAMGIADUAN",
                column: "id_nguoidung");

            migrationBuilder.CreateIndex(
                name: "IX_THANHTOAN_id_donhang",
                table: "THANHTOAN",
                column: "id_donhang");

            migrationBuilder.CreateIndex(
                name: "IX_THONGBAO_id_nguoidung",
                table: "THONGBAO",
                column: "id_nguoidung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BINHLUAN");

            migrationBuilder.DropTable(
                name: "CAMXUC");

            migrationBuilder.DropTable(
                name: "DANHGIA");

            migrationBuilder.DropTable(
                name: "DONHANG_CHITIET");

            migrationBuilder.DropTable(
                name: "DUAN_TACPHAM");

            migrationBuilder.DropTable(
                name: "GIOHANG");

            migrationBuilder.DropTable(
                name: "TACPHAM_HASHTAGS");

            migrationBuilder.DropTable(
                name: "THAMGIADUAN");

            migrationBuilder.DropTable(
                name: "THANHTOAN");

            migrationBuilder.DropTable(
                name: "THONGBAO");

            migrationBuilder.DropTable(
                name: "HASHTAGS");

            migrationBuilder.DropTable(
                name: "TACPHAM");

            migrationBuilder.DropTable(
                name: "DUANCONGDONG");

            migrationBuilder.DropTable(
                name: "DONHANG");

            migrationBuilder.DropTable(
                name: "MAUSAC");

            migrationBuilder.DropTable(
                name: "THELOAI");

            migrationBuilder.DropTable(
                name: "NGUOIDUNG");
        }
    }
}
