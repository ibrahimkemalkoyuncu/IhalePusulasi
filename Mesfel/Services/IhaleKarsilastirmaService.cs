using Mesfel.Data;
using Mesfel.Models;
using Microsoft.EntityFrameworkCore;

namespace Mesfel.Services
{
    public class IhaleKarsilastirmaService : IIhaleKarsilastirmaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IhaleKarsilastirmaService> _logger;

        public IhaleKarsilastirmaService(ApplicationDbContext context, ILogger<IhaleKarsilastirmaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IhaleKarsilastirmaSonucu> KarsilastirAsync(int ihaleId1, int ihaleId2)
        {
            try
            {
                var ihale1 = await _context.Ihaleler
                    .Include(i => i.Teklifler)
                    .Include(i => i.IhaleAnalizleri)
                    .FirstOrDefaultAsync(i => i.Id == ihaleId1);

                var ihale2 = await _context.Ihaleler
                    .Include(i => i.Teklifler)
                    .Include(i => i.IhaleAnalizleri)
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
                }

                if (analiz2 == null)
                {
                    analiz2 = await _analizService.AnalizYapAsync(ihaleId2);
                }

                var sonuc = new IhaleKarsilastirmaSonucu
                {
                    Ihale1 = ihale1,
                    Ihale2 = ihale2,
                    Analiz1 = analiz1,
                    Analiz2 = analiz2,
                    BenzerlikOrani = HesaplaBenzerlikOrani(ihale1, ihale2, analiz1, analiz2),
                    Farkliliklar = HesaplaFarkliliklar(ihale1, ihale2, analiz1, analiz2)
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

        public async Task<List<IhaleBenzerlikAnalizi>> BenzerIhaleleriBulAsync(int ihaleId)
        {
            var hedefIhale = await _context.Ihaleler
                .Include(i => i.IhaleAnalizleri)
                .FirstOrDefaultAsync(i => i.Id == ihaleId);

            if (hedefIhale == null)
            {
                return new List<IhaleBenzerlikAnalizi>();
            }

            var hedefAnaliz = hedefIhale.IhaleAnalizleri.OrderByDescending(a => a.AnalizTarihi).FirstOrDefault();
            if (hedefAnaliz == null)
            {
                hedefAnaliz = await _analizService.AnalizYapAsync(ihaleId);
            }

            var benzerIhaleler = await _context.Ihaleler
                .Include(i => i.IhaleAnalizleri)
                .Where(i => i.Id != ihaleId && i.IhaleTuru == hedefIhale.IhaleTuru)
                .ToListAsync();

            var sonuclar = new List<IhaleBenzerlikAnalizi>();

            foreach (var ihale in benzerIhaleler)
            {
                var analiz = ihale.IhaleAnalizleri.OrderByDescending(a => a.AnalizTarihi).FirstOrDefault();
                if (analiz == null) continue;

                var benzerlikOrani = HesaplaBenzerlikOrani(hedefIhale, ihale, hedefAnaliz, analiz);

                sonuclar.Add(new IhaleBenzerlikAnalizi
                {
                    Ihale = ihale,
                    Analiz = analiz,
                    BenzerlikOrani = benzerlikOrani,
                    OrtalamaTeklifFarki = analiz.OrtalamaTeklif - hedefAnaliz.OrtalamaTeklif
                });
            }

            return sonuclar.OrderByDescending(s => s.BenzerlikOrani).Take(5).ToList();
        }

        private decimal HesaplaBenzerlikOrani(Ihale ihale1, Ihale ihale2, IhaleAnaliz analiz1, IhaleAnaliz analiz2)
        {
            // Temel benzerlik faktörleri
            decimal turBenzerligi = ihale1.IhaleTuru == ihale2.IhaleTuru ? 1 : 0;
            decimal kurumBenzerligi = ihale1.IhaleYapanKurum == ihale2.IhaleYapanKurum ? 0.5m : 0;

            // Fiyat benzerliği
            decimal fiyatBenzerligi = 1 - (Math.Abs(ihale1.KesifBedeli - ihale2.KesifBedeli) /
                Math.Max(ihale1.KesifBedeli, ihale2.KesifBedeli));

            // Teklif dağılım benzerliği
            decimal teklifBenzerligi = 1 - (Math.Abs(analiz1.OrtalamaTeklif - analiz2.OrtalamaTeklif) /
                Math.Max(analiz1.OrtalamaTeklif, analiz2.OrtalamaTeklif));

            // Ağırlıklı ortalama ile benzerlik puanı
            return (turBenzerligi * 0.3m) + (kurumBenzerligi * 0.2m) +
                   (fiyatBenzerligi * 0.3m) + (teklifBenzerligi * 0.2m);
        }

        private List<string> HesaplaFarkliliklar(Ihale ihale1, Ihale ihale2, IhaleAnaliz analiz1, IhaleAnaliz analiz2)
        {
            var farkliliklar = new List<string>();

            if (ihale1.IhaleTuru != ihale2.IhaleTuru)
            {
                farkliliklar.Add($"İhale türleri farklı: {ihale1.IhaleTuru} vs {ihale2.IhaleTuru}");
            }

            if (ihale1.IhaleYapanKurum != ihale2.IhaleYapanKurum)
            {
                farkliliklar.Add($"Farklı kurumlar: {ihale1.IhaleYapanKurum} vs {ihale2.IhaleYapanKurum}");
            }

            decimal fiyatFarki = Math.Abs(ihale1.KesifBedeli - ihale2.KesifBedeli);
            if (fiyatFarki > (Math.Max(ihale1.KesifBedeli, ihale2.KesifBedeli) * 0.2m)
            {
                farkliliklar.Add($"Önemli keşif bedeli farkı: {ihale1.KesifBedeli:C} vs {ihale2.KesifBedeli:C}");
            }

            decimal ortalamaFark = Math.Abs(analiz1.OrtalamaTeklif - analiz2.OrtalamaTeklif);
            if (ortalamaFark > (Math.Max(analiz1.OrtalamaTeklif, analiz2.OrtalamaTeklif) * 0.15m))
            {
                farkliliklar.Add($"Önemli ortalama teklif farkı: {analiz1.OrtalamaTeklif:C} vs {analiz2.OrtalamaTeklif:C}");
            }

            if (analiz1.ToplamTeklifSayisi != analiz2.ToplamTeklifSayisi)
            {
                farkliliklar.Add($"Teklif sayıları farklı: {analiz1.ToplamTeklifSayisi} vs {analiz2.ToplamTeklifSayisi}");
            }

            return farkliliklar;
        }
    }
}
