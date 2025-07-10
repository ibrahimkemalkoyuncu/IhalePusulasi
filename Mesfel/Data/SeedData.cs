using Mesfel.Data;
using Mesfel.Models;
using Mesfel.Services;
using Mesfel.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mesfel.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();
            var passwordHasher = services.GetRequiredService<IPasswordHasherService>();

            // Veritabanı oluşturulmamışsa oluştur
            context.Database.EnsureCreated();

            // Temel verileri seed et
            SeedTemelVeriler(context, passwordHasher);

            // HomeController için özel verileri seed et
            SeedHomeControllerVerileri(context);
        }

        private static void SeedTemelVeriler(ApplicationDbContext context, IPasswordHasherService passwordHasher)
        {
            SeedKategoriler(context);
            SeedKullanicilar(context, passwordHasher);
            SeedIhaleler(context);
        }

        private static void SeedHomeControllerVerileri(ApplicationDbContext context)
        {
            // Home/Index için gerekli minimum veri kontrolü
            if (!context.Ihaleler.Any())
            {
                SeedIhaleler(context);
            }

            // Dashboard için istatistik verileri
            if (!context.IhaleAnalizleri.Any())
            {
                SeedIhaleAnalizleri(context);
            }
        }

        private static void SeedKategoriler(ApplicationDbContext context)
        {
            if (!context.Kategoriler.Any())
            {
                context.Kategoriler.AddRange(
                    new Kategori { Id = 1, KategoriAdi = "İnşaat", Aciklama = "İnşaat ihaleleri", AktifMi = true, KayitTarihi = DateTime.Now },
                    new Kategori { Id = 2, KategoriAdi = "Bilgi İşlem", Aciklama = "Bilgi işlem ihaleleri", AktifMi = true, KayitTarihi = DateTime.Now },
                    new Kategori { Id = 3, KategoriAdi = "Hizmet", Aciklama = "Hizmet ihaleleri", AktifMi = true, KayitTarihi = DateTime.Now },
                    new Kategori { Id = 4, KategoriAdi = "Mal", Aciklama = "Mal alım ihaleleri", AktifMi = true, KayitTarihi = DateTime.Now },
                    new Kategori { Id = 5, KategoriAdi = "Danışmanlık", Aciklama = "Danışmanlık hizmet ihaleleri", AktifMi = true, KayitTarihi = DateTime.Now }
                );
                context.SaveChanges();
            }
        }

        private static void SeedKullanicilar(ApplicationDbContext context, IPasswordHasherService passwordHasher)
        {
            if (!context.Kullanicilar.Any())
            {
                context.Kullanicilar.Add(
                    new Kullanici
                    {
                        Id = 1,
                        KullaniciAdi = "admin",
                        Email = "admin@mesfel.com",
                        AdSoyad = "Sistem Yöneticisi",
                        Telefon = "555-0000",
                        Rol = "Admin",
                        SifreHash = passwordHasher.HashPassword("Admin123!"),
                        AktifMi = true,
                        KayitTarihi = DateTime.Now
                    }
                );
                context.SaveChanges();
            }
        }

        private static void SeedIhaleler(ApplicationDbContext context)
        {
            if (!context.Ihaleler.Any() && context.Kategoriler.Any())
            {
                context.Ihaleler.AddRange(
                    new Ihale
                    {
                        Id = 1,
                        IhaleAdi = "Ankara Yolu Asfaltlama İşi",
                        IhaleKurumu = "Ankara Büyükşehir Belediyesi",
                        KesifBedeli = 5000000,
                        IhaleBaslangicTarihi = DateTime.Now.AddDays(-10),
                        IhaleBitisTarihi = DateTime.Now.AddDays(20),
                        IhaleTuru = IhaleTuru.YapimIsi,
                        IhaleDurumu = nameof(IhaleDurumu.TeklifAlma),
                        KayitTarihi = DateTime.Now,
                        KaydedenKullanici = "admin",
                        IhaleKategorileri = new List<IhaleKategori>
                        {
                            new IhaleKategori { KategoriId = 1 }
                        },
                        IhaleTeklifleri = new List<IhaleTeklif>
                        {
                            new IhaleTeklif
                            {
                                FirmaAdi = "AŞTİ İnşaat",
                                TeklifTutari = 4800000,
                                TeklifTarihi = DateTime.Now.AddDays(-2),
                                TeklifDurumu = TeklifDurumu.Verildi // Correctly assigning the enum value
                            }
                        }
                    },
                    new Ihale
                    {
                        Id = 2,
                        IhaleAdi = "Bilgi Sistemi Alımı",
                        IhaleKurumu = "Sağlık Bakanlığı",
                        KesifBedeli = 2500000,
                        IhaleBaslangicTarihi = DateTime.Now.AddDays(-5),
                        IhaleBitisTarihi = DateTime.Now.AddDays(15),
                        IhaleTuru = IhaleTuru.MalAlimi,
                        IhaleDurumu = nameof(IhaleDurumu.IlanEdildi),
                        KayitTarihi = DateTime.Now,
                        KaydedenKullanici = "admin",
                        IhaleKategorileri = new List<IhaleKategori>
                        {
                            new IhaleKategori { KategoriId = 2 }
                        }
                    }
                );
                context.SaveChanges();
            }
        }

        private static void SeedIhaleAnalizleri(ApplicationDbContext context)
        {
            if (!context.IhaleAnalizleri.Any() && context.Ihaleler.Any())
            {
                foreach (var ihale in context.Ihaleler)
                {
                    context.IhaleAnalizleri.Add(
                        new IhaleAnaliz
                        {
                            IhaleId = ihale.Id,
                            AnalizTipi = "Başlangıç Analizi",
                            AnalizTarihi = DateTime.Now,
                            ToplamTeklifSayisi = ihale.IhaleTeklifleri?.Count ?? 0,
                            OrtalamaTeklif = ihale.IhaleTeklifleri?.Average(t => t.TeklifTutari) ?? 0,
                            EnDusukTeklif = ihale.IhaleTeklifleri?.Min(t => t.TeklifTutari) ?? 0,
                            EnYuksekTeklif = ihale.IhaleTeklifleri?.Max(t => t.TeklifTutari) ?? 0,
                            RekabetOrani = ihale.IhaleTeklifleri?.Count > 1 ?
                                (ihale.IhaleTeklifleri.Max(t => t.TeklifTutari) - ihale.IhaleTeklifleri.Min(t => t.TeklifTutari)) /
                                ihale.IhaleTeklifleri.Min(t => t.TeklifTutari) * 100 : 0,
                            Aktif = true,
                            OlusturulmaTarihi = DateTime.Now
                        }
                    );
                }
                context.SaveChanges();
            }
        }
    }
}