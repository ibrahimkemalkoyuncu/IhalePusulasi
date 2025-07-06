using Mesfel.Models;
using Mesfel.Sabitler;
using Microsoft.EntityFrameworkCore;

namespace Mesfel.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet tanımları - Veritabanı tablolarını temsil eder
        public DbSet<Ihale> Ihaleler { get; set; }
        public DbSet<IhaleKalemi> IhaleKalemleri { get; set; }
        public DbSet<Teklif> Teklifler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // İhale tablosu konfigürasyonu
            modelBuilder.Entity<Ihale>(entity =>
            {
                entity.HasKey(e => e.IhaleId);
                entity.Property(e => e.IhaleAdi).IsRequired().HasMaxLength(200);
                entity.Property(e => e.IhaleKodu).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IhaleKonusu).IsRequired().HasMaxLength(500);
                entity.Property(e => e.YaklasikMaliyet).HasColumnType("decimal(18,2)");
                entity.Property(e => e.GeciciTeminat).HasColumnType("decimal(18,2)");

                // Index tanımları - Performans için
                entity.HasIndex(e => e.IhaleKodu).IsUnique();
                entity.HasIndex(e => e.IhaleTarihi);
                entity.HasIndex(e => e.IhaleDurumu);
            });

            // İhale Kalemi tablosu konfigürasyonu
            modelBuilder.Entity<IhaleKalemi>(entity =>
            {
                entity.HasKey(e => e.IhaleKalemiId);
                entity.Property(e => e.KalemAdi).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Birim).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Miktar).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BirimFiyat).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Aciklama).HasMaxLength(500);

                // İhale ile ilişki kurma
                entity.HasOne(e => e.Ihale)
                      .WithMany(i => i.IhaleKalemleri)
                      .HasForeignKey(e => e.IhaleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Teklif tablosu konfigürasyonu
            modelBuilder.Entity<Teklif>(entity =>
            {
                entity.HasKey(e => e.TeklifId);
                entity.Property(e => e.FirmaAdi).IsRequired().HasMaxLength(200);
                entity.Property(e => e.VergiNumarasi).IsRequired().HasMaxLength(20);
                entity.Property(e => e.TeklifTutari).HasColumnType("decimal(18,2)");
                entity.Property(e => e.KdvTutari).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Aciklama).HasMaxLength(500);

                // İhale ile ilişki kurma
                entity.HasOne(e => e.Ihale)
                      .WithMany(i => i.Teklifler)
                      .HasForeignKey(e => e.IhaleId)
                      .OnDelete(DeleteBehavior.Cascade);

                // İndex tanımları
                entity.HasIndex(e => e.VergiNumarasi);
                entity.HasIndex(e => e.TeklifTarihi);
            });

            // Başlangıç verisi ekleme
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Örnek ihale verisi
            modelBuilder.Entity<Ihale>().HasData(
                new Ihale
                {
                    IhaleId = 1,
                    IhaleAdi = "Okul Binası Yapımı",
                    IhaleKodu = "2025-001",
                    IhaleTuru = IhaleTuru.YapimIsi,
                    IhaleUsulu = IhaleUsulu.AcikIhale,
                    IhaleKonusu = "İlkokul binası yapım işi",
                    YaklasikMaliyet = 5000000.00m,
                    GeciciTeminat = 75000.00m,
                    KesinTeminatOrani = 6.0,
                    IhaleTarihi = DateTime.Now.AddDays(30),
                    TeklifSonTarihi = DateTime.Now.AddDays(15),
                    IhaleDurumu = IhaleDurumu.IlanEdildi,
                    OlusturulmaTarihi = DateTime.Now
                },
                new Ihale
                {
                    IhaleId = 2,
                    IhaleAdi = "Temizlik Malzemesi Alımı",
                    IhaleKodu = "2025-002",
                    IhaleTuru = IhaleTuru.MalAlimi,
                    IhaleUsulu = IhaleUsulu.AcikIhale,
                    IhaleKonusu = "Belediye temizlik malzemesi alımı",
                    YaklasikMaliyet = 250000.00m,
                    GeciciTeminat = 3750.00m,
                    KesinTeminatOrani = 6.0,
                    IhaleTarihi = DateTime.Now.AddDays(20),
                    TeklifSonTarihi = DateTime.Now.AddDays(10),
                    IhaleDurumu = IhaleDurumu.TeklifAlma,
                    OlusturulmaTarihi = DateTime.Now
                }
            );

            // Örnek ihale kalemleri
            modelBuilder.Entity<IhaleKalemi>().HasData(
                new IhaleKalemi
                {
                    IhaleKalemiId = 1,
                    IhaleId = 1,
                    KalemAdi = "Temel Kazı İşi",
                    Birim = "m3",
                    Miktar = 500.00m,
                    BirimFiyat = 150.00m,
                    Aciklama = "Temel kazı ve hafriyat işi"
                },
                new IhaleKalemi
                {
                    IhaleKalemiId = 2,
                    IhaleId = 1,
                    KalemAdi = "Beton Döküm İşi",
                    Birim = "m3",
                    Miktar = 300.00m,
                    BirimFiyat = 800.00m,
                    Aciklama = "C25 beton döküm işi"
                },
                new IhaleKalemi
                {
                    IhaleKalemiId = 3,
                    IhaleId = 2,
                    KalemAdi = "Genel Temizlik Malzemesi",
                    Birim = "Takım",
                    Miktar = 100.00m,
                    BirimFiyat = 2500.00m,
                    Aciklama = "Standart temizlik malzemesi takımı"
                }
            );

            // Örnek teklifler
            modelBuilder.Entity<Teklif>().HasData(
                new Teklif
                {
                    TeklifId = 1,
                    IhaleId = 1,
                    FirmaAdi = "ABC İnşaat Ltd. Şti.",
                    VergiNumarasi = "1234567890",
                    TeklifTutari = 4750000.00m,
                    KdvTutari = 855000.00m,
                    YaklasikMaliyetOrani = 95.0,
                    TeklifDurumu = TeklifDurumu.Verildi,
                    TeklifTarihi = DateTime.Now.AddDays(-5),
                    Aciklama = "Kaliteli işçilik garantisi"
                },
                new Teklif
                {
                    TeklifId = 2,
                    IhaleId = 1,
                    FirmaAdi = "XYZ Yapı A.Ş.",
                    VergiNumarasi = "0987654321",
                    TeklifTutari = 4900000.00m,
                    KdvTutari = 882000.00m,
                    YaklasikMaliyetOrani = 98.0,
                    TeklifDurumu = TeklifDurumu.Verildi,
                    TeklifTarihi = DateTime.Now.AddDays(-3),
                    Aciklama = "Hızlı teslimat garantisi"
                }
            );
        }
    }
}
