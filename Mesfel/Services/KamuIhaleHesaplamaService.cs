using Mesfel.Models;
using Mesfel.Utilities;

namespace Mesfel.Services
{
    public class KamuIhaleHesaplamaService : IKamuIhaleHesaplamaService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KamuIhaleHesaplamaService> _logger;

        public KamuIhaleHesaplamaService(IConfiguration configuration, ILogger<KamuIhaleHesaplamaService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// 4734 sayılı Kamu İhale Kanunu'na göre aşırı düşük teklif sınırını hesaplar
        /// </summary>
        public decimal AsiriDusukTeklifSiniriHesapla(Ihale ihale, IhaleAnaliz analiz)
        {
            try
            {
                // Kanunun 38. maddesine göre hesaplama
                decimal ortalamaTeklif = analiz.OrtalamaTeklif;
                decimal standartSapma = analiz.StandartSapma;
                decimal kesifBedeli = ihale.KesifBedeli;

                // 1. Yöntem: Ortalama - (2 * Standart Sapma)
                decimal yontem1 = ortalamaTeklif - (2 * standartSapma);

                // 2. Yöntem: Ortalamanın %30 altı
                decimal yontem2 = ortalamaTeklif * 0.70m;

                // 3. Yöntem: Keşif bedelinin %60'ı (Yapım işleri için)
                decimal yontem3 = kesifBedeli * 0.60m;

                // En düşük olanı al, ancak keşif bedelinin %50'sinden az olamaz
                decimal sinir = Math.Min(Math.Min(yontem1, yontem2), yontem3);
                decimal minimumSinir = kesifBedeli * 0.50m;

                return Math.Max(sinir, minimumSinir);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aşırı düşük teklif sınırı hesaplanırken hata oluştu. İhale ID: {IhaleId}", ihale.Id);
                // Hata durumunda keşif bedelinin %60'ını döndür
                return ihale.KesifBedeli * 0.60m;
            }
        }

        /// <summary>
        /// Verilen teklifin aşırı düşük olup olmadığını kontrol eder
        /// </summary>
        public bool AsiriDusukTeklifMi(Ihale ihale, decimal teklifTutari, IhaleAnaliz analiz)
        {
            try
            {
                decimal sinir = AsiriDusukTeklifSiniriHesapla(ihale, analiz);
                return teklifTutari < sinir;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aşırı düşük teklif kontrolü yapılırken hata oluştu. İhale ID: {IhaleId}", ihale.Id);
                return false;
            }
        }

        /// <summary>
        /// İhale türüne göre alt sınırı hesaplar
        /// </summary>
        public decimal HesaplaAltSinir(IhaleTuru ihaleTuru, decimal kesifBedeli)
        {
            // 4734 sayılı Kanun'un ilgili maddelerine göre alt sınırlar
            switch (ihaleTuru)
            {
                case IhaleTuru.YapimIsi:
                    return kesifBedeli * 0.60m; // Yapım işlerinde %60
                case IhaleTuru.HizmetAlimi:
                    return kesifBedeli * 0.50m; // Hizmet alımlarında %50
                case IhaleTuru.MalAlimi:
                    return kesifBedeli * 0.40m; // Mal alımlarında %40
                case IhaleTuru.Danismanlik:
                    return kesifBedeli * 0.30m; // Danışmanlıkta %30
                default:
                    return kesifBedeli * 0.50m; // Diğer durumlarda %50
            }
        }

        /// <summary>
        /// İhale türüne göre üst sınırı hesaplar
        /// </summary>
        public decimal HesaplaUstSinir(IhaleTuru ihaleTuru, decimal kesifBedeli)
        {
            // 4734 sayılı Kanun'un ilgili maddelerine göre üst sınırlar
            switch (ihaleTuru)
            {
                case IhaleTuru.YapimIsi:
                    return kesifBedeli * 1.10m; // Yapım işlerinde %110
                case IhaleTuru.HizmetAlimi:
                    return kesifBedeli * 1.20m; // Hizmet alımlarında %120
                case IhaleTuru.MalAlimi:
                    return kesifBedeli * 1.30m; // Mal alımlarında %130
                case IhaleTuru.Danismanlik:
                    return kesifBedeli * 1.40m; // Danışmanlıkta %140
                default:
                    return kesifBedeli * 1.20m; // Diğer durumlarda %120
            }
        }

        /// <summary>
        /// Kanun kurallarına uygun optimal teklifi hesaplar
        /// </summary>
        public decimal HesaplaOptimalTeklif(Ihale ihale, IhaleAnaliz analiz)
        {
            try
            {
                decimal altSinir = HesaplaAltSinir(ihale.IhaleTuru, ihale.KesifBedeli);
                decimal ustSinir = HesaplaUstSinir(ihale.IhaleTuru, ihale.KesifBedeli);
                decimal asiriDusukSinir = AsiriDusukTeklifSiniriHesapla(ihale, analiz);

                // Ortalama teklif
                decimal ortalama = analiz.OrtalamaTeklif;

                // Optimal teklif: Ortalama ile alt sınır arasında, ancak aşırı düşük sınırın üstünde
                decimal optimalTeklif = (ortalama + altSinir) / 2;

                // Aşırı düşük sınırın altına düşmemeli
                optimalTeklif = Math.Max(optimalTeklif, asiriDusukSinir * 1.05m);

                // Üst sınırı aşmamalı
                optimalTeklif = Math.Min(optimalTeklif, ustSinir);

                return Math.Round(optimalTeklif, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Optimal teklif hesaplanırken hata oluştu. İhale ID: {IhaleId}", ihale.Id);
                // Hata durumunda keşif bedelinin %85'ini döndür
                return ihale.KesifBedeli * 0.85m;
            }
        }
    }
}
