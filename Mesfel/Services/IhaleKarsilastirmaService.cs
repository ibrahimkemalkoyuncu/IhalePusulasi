using Mesfel.Data;
using Mesfel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mesfel.Services
{
    public class IhaleKarsilastirmaService : IIhaleKarsilastirmaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IhaleKarsilastirmaService> _logger;
        private readonly IIhaleAnalizService _analizService;

        public IhaleKarsilastirmaService(
            ApplicationDbContext context,
            ILogger<IhaleKarsilastirmaService> logger,
            IIhaleAnalizService analizService)
        {
            _context = context;
            _logger = logger;
            _analizService = analizService;
        }

        public async Task<IhaleKarsilastirmaSonucu> KarsilastirAsync(int ihaleId1, int ihaleId2)
        {
            try
            {
                var ihale1 = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .Include(i => i.IhaleAnalizleri)
                    .Include(i => i.IhaleKategorileri)
                        .ThenInclude(ik => ik.Kategori)
                    .FirstOrDefaultAsync(i => i.Id == ihaleId1);

                var ihale2 = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .Include(i => i.IhaleAnalizleri)
                    .Include(i => i.IhaleKategorileri)
                        .ThenInclude(ik => ik.Kategori)
                    .FirstOrDefaultAsync(i => i.Id == ihaleId2);

                if (ihale1 == null || ihale2 == null)
                {
                    throw new ArgumentException("İhalelerden biri bulunamadı");
                }

                var analiz1 = ihale1.IhaleAnalizleri.OrderByDescending(a => a.AnalizTarihi).FirstOrDefault();
                var analiz2 = ihale2.IhaleAnalizleri.OrderByDescending(a => a.AnalizTarihi).FirstOrDefault();

                if (analiz1 == null)
                {
                    analiz1 = await _analizService.AnalizYapAsync(ihaleId1);
                    ihale1.IhaleAnalizleri.Add(analiz1);
                    await _context.SaveChangesAsync();
                }

                if (analiz2 == null)
                {
                    analiz2 = await _analizService.AnalizYapAsync(ihaleId2);
                    ihale2.IhaleAnalizleri.Add(analiz2);
                    await _context.SaveChangesAsync();
                }

                var sonuc = new IhaleKarsilastirmaSonucu
                {
                    Ihale1 = ihale1,
                    Ihale2 = ihale2,
                    Analiz1 = analiz1,
                    Analiz2 = analiz2,
                    BenzerlikOrani = await HesaplaBenzerlikOraniAsync(ihale1, ihale2, analiz1, analiz2),
                    Farkliliklar = await HesaplaFarkliliklarAsync(ihale1, ihale2, analiz1, analiz2),
                    OrtakOzellikler = await HesaplaOrtakOzelliklerAsync(ihale1, ihale2),
                    Tavsiyeler = await TavsiyeleriOlusturAsync(ihale1, ihale2, analiz1, analiz2)
                };

                return sonuc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale karşılaştırma sırasında hata oluştu. İhale1: {IhaleId1}, İhale2: {IhaleId2}",
                    ihaleId1, ihaleId2);
                throw;
            }
        }

        private async Task<List<string>> TavsiyeleriOlusturAsync(Ihale ihale1, Ihale ihale2, IhaleAnaliz analiz1, IhaleAnaliz analiz2)
        {
            var tavsiyeler = new List<string>();

            // Rekabet analizi
            if (analiz1.ToplamTeklifSayisi > analiz2.ToplamTeklifSayisi + 5)
            {
                tavsiyeler.Add($"{ihale1.IhaleAdi} daha rekabetçi bir ortama sahip");
            }

            // Fiyat analizi
            if (analiz1.OrtalamaTeklif < analiz2.OrtalamaTeklif * 0.9m)
            {
                tavsiyeler.Add($"{ihale1.IhaleAdi} daha uygun fiyatlı teklifler almış");
            }

            // Kategori benzerliği
            var ortakKategoriler = ihale1.IhaleKategorileri
                .Select(ik => ik.Kategori.KategoriAdi)
                .Intersect(ihale2.IhaleKategorileri.Select(ik => ik.Kategori.KategoriAdi))
                .Count();

            if (ortakKategoriler > 0)
            {
                tavsiyeler.Add("Benzer kategorilerde tecrübeniz varsa iki ihale için de avantajlı olabilirsiniz");
            }

            return tavsiyeler;
        }

        public async Task<List<IhaleBenzerlikAnalizi>> BenzerIhaleleriBulAsync(int ihaleId)
        {
            try
            {
                var hedefIhale = await _context.Ihaleler
                    .Include(i => i.IhaleAnalizleri)
                    .Include(i => i.IhaleKategorileri)
                        .ThenInclude(ik => ik.Kategori)
                    .FirstOrDefaultAsync(i => i.Id == ihaleId);

                if (hedefIhale == null)
                {
                    return new List<IhaleBenzerlikAnalizi>();
                }

                var hedefAnaliz = hedefIhale.IhaleAnalizleri.OrderByDescending(a => a.AnalizTarihi).FirstOrDefault();
                if (hedefAnaliz == null)
                {
                    hedefAnaliz = await _analizService.AnalizYapAsync(ihaleId);
                    hedefIhale.IhaleAnalizleri.Add(hedefAnaliz);
                    await _context.SaveChangesAsync();
                }

                var benzerIhaleler = await _context.Ihaleler
                    .Include(i => i.IhaleAnalizleri)
                    .Include(i => i.IhaleKategorileri)
                        .ThenInclude(ik => ik.Kategori)
                    .Where(i => i.Id != ihaleId && i.IhaleTuru == hedefIhale.IhaleTuru)
                    .ToListAsync();

                var sonuclar = new List<IhaleBenzerlikAnalizi>();

                foreach (var ihale in benzerIhaleler)
                {
                    var analiz = ihale.IhaleAnalizleri.OrderByDescending(a => a.AnalizTarihi).FirstOrDefault();
                    if (analiz == null)
                    {
                        analiz = await _analizService.AnalizYapAsync(ihale.Id);
                        ihale.IhaleAnalizleri.Add(analiz);
                        await _context.SaveChangesAsync();
                    }

                    var benzerlikOrani = await HesaplaBenzerlikOraniAsync(hedefIhale, ihale, hedefAnaliz, analiz);

                    sonuclar.Add(new IhaleBenzerlikAnalizi
                    {
                        Ihale = ihale,
                        Analiz = analiz,
                        BenzerlikOrani = benzerlikOrani,
                        OrtalamaTeklifFarki = analiz.OrtalamaTeklif - hedefAnaliz.OrtalamaTeklif,
                        KategoriBenzerlikPuan = await HesaplaKategoriBenzerlikPuanAsync(hedefIhale, ihale)
                    });
                }

                return sonuclar.OrderByDescending(s => s.BenzerlikOrani).Take(5).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Benzer ihaleler bulunurken hata oluştu. İhale ID: {IhaleId}", ihaleId);
                throw;
            }
        }

        private async Task<decimal> HesaplaBenzerlikOraniAsync(Ihale ihale1, Ihale ihale2, IhaleAnaliz analiz1, IhaleAnaliz analiz2)
        {
            // Temel benzerlik faktörleri
            decimal turBenzerligi = ihale1.IhaleTuru == ihale2.IhaleTuru ? 1 : 0;
            decimal kurumBenzerligi = ihale1.IhaleKurumu == ihale2.IhaleKurumu ? 0.5m : 0;

            // Fiyat benzerliği
            decimal fiyatBenzerligi = 1 - (Math.Abs(ihale1.KesifBedeli - ihale2.KesifBedeli) /
                Math.Max(ihale1.KesifBedeli, ihale2.KesifBedeli));

            // Teklif dağılım benzerliği
            decimal teklifBenzerligi = 1 - (Math.Abs(analiz1.OrtalamaTeklif - analiz2.OrtalamaTeklif) /
                Math.Max(analiz1.OrtalamaTeklif, analiz2.OrtalamaTeklif));

            // Kategori benzerliği
            decimal kategoriBenzerligi = await HesaplaKategoriBenzerlikPuanAsync(ihale1, ihale2) / 100m;

            // Ağırlıklı ortalama ile benzerlik puanı
            return (turBenzerligi * 0.25m) +
                   (kurumBenzerligi * 0.15m) +
                   (fiyatBenzerligi * 0.25m) +
                   (teklifBenzerligi * 0.2m) +
                   (kategoriBenzerligi * 0.15m);
        }

        private async Task<int> HesaplaKategoriBenzerlikPuanAsync(Ihale ihale1, Ihale ihale2)
        {
            // Null kontrolü ve kategori ID'lerini al
            var ihale1Kategoriler = ihale1.IhaleKategorileri?
                .Where(ik => ik.Kategori != null)
                .Select(ik => ik.Kategori.Id)
                .ToList() ?? new List<int>();

            var ihale2Kategoriler = ihale2.IhaleKategorileri?
                .Where(ik => ik.Kategori != null)
                .Select(ik => ik.Kategori.Id)
                .ToList() ?? new List<int>();

            var ortakKategoriler = ihale1Kategoriler.Intersect(ihale2Kategoriler).Count();
            var toplamKategoriler = ihale1Kategoriler.Union(ihale2Kategoriler).Count();

            return toplamKategoriler == 0 ? 0 : (ortakKategoriler * 100) / toplamKategoriler;
        }

        private async Task<List<Farklilik>> HesaplaFarkliliklarAsync(Ihale ihale1, Ihale ihale2, IhaleAnaliz analiz1, IhaleAnaliz analiz2)
        {
            var farkliliklar = new List<Farklilik>();

            // 1. İhale Türü Farkı
            if (ihale1.IhaleTuru != ihale2.IhaleTuru)
            {
                farkliliklar.Add(new Farklilik
                {
                    Baslik = "İhale Türü",
                    Deger1 = ihale1.IhaleTuru.ToString(),
                    Deger2 = ihale2.IhaleTuru.ToString(),
                    Aciklama = "İhale türleri farklı"
                });
            }

            // 2. Keşif Bedeli Farkı (%20'den fazla ise)
            var farkOrani = Math.Abs(ihale1.KesifBedeli - ihale2.KesifBedeli) / Math.Max(ihale1.KesifBedeli, ihale2.KesifBedeli);
            if (farkOrani > 0.2m)
            {
                farkliliklar.Add(new Farklilik
                {
                    Baslik = "Keşif Bedeli",
                    Deger1 = ihale1.KesifBedeli.ToString("C"),
                    Deger2 = ihale2.KesifBedeli.ToString("C"),
                    Aciklama = $"Önemli fark: %{farkOrani * 100:F1}"
                });
            }

            // 3. Teklif Sayısı Farkı
            if (analiz1.ToplamTeklifSayisi != analiz2.ToplamTeklifSayisi)
            {
                farkliliklar.Add(new Farklilik
                {
                    Baslik = "Teklif Sayısı",
                    Deger1 = analiz1.ToplamTeklifSayisi.ToString(),
                    Deger2 = analiz2.ToplamTeklifSayisi.ToString(),
                    Aciklama = "Rekabet düzeyi farklı"
                });
            }

            // 4. Ortalama Teklif Farkı
            var ortalamaFark = Math.Abs(analiz1.OrtalamaTeklif - analiz2.OrtalamaTeklif);
            if (ortalamaFark > (Math.Max(analiz1.OrtalamaTeklif, analiz2.OrtalamaTeklif) * 0.15m))
            {
                farkliliklar.Add(new Farklilik
                {
                    Baslik = "Ortalama Teklif",
                    Deger1 = analiz1.OrtalamaTeklif.ToString("C"),
                    Deger2 = analiz2.OrtalamaTeklif.ToString("C"),
                    Aciklama = $"Önemli fark: {ortalamaFark.ToString("C")}"
                });
            }

            return farkliliklar;
        }

        private async Task<List<OrtakOzellik>> HesaplaOrtakOzelliklerAsync(Ihale ihale1, Ihale ihale2)
        {
            var ortakOzellikler = new List<OrtakOzellik>();

            // 1. Aynı Kurum
            if (ihale1.IhaleKurumu == ihale2.IhaleKurumu)
            {
                ortakOzellikler.Add(new OrtakOzellik
                {
                    OzellikAdi = "Kurum",
                    Deger = ihale1.IhaleKurumu
                });
            }

            // 2. Benzer Kategoriler
            var ortakKategoriler = ihale1.IhaleKategorileri
                .Select(ik => ik.Kategori.KategoriAdi)
                .Intersect(ihale2.IhaleKategorileri.Select(ik => ik.Kategori.KategoriAdi))
                .ToList();

            if (ortakKategoriler.Any())
            {
                ortakOzellikler.Add(new OrtakOzellik
                {
                    OzellikAdi = "Ortak Kategoriler",
                    Deger = string.Join(", ", ortakKategoriler)
                });
            }

            // 3. Benzer Teklif Aralığı (%10 fark içinde)
            var teklifOran = Math.Abs(ihale1.KesifBedeli - ihale2.KesifBedeli) / Math.Max(ihale1.KesifBedeli, ihale2.KesifBedeli);
            if (teklifOran < 0.1m)
            {
                ortakOzellikler.Add(new OrtakOzellik
                {
                    OzellikAdi = "Benzer Bütçe",
                    Deger = $"±%10 fark ({ihale1.KesifBedeli.ToString("C")} - {ihale2.KesifBedeli.ToString("C")})"
                });
            }

            return ortakOzellikler;
        }
    }
}