using Mesfel.Models;
using Mesfel.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Mesfel.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet tanımlamaları
        public DbSet<Ihale> Ihaleler { get; set; }
        public DbSet<IhaleDetay> IhaleDetaylari { get; set; }
        public DbSet<IhaleTeklif> IhaleTeklifleri { get; set; }
        public DbSet<IhaleKategori> IhaleKategorileri { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<IhaleAnaliz> IhaleAnalizleri { get; set; }
        public DbSet<IhaleKalemi> IhaleKalemleri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            

            // İhale model konfigürasyonu
            modelBuilder.Entity<Ihale>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IhaleAdi).IsRequired().HasMaxLength(500);
                entity.Property(e => e.IhaleKurumu).IsRequired().HasMaxLength(200);
                entity.Property(e => e.KesifBedeli).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IhaleBaslangicTarihi).IsRequired();
                entity.Property(e => e.IhaleBitisTarihi).IsRequired();
                entity.Property(e => e.IhaleTuru).IsRequired().HasConversion<string>().HasMaxLength(100);
                entity.Property(e => e.Aciklama).HasMaxLength(1000);
                entity.Property(e => e.IhaleLinki).HasMaxLength(500);
                entity.Property(e => e.IhaleNumarasi).HasMaxLength(100);
                entity.Property(e => e.IletisimBilgileri).HasMaxLength(200);
                entity.Property(e => e.IhaleUsulu).HasConversion<string>().HasMaxLength(100);
                entity.Property(e => e.IhaleDurumu).HasConversion<string>().HasMaxLength(50).HasDefaultValue("Aktif");
                entity.Property(e => e.KayitTarihi).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.KaydedenKullanici).HasMaxLength(100);
                entity.Property(e => e.GuncelleyenKullanici).HasMaxLength(100);
            });

            // İhale Detay ilişkisi
            modelBuilder.Entity<IhaleDetay>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.KalemAdi).IsRequired().HasMaxLength(200);
                entity.Property(e => e.BirimFiyat).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Miktar).IsRequired();
                entity.Property(e => e.Birim).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ToplamTutar).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Aciklama).HasMaxLength(500);
                entity.Property(e => e.KayitTarihi).HasDefaultValueSql("GETDATE()");

                entity.HasOne(d => d.Ihale)
                    .WithMany(p => p.IhaleDetaylari)
                    .HasForeignKey(d => d.IhaleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Ihale>()
                .HasMany(i => i.IhaleKalemleri)
                .WithOne(ik => ik.Ihale)
                .HasForeignKey(ik => ik.IhaleId);

            // İhale Teklif ilişkisi
            modelBuilder.Entity<IhaleTeklif>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirmaAdi).IsRequired().HasMaxLength(200);
                entity.Property(e => e.VergiNo).IsRequired().HasMaxLength(11);
                entity.Property(e => e.TeklifTutari).HasColumnType("decimal(18,2)");
                entity.Property(e => e.KdvTutari).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TeklifDurumu).HasDefaultValue(TeklifDurumu.Verildi);
                entity.Property(e => e.TeklifTarihi).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Aciklama).HasMaxLength(500);
                entity.Property(e => e.GecerliTeklif).HasDefaultValue(true);

                entity.HasOne(d => d.Ihale)
                    .WithMany(p => p.IhaleTeklifleri)
                    .HasForeignKey(d => d.IhaleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.TeklifDurumu)
                    .HasConversion<TeklifDurumuConverter>()
                    .HasMaxLength(20)
                    .HasDefaultValue(TeklifDurumu.Verildi)
                    .IsRequired()
                    .Metadata.SetValueComparer(
                        new ValueComparer<TeklifDurumu>(
                            (l, r) => l == r,
                            v => v.GetHashCode(),
                            v => v));

            });

            // İhale Kategori ilişkisi (Many-to-Many)
            modelBuilder.Entity<IhaleKategori>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AnaKategori).HasDefaultValue(false);
                entity.Property(e => e.KayitTarihi).HasDefaultValueSql("GETDATE()");

                entity.HasOne(d => d.Ihale)
                    .WithMany(p => p.IhaleKategorileri)
                    .HasForeignKey(d => d.IhaleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Kategori)
                    .WithMany(p => p.IhaleKategorileri)
                    .HasForeignKey(d => d.KategoriId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Kategori model konfigürasyonu
            modelBuilder.Entity<Kategori>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.KategoriAdi).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Aciklama).HasMaxLength(500);
                entity.Property(e => e.AktifMi).HasDefaultValue(true);
                entity.Property(e => e.KayitTarihi).HasDefaultValueSql("GETDATE()");
            });

            // Kullanıcı model konfigürasyonu
            modelBuilder.Entity<Kullanici>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.KullaniciAdi).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AdSoyad).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Telefon).HasMaxLength(15);
                entity.Property(e => e.Rol).HasMaxLength(50).HasDefaultValue("Kullanici");
                entity.Property(e => e.AktifMi).HasDefaultValue(true);
                entity.Property(e => e.KayitTarihi).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.SifreHash).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.KullaniciAdi).IsUnique();
            });

            // IhaleAnaliz konfigürasyonu
            modelBuilder.Entity<IhaleAnaliz>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AnalizTipi).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Sonuc).HasMaxLength(1000);
                entity.Property(e => e.AnalizTarihi).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Puan).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Aciklama).HasMaxLength(2000);
                entity.Property(e => e.Aktif).HasDefaultValue(true);
                entity.Property(e => e.OrtalamaTeklif).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EnDusukTeklif).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EnYuksekTeklif).HasColumnType("decimal(18,2)");
                entity.Property(e => e.KazananTeklif).HasColumnType("decimal(18,2)");
                entity.Property(e => e.RekabetOrani).HasColumnType("decimal(5,2)");
                entity.Property(e => e.TasarrufOrani).HasColumnType("decimal(5,2)");

                entity.HasOne(d => d.Ihale)
                    .WithMany(p => p.IhaleAnalizleri)
                    .HasForeignKey(d => d.IhaleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // IhaleKalemi konfigürasyonu
            modelBuilder.Entity<IhaleKalemi>(entity =>
            {
                entity.HasKey(e => e.IhaleKalemiId);
                entity.Property(e => e.KalemAdi).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Birim).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Miktar).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BirimFiyat).HasColumnType("decimal(18,2)");
                // entity.Property(e => e.ToplamFiyat).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.Ihale)
                    .WithMany(p => p.IhaleKalemleri)
                    .HasForeignKey(d => d.IhaleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Kategori seed data
            modelBuilder.Entity<Kategori>().HasData(
                new Kategori { Id = 1, KategoriAdi = "İnşaat", Aciklama = "İnşaat ihaleleri", AktifMi = true, KayitTarihi = DateTime.Now },
                new Kategori { Id = 2, KategoriAdi = "Bilgi İşlem", Aciklama = "Bilgi işlem ihaleleri", AktifMi = true, KayitTarihi = DateTime.Now },
                new Kategori { Id = 3, KategoriAdi = "Hizmet", Aciklama = "Hizmet ihaleleri", AktifMi = true, KayitTarihi = DateTime.Now },
                new Kategori { Id = 4, KategoriAdi = "Mal", Aciklama = "Mal alım ihaleleri", AktifMi = true, KayitTarihi = DateTime.Now },
                new Kategori { Id = 5, KategoriAdi = "Danışmanlık", Aciklama = "Danışmanlık hizmet ihaleleri", AktifMi = true, KayitTarihi = DateTime.Now }
            );

            // Kullanıcı seed data
            modelBuilder.Entity<Kullanici>().HasData(
                new Kullanici
                {
                    Id = 1,
                    KullaniciAdi = "admin",
                    Email = "admin@mesfel.com",
                    AdSoyad = "Sistem Yöneticisi",
                    Telefon = "555-0000",
                    Rol = "Admin",
                    AktifMi = true,
                    KayitTarihi = DateTime.Now,
                    // BURAYA SifreHash değerini eklemelisiniz:
                    SifreHash = "YOUR_HASHED_PASSWORD_HERE" // Örnek: BCrypt.Net.BCrypt.HashPassword("your_initial_admin_password")
                }
            );
        }
    }
}