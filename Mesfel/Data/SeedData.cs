using Mesfel.Models;
using Mesfel.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Mesfel.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Veritabanı oluşturulmuş mu kontrol et
                await context.Database.EnsureCreatedAsync();

                // Kullanıcılar eklenmiş mi kontrol et
                if (!context.Kullanicilar.Any())
                {
                    await SeedKullanicilar(context);
                }

                // İhaleler eklenmiş mi kontrol et
                if (!context.Ihaleler.Any())
                {
                    await SeedIhaleler(context);
                }

                // İhale kalemleri eklenmiş mi kontrol et
                if (!context.IhaleKalemleri.Any())
                {
                    await SeedIhaleKalemleri(context);
                }
            }
        }

        private static async Task SeedKullanicilar(ApplicationDbContext context)
        {
            var kullanicilar = new List<Kullanici>
            {
                new Kullanici
                {
                    KullaniciAdi = "admin",
                    Email = "admin@mesfel.com",
                    AdSoyad = "Sistem Yöneticisi",
                    Telefon = "05551234567",
                    //SifreHash = PasswordHasher.HashPassword("Admin123!"), // Gerçek uygulamada daha güçlü şifre kullanın
                   SifreHash = new PasswordHasher<Kullanici>().HashPassword(null, "Admin123!"), // Gerçek uygulamada daha güçlü şifre kullanın
                    Rol = "Admin",
                    AktifMi = true,
                    KayitTarihi = DateTime.Now.AddDays(-30)
                },
                new Kullanici
                {
                    KullaniciAdi = "ihaleuzmani",
                    Email = "uzman@mesfel.com",
                    AdSoyad = "İhale Uzmanı",
                    Telefon = "05552345678",
                    SifreHash = new PasswordHasher<Kullanici>().HashPassword(null, "Uzman123!"),
                    Rol = "Uzman",
                    AktifMi = true,
                    KayitTarihi = DateTime.Now.AddDays(-20)
                },
                new Kullanici
                {
                    KullaniciAdi = "standartkullanici",
                    Email = "kullanici@mesfel.com",
                    AdSoyad = "Standart Kullanıcı",
                    Telefon = "05553456789",
                    SifreHash = new PasswordHasher<Kullanici>().HashPassword(null, "User123!"),
                    Rol = "Kullanici",
                    AktifMi = true,
                    KayitTarihi = DateTime.Now.AddDays(-10)
                }
            };

            await context.Kullanicilar.AddRangeAsync(kullanicilar);
            await context.SaveChangesAsync();
        }

        private static async Task SeedIhaleler(ApplicationDbContext context)
        {
            var adminKullanici = await context.Kullanicilar.FirstOrDefaultAsync(k => k.KullaniciAdi == "admin");

            var ihaleler = new List<Ihale>
            {
                new Ihale
                {
                    IhaleNo = "IH-2023-001",
                    IhaleAdi = "Ankara Hastanesi Tıbbi Cihaz Alımı",
                    IhaleKurumu = "Sağlık Bakanlığı",
                    KesifBedeli = 5_000_000m,
                    IhaleBaslangicTarihi = DateTime.Now.AddDays(-15),
                    IhaleBitisTarihi = DateTime.Now.AddDays(15),
                    TeklifSonTarihi = DateTime.Now.AddDays(10),
                    IhaleTuru = IhaleTuru.Kamu,
                    Aciklama = "Ankara Şehir Hastanesi için tıbbi cihaz alım ihalesi",
                    IhaleLinki = "https://example.com/ihale/IH-2023-001",
                    IhaleNumarasi = "2023/001",
                    IletisimBilgileri = "0312 123 45 67 - ihale@ankarahastanesi.gov.tr",
                    IhaleUsulu = IhaleUsulu.AcikIhale.ToString(),
                    IhaleDurumu = "Aktif",
                    KayitTarihi = DateTime.Now.AddDays(-15),
                    KaydedenKullanici = adminKullanici?.KullaniciAdi,
                    YaklasikMaliyet = 4_800_000m
                },
                new Ihale
                {
                    IhaleNo = "IH-2023-002",
                    IhaleAdi = "İstanbul Metro Hattı İnşaat İşi",
                    IhaleKurumu = "Ulaştırma Bakanlığı",
                    KesifBedeli = 250_000_000m,
                    IhaleBaslangicTarihi = DateTime.Now.AddDays(-10),
                    IhaleBitisTarihi = DateTime.Now.AddDays(20),
                    TeklifSonTarihi = DateTime.Now.AddDays(15),
                    IhaleTuru = IhaleTuru.Kamu,
                    Aciklama = "İstanbul M5 metro hattı inşaat işi ihalesi",
                    IhaleLinki = "https://example.com/ihale/IH-2023-002",
                    IhaleNumarasi = "2023/002",
                    IletisimBilgileri = "0212 987 65 43 - ihale@ulasitim.gov.tr",
                    IhaleUsulu = IhaleUsulu.AcikIhale.ToString(),
                    IhaleDurumu = "Aktif",
                    KayitTarihi = DateTime.Now.AddDays(-10),
                    KaydedenKullanici = adminKullanici?.KullaniciAdi,
                    YaklasikMaliyet = 245_000_000m
                },
                new Ihale
                {
                    IhaleNo = "IH-2023-003",
                    IhaleAdi = "Özel Üniversite Kampüs İnşaatı",
                    IhaleKurumu = "XYZ Üniversitesi",
                    KesifBedeli = 120_000_000m,
                    IhaleBaslangicTarihi = DateTime.Now.AddDays(-5),
                    IhaleBitisTarihi = DateTime.Now.AddDays(25),
                    TeklifSonTarihi = DateTime.Now.AddDays(20),
                    IhaleTuru = IhaleTuru.Ozel,
                    Aciklama = "Özel üniversite kampüs inşaatı ihalesi",
                    IhaleLinki = "https://example.com/ihale/IH-2023-003",
                    IhaleNumarasi = "2023/003",
                    IletisimBilgileri = "0312 456 78 90 - ihale@xyzuniversitesi.edu.tr",
                    IhaleUsulu = IhaleUsulu.BelliIsyerlerineDavetli.ToString(),
                    IhaleDurumu = "Aktif",
                    KayitTarihi = DateTime.Now.AddDays(-5),
                    KaydedenKullanici = adminKullanici?.KullaniciAdi,
                    YaklasikMaliyet = 118_000_000m
                },
                new Ihale
                {
                    IhaleNo = "IH-2023-004",
                    IhaleAdi = "Belediye Otobüs Alımı",
                    IhaleKurumu = "Ankara Büyükşehir Belediyesi",
                    KesifBedeli = 80_000_000m,
                    IhaleBaslangicTarihi = DateTime.Now.AddDays(-30),
                    IhaleBitisTarihi = DateTime.Now.AddDays(-5),
                    TeklifSonTarihi = DateTime.Now.AddDays(-10),
                    IhaleTuru = IhaleTuru.Kamu,
                    Aciklama = "Belediye için 50 adet otobüs alım ihalesi",
                    IhaleLinki = "https://example.com/ihale/IH-2023-004",
                    IhaleNumarasi = "2023/004",
                    IletisimBilgileri = "0312 999 88 77 - ihale@ankara.bel.tr",
                    IhaleUsulu = IhaleUsulu.AcikIhale.ToString(),
                    IhaleDurumu = "Tamamlandı",
                    KayitTarihi = DateTime.Now.AddDays(-30),
                    KaydedenKullanici = adminKullanici?.KullaniciAdi,
                    YaklasikMaliyet = 78_500_000m
                }
            };

            await context.Ihaleler.AddRangeAsync(ihaleler);
            await context.SaveChangesAsync();
        }

        private static async Task SeedIhaleKalemleri(ApplicationDbContext context)
        {
            var ihaleler = await context.Ihaleler.ToListAsync();

            var ihaleKalemleri = new List<IhaleKalemi>();

            foreach (var ihale in ihaleler)
            {
                // Her ihale için 3-5 kalem ekleyelim
                switch (ihale.IhaleNo)
                {
                    case "IH-2023-001": // Tıbbi Cihaz Alımı
                        ihaleKalemleri.AddRange(new List<IhaleKalemi>
                        {
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "MR Cihazı",
                                Birim = "Adet",
                                Miktar = 2,
                                BirimFiyat = 1_500_000m,
                                Aciklama = "1.5 Tesla manyetik rezonans görüntüleme cihazı"
                            },
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "BT Cihazı",
                                Birim = "Adet",
                                Miktar = 3,
                                BirimFiyat = 800_000m,
                                Aciklama = "64 kesit bilgisayarlı tomografi cihazı"
                            },
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "Ultrason Cihazı",
                                Birim = "Adet",
                                Miktar = 5,
                                BirimFiyat = 250_000m,
                                Aciklama = "Renkli Doppler ultrason cihazı"
                            }
                        });
                        break;

                    case "IH-2023-002": // Metro Hattı İnşaatı
                        ihaleKalemleri.AddRange(new List<IhaleKalemi>
                        {
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "Tünel Açma",
                                Birim = "Metre",
                                Miktar = 5000,
                                BirimFiyat = 15_000m,
                                Aciklama = "TBM ile tünel açma işi"
                            },
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "Ray Döşeme",
                                Birim = "Metre",
                                Miktar = 10_000,
                                BirimFiyat = 2_500m,
                                Aciklama = "Ray ve travers döşeme işi"
                            },
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "İstasyon İnşaatı",
                                Birim = "Adet",
                                Miktar = 8,
                                BirimFiyat = 12_000_000m,
                                Aciklama = "Metro istasyonu inşaatı"
                            }
                        });
                        break;

                    case "IH-2023-003": // Üniversite Kampüs İnşaatı
                        ihaleKalemleri.AddRange(new List<IhaleKalemi>
                        {
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "Derslik Binası",
                                Birim = "Adet",
                                Miktar = 1,
                                BirimFiyat = 40_000_000m,
                                Aciklama = "5 katlı derslik binası inşaatı"
                            },
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "Kütüphane Binası",
                                Birim = "Adet",
                                Miktar = 1,
                                BirimFiyat = 25_000_000m,
                                Aciklama = "3 katlı kütüphane binası inşaatı"
                            },
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "Yurt Binası",
                                Birim = "Adet",
                                Miktar = 1,
                                BirimFiyat = 35_000_000m,
                                Aciklama = "8 katlı öğrenci yurdu inşaatı"
                            }
                        });
                        break;

                    case "IH-2023-004": // Belediye Otobüs Alımı
                        ihaleKalemleri.AddRange(new List<IhaleKalemi>
                        {
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "Dizel Otobüs",
                                Birim = "Adet",
                                Miktar = 30,
                                BirimFiyat = 1_200_000m,
                                Aciklama = "Euro 6 standartlarında dizel otobüs"
                            },
                            new IhaleKalemi
                            {
                                IhaleId = ihale.Id,
                                KalemAdi = "Elektrikli Otobüs",
                                Birim = "Adet",
                                Miktar = 20,
                                BirimFiyat = 1_800_000m,
                                Aciklama = "Tam elektrikli şarjlı otobüs"
                            }
                        });
                        break;
                }
            }

            await context.IhaleKalemleri.AddRangeAsync(ihaleKalemleri);
            await context.SaveChangesAsync();
        }
    }
}