using Mesfel.Models;
using Mesfel.Sabitler;

namespace Mesfel.Services
{
    public class RiskAnalizService : IRiskAnalizService
    {
        private readonly IKamuIhaleHesaplamaService _kamuIhaleHesaplama;
        private readonly ILogger<RiskAnalizService> _logger;

        public RiskAnalizService(IKamuIhaleHesaplamaService kamuIhaleHesaplama, ILogger<RiskAnalizService> logger)
        {
            _kamuIhaleHesaplama = kamuIhaleHesaplama;
            _logger = logger;
        }

        /// <summary>
        /// Kapsamlı risk analizi yapar
        /// </summary>
        public RiskAnalizSonucu RiskAnaliziYap(Ihale ihale, IhaleAnaliz analiz, decimal teklifTutari)
        {
            try
            {
                var sonuc = new RiskAnalizSonucu
                {
                    TeklifTutari = teklifTutari,
                    AsiriDusukTeklifSiniri = _kamuIhaleHesaplama.AsiriDusukTeklifSiniriHesapla(ihale, analiz),
                    AsiriDusukKalmaOlasiligi = AsiriDusukKalmaOlasiligi(ihale, analiz, teklifTutari),
                    UstundeKalmaOlasiligi = UstundeKalmaOlasiligi(ihale, analiz, teklifTutari),
                    OptimalTeklifAraligi = new TeklifAraligi
                    {
                        AltSinir = _kamuIhaleHesaplama.HesaplaAltSinir(ihale.IhaleTuru, ihale.KesifBedeli),
                        UstSinir = _kamuIhaleHesaplama.HesaplaUstSinir(ihale.IhaleTuru, ihale.KesifBedeli),
                        OptimalTeklif = _kamuIhaleHesaplama.HesaplaOptimalTeklif(ihale, analiz)
                    },
                    RiskSeviyesi = RiskSeviyesiniBelirle(ihale, analiz, teklifTutari),
                    Senaryolar = MonteCarloSimulasyonu(ihale, analiz, 500)
                };

                sonuc.Oneriler = GenerateOneriler(sonuc);

                return sonuc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Risk analizi yapılırken hata oluştu. İhale ID: {IhaleId}", ihale.IhaleId);
                throw;
            }
        }

