using Mesfel.Data;
using Mesfel.Models;
using Mesfel.Sabitler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mesfel.Services
{
    public class ZamanSerisiAnalizService : IZamanSerisiAnalizService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ZamanSerisiAnalizService> _logger;

        public ZamanSerisiAnalizService(ApplicationDbContext context, ILogger<ZamanSerisiAnalizService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TeklifTrendAnalizi> TeklifTrendleriniAnalizEtAsync(int ihaleId)
        {
            try
            {
                var ihale = await _context.Ihaleler
                    .Include(i => i.Teklifler)
                    .FirstOrDefaultAsync(i => i.IhaleId == ihaleId);

                if (ihale == null)
                {
                    throw new ArgumentException("İhale bulunamadı", nameof(ihaleId));
                }

                var teklifler = ihale.Teklifler
                    .OrderBy(t => t.TeklifTarihi)
                    .ToList();

                if (teklifler.Count < 2)
                {
                    throw new InvalidOperationException("Trend analizi için yeterli teklif yok");
                }

                var trendAnalizi = new TeklifTrendAnalizi
                {
                    IhaleId = ihaleId,
                    IhaleAdi = ihale.IhaleAdi,
                    BaslangicTarihi = teklifler.First().TeklifTarihi,
                    BitisTarihi = teklifler.Last().TeklifTarihi,
                    ToplamTeklifSayisi = teklifler.Count,
                    VeriNoktalari = new List<TeklifTrendVeriNoktasi>()
                };

                // Zaman aralıklarını belirle (örneğin günlük)
                var tarihAraliklari = Enumerable.Range(0, (trendAnalizi.BitisTarihi - trendAnalizi.BaslangicTarihi).Days + 1)
                    .Select(offset => trendAnalizi.BaslangicTarihi.AddDays(offset))
                    .ToList();

                foreach (var tarih in tarihAraliklari)
                {
                    var gununTeklifleri = teklifler
                        .Where(t => t.TeklifTarihi.Date == tarih.Date)
                        .ToList();

                    if (!gununTeklifleri.Any()) continue;

                    var veriNoktasi = new TeklifTrendVeriNoktasi
                    {
                        Tarih = tarih,
                        TeklifSayisi = gununTeklifleri.Count,
                        OrtalamaTeklif = gununTeklifleri.Average(t => t.TeklifTutari),
                        MinimumTeklif = gununTeklifleri.Min(t => t.TeklifTutari),
                        MaximumTeklif = gununTeklifleri.Max(t => t.TeklifTutari)
                    };

                    trendAnalizi.VeriNoktalari.Add(veriNoktasi);
                }

                // Regresyon analizi yap
                trendAnalizi = RegresyonAnaliziYap(trendAnalizi);

                return trendAnalizi;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklif trend analizi sırasında hata oluştu. İhale ID: {IhaleId}", ihaleId);
                throw;
            }
        }

        public async Task<List<IhaleTrendAnalizi>> IhaleTrendleriniAnalizEtAsync(IhaleTuru? ihaleTuru = null)
        {
            try
            {
                var ihaleler = await _context.Ihaleler
                    .Include(i => i.Teklifler)
                    .Include(i => i.IhaleAnalizleri)
                    .Where(i => ihaleTuru == null || i.IhaleTuru == ihaleTuru)
                    .OrderBy(i => i.IhaleTarihi)
                    .ToListAsync();

                var aylikGruplar = ihaleler
                    .GroupBy(i => new { i.IhaleTarihi.Year, i.IhaleTarihi.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .ToList();

                var trendAnalizleri = new List<IhaleTrendAnalizi>();

                foreach (var grup in aylikGruplar)
                {
                    var grupIhaleler = grup.ToList();
                    var analiz = new IhaleTrendAnalizi
                    {
                        Yil = grup.Key.Year,
                        Ay = grup.Key.Month,
                        IhaleSayisi = grupIhaleler.Count,
                        OrtalamaTeklifSayisi = grupIhaleler.Average(i => i.Teklifler.Count),
                        OrtalamaTeklifTutari = grupIhaleler
                            .SelectMany(i => i.Teklifler)
                            .Average(t => t.TeklifTutari),
                        OrtalamaRekabetSeviyesi = grupIhaleler
                            .Select(i => i.IhaleAnalizleri.OrderByDescending(a => a.AnalizTarihi).FirstOrDefault())
                            .Where(a => a != null)
                            .Average(a => a.RekabetSeviyesi)
                    };

                    trendAnalizleri.Add(analiz);
                }

                return trendAnalizleri;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale trend analizi sırasında hata oluştu. Tür: {IhaleTuru}", ihaleTuru);
                throw;
            }
        }

        private TeklifTrendAnalizi RegresyonAnaliziYap(TeklifTrendAnalizi trendAnalizi)
        {
            // Basit doğrusal regresyon (y = a + bx)
            var n = trendAnalizi.VeriNoktalari.Count;
            var xValues = trendAnalizi.VeriNoktalari
                .Select((v, i) => (double)i)
                .ToArray();
            var yValues = trendAnalizi.VeriNoktalari
                .Select(v => (double)v.OrtalamaTeklif)
                .ToArray();

            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
            for (int i = 0; i < n; i++)
            {
                sumX += xValues[i];
                sumY += yValues[i];
                sumXY += xValues[i] * yValues[i];
                sumX2 += xValues[i] * xValues[i];
            }

            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            double intercept = (sumY - slope * sumX) / n;

            trendAnalizi.Egim = (decimal)slope;
            trendAnalizi.YKesmeNoktasi = (decimal)intercept;
            trendAnalizi.TrendYonu = slope > 0 ? "Artış" : slope < 0 ? "Azalış" : "Sabit";

            // Tahminler oluştur
            for (int i = 0; i < n; i++)
            {
                trendAnalizi.VeriNoktalari[i].TahminiTeklif =
                    (decimal)(intercept + slope * xValues[i]);
            }

            return trendAnalizi;
        }
    }
}
