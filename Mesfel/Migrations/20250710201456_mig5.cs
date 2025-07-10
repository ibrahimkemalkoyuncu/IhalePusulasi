using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mesfel.Migrations
{
    /// <inheritdoc />
    public partial class mig5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ihaleler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IhaleNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IhaleAdi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IhaleKurumu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    KesifBedeli = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IhaleBaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IhaleBitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IhaleTuru = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IhaleLinki = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IhaleNumarasi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IletisimBilgileri = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IhaleUsulu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IhaleDurumu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Aktif"),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KaydedenKullanici = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GuncelleyenKullanici = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    YaklasikMaliyet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TeklifSonTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ihaleler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KategoriAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    SifreHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Kullanici"),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IhaleAnalizleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IhaleId = table.Column<int>(type: "int", nullable: false),
                    AnalizTipi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sonuc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AnalizTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Puan = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToplamTeklifSayisi = table.Column<int>(type: "int", nullable: false),
                    OrtalamaTeklif = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnDusukTeklif = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EnYuksekTeklif = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    KazananTeklif = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RekabetOrani = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TasarrufOrani = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    StandartSapma = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TahminEdilenKazananTeklif = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RekabetSeviyesi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IhaleAnalizleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IhaleAnalizleri_Ihaleler_IhaleId",
                        column: x => x.IhaleId,
                        principalTable: "Ihaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IhaleDetaylari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IhaleId = table.Column<int>(type: "int", nullable: false),
                    KalemAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Miktar = table.Column<int>(type: "int", nullable: false),
                    Birim = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IhaleDetaylari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IhaleDetaylari_Ihaleler_IhaleId",
                        column: x => x.IhaleId,
                        principalTable: "Ihaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IhaleKalemleri",
                columns: table => new
                {
                    IhaleKalemiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IhaleId = table.Column<int>(type: "int", nullable: false),
                    KalemAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Birim = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IhaleKalemleri", x => x.IhaleKalemiId);
                    table.ForeignKey(
                        name: "FK_IhaleKalemleri_Ihaleler_IhaleId",
                        column: x => x.IhaleId,
                        principalTable: "Ihaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IhaleKategorileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IhaleId = table.Column<int>(type: "int", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: false),
                    AnaKategori = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IhaleKategorileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IhaleKategorileri_Ihaleler_IhaleId",
                        column: x => x.IhaleId,
                        principalTable: "Ihaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IhaleKategorileri_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IhaleTeklifleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IhaleId = table.Column<int>(type: "int", nullable: false),
                    FirmaAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VergiNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    TeklifTutari = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KdvTutari = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TeklifDurumu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "VERILDI"),
                    TeklifTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GecerliTeklif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IhaleAnalizId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IhaleTeklifleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IhaleTeklifleri_IhaleAnalizleri_IhaleAnalizId",
                        column: x => x.IhaleAnalizId,
                        principalTable: "IhaleAnalizleri",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IhaleTeklifleri_Ihaleler_IhaleId",
                        column: x => x.IhaleId,
                        principalTable: "Ihaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IhaleAnalizleri_IhaleId",
                table: "IhaleAnalizleri",
                column: "IhaleId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleDetaylari_IhaleId",
                table: "IhaleDetaylari",
                column: "IhaleId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleKalemleri_IhaleId",
                table: "IhaleKalemleri",
                column: "IhaleId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleKategorileri_IhaleId",
                table: "IhaleKategorileri",
                column: "IhaleId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleKategorileri_KategoriId",
                table: "IhaleKategorileri",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleTeklifleri_IhaleAnalizId",
                table: "IhaleTeklifleri",
                column: "IhaleAnalizId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleTeklifleri_IhaleId",
                table: "IhaleTeklifleri",
                column: "IhaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_Email",
                table: "Kullanicilar",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar",
                column: "KullaniciAdi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IhaleDetaylari");

            migrationBuilder.DropTable(
                name: "IhaleKalemleri");

            migrationBuilder.DropTable(
                name: "IhaleKategorileri");

            migrationBuilder.DropTable(
                name: "IhaleTeklifleri");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropTable(
                name: "IhaleAnalizleri");

            migrationBuilder.DropTable(
                name: "Ihaleler");
        }
    }
}