        /// <summary>
        /// Teklifin aşırı düşük kalma olasılığını hesaplar
        /// </summary>
        public decimal AsiriDusukKalmaOlasiligi(Ihale ihale, IhaleAnaliz analiz, decimal teklifTutari)
        {
            try
            {
                decimal sinir = _kamuIhaleHesaplama.AsiriDusukTeklifSiniriHesapla(ihale, analiz);

                if (teklifTutari >= sinir)
                    return 0;

                // Normal dağılım varsayımıyla olasılık hesaplama
                decimal zScore = (sinir - analiz.OrtalamaTeklif) / analiz.StandartSapma;
                decimal teklifZScore = (teklifTutari - analiz.OrtalamaTeklif) / analiz.StandartSapma;

                // Basitleştirilmiş hesaplama
                decimal olasilik = (sinir - teklifTutari) / sinir * 100;
                return Math.Round(Math.Min(100, Math.Max(0, olasilik)), 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aşırı düşük kalma olasılığı hesaplanırken hata oluştu. İhale ID: {IhaleId}", ihale.IhaleId);
                return 0;
            }
        }

        /// <summary>
        /// Teklifin üstünde kalma olasılığını hesaplar
        /// </summary>
        public decimal UstundeKalmaOlasiligi(Ihale ihale, IhaleAnaliz analiz, decimal teklifTutari)
        {
            try
            {
                if (analiz.ToplamTeklifSayisi < 2)
                    return 0;

                // Ortalamanın üzerindeki tekliflerin oranı
                decimal ustundekiTeklifler = analiz.Teklifler.Count(t => t.TeklifTutari > teklifTutari);
                decimal oran = (ustundekiTeklifler / analiz.ToplamTeklifSayisi) * 100;

                return Math.Round(oran, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Üstünde kalma olasılığı hesaplanırken hata oluştu. İhale ID: {IhaleId}", ihale.IhaleId);
                return 0;
            }
        }

        /// <summary>
        /// Monte Carlo simülasyonu ile farklı senaryoları analiz eder
        /// </summary>
        public List<RiskSenaryo> MonteCarloSimulasyonu(Ihale ihale, IhaleAnaliz analiz, int simulasyonSayisi = 1000)
        {
            var senaryolar = new List<RiskSenaryo>();
            var random = new Random();

            for (int i = 0; i < simulasyonSayisi; i++)
            {
                // Normal dağılıma göre rastgele teklif üret
                double randomNormal = BoxMullerTransform(random.NextDouble(), random.NextDouble());
                decimal rastgeleTeklif = analiz.OrtalamaTeklif + (decimal)(randomNormal * (double)analiz.StandartSapma);

                var senaryo = new RiskSenaryo
                {
                    SenaryoNo = i + 1,
                    RastgeleTeklif = Math.Round(rastgeleTeklif, 2),
                    AsiriDusuk = rastgeleTeklif < _kamuIhaleHesaplama.AsiriDusukTeklifSiniriHesapla(ihale, analiz),
                    KazanmaDurumu = rastgeleTeklif < analiz.TahminEdilenKazananTeklif
                };

                senaryolar.Add(senaryo);
            }

            return senaryolar;
        }

        // Box-Muller dönüşümü ile normal dağılım üretme
        private double BoxMullerTransform(double u1, double u2)
        {
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }

        private RiskSeviyesi RiskSeviyesiniBelirle(Ihale ihale, IhaleAnaliz analiz, decimal teklifTutari)
        {
            decimal asiriDusukOlasilik = AsiriDusukKalmaOlasiligi(ihale, analiz, teklifTutari);
            decimal ustundeKalmaOlasilik = UstundeKalmaOlasiligi(ihale, analiz, teklifTutari);

            if (asiriDusukOlasilik > 50)
                return RiskSeviyesi.CokYuksek;
            else if (asiriDusukOlasilik > 30)
                return RiskSeviyesi.Yuksek;
            else if (ustundeKalmaOlasilik > 70)
                return RiskSeviyesi.Orta;
            else
                return RiskSeviyesi.Dusuk;
        }

        private List<string> GenerateOneriler(RiskAnalizSonucu sonuc)
        {
            var oneriler = new List<string>();

            if (sonuc.AsiriDusukKalmaOlasiligi > 30)
            {
                oneriler.Add($"Teklifiniz aşırı düşük kalma riski yüksek (%{sonuc.AsiriDusukKalmaOlasiligi}). " +
                    $"Önerilen minimum teklif: {sonuc.AsiriDusukTeklifSiniri:C}");
            }

            if (sonuc.UstundeKalmaOlasiligi > 60)
            {
                oneriler.Add($"Teklifinizin üstünde kalma olasılığı yüksek (%{sonuc.UstundeKalmaOlasiligi}). " +
                    $"Rekabetçi olmak için teklifi düşürmeyi düşünebilirsiniz.");
            }

            if (sonuc.RiskSeviyesi == RiskSeviyesi.Dusuk || sonuc.RiskSeviyesi == RiskSeviyesi.Orta)
            {
                oneriler.Add($"Risk seviyeniz kabul edilebilir düzeyde. " +
                    $"Optimal teklif aralığı: {sonuc.OptimalTeklifAraligi.AltSinir:C} - {sonuc.OptimalTeklifAraligi.UstSinir:C}");
            }

            if (sonuc.TeklifTutari < sonuc.OptimalTeklifAraligi.AltSinir)
            {
                oneriler.Add($"Teklifiniz önerilen alt sınırın altında. " +
                    $"Minimum önerilen teklif: {sonuc.OptimalTeklifAraligi.AltSinir:C}");
            }

            if (sonuc.TeklifTutari > sonuc.OptimalTeklifAraligi.UstSinir)
            {
                oneriler.Add($"Teklifiniz önerilen üst sınırın üzerinde. " +
                    $"Maksimum önerilen teklif: {sonuc.OptimalTeklifAraligi.UstSinir:C}");
            }

            if (!oneriler.Any())
            {
                oneriler.Add("Teklifiniz optimal aralıkta görünüyor. Risk seviyesi düşük.");
            }

            return oneriler;
        }
    }
}