using Mesfel.Data;
using Mesfel.Models;
using Mesfel.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Mesfel.Services
{
    public class IhaleHesaplamaService : IIhaleHesaplamaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IhaleHesaplamaService> _logger;

        public IhaleHesaplamaService(ApplicationDbContext context, ILogger<IhaleHesaplamaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// İhale analiz sonuçlarını hesaplar
        /// </summary>
        public async Task<IhaleAnalizSonucu> IhaleAnalizYapAsync(int ihaleId)
        {
            try
            {
                var ihale = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .Include(i => i.IhaleKalemleri)
                    .FirstOrDefaultAsync(i => i.Id == ihaleId);

                if (ihale == null)
                    throw new ArgumentException("İhale bulunamadı");

                var teklifler = ihale.IhaleTeklifleri.Where(t => t.TeklifDurumu == TeklifDurumu.Verildi).ToList();

                var sonuc = new IhaleAnalizSonucu
                {
                    IhaleId = ihaleId,
                    IhaleAdi = ihale.IhaleAdi,
                    YaklasikMaliyet = ihale.YaklasikMaliyet,
                    ToplamTeklifSayisi = teklifler.Count,
                    EnDusukTeklif = teklifler.Any() ? teklifler.Min(t => t.TeklifTutari) : 0,
                    EnYuksekTeklif = teklifler.Any() ? teklifler.Max(t => t.TeklifTutari) : 0,
                    OrtalamaTeklif = teklifler.Any() ? teklifler.Average(t => t.TeklifTutari) : 0,
                    Ortanca = teklifler.Any() ? HesaplaOrtanca(teklifler.Select(t => t.TeklifTutari).ToList()) : 0,
                    KazananTeklif = teklifler.Any() ? teklifler.OrderBy(t => t.TeklifTutari).First().TeklifTutari : 0,
                    KazananFirma = teklifler.Any() ? teklifler.OrderBy(t => t.TeklifTutari).First().FirmaAdi : "",
                    RekabetIndeksi = HesaplaRekabetIndeksi(teklifler),
                    PiyasaAnalizi = PiyasaAnalizYap(teklifler, ihale.YaklasikMaliyet)
                };

                return sonuc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale analizi yapılırken hata oluştu. İhale ID: {IhaleId}", ihaleId);
                throw;
            }
        }

        /// <summary>
        /// Optimal teklif hesaplar
        /// </summary>
        public async Task<OptimalTeklifSonucu> OptimalTeklifHesaplaAsync(int ihaleId, decimal hedefKarOrani)
        {
            try
            {
                var ihale = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .FirstOrDefaultAsync(i => i.Id == ihaleId);

                if (ihale == null)
                    throw new ArgumentException("İhale bulunamadı");

                var teklifler = ihale.IhaleTeklifleri.Where(t => t.TeklifDurumu == TeklifDurumu.Verildi).ToList();

                // Geçmiş verilerden trend analizi
                var benzerIhaleler = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .Where(i => i.IhaleTuru == ihale.IhaleTuru && i.Id != ihaleId)
                    .Take(10)
                    .ToListAsync();

                var sonuc = new OptimalTeklifSonucu
                {
                    IhaleId = ihaleId,
                    YaklasikMaliyet = ihale.YaklasikMaliyet,
                    HedefKarOrani = hedefKarOrani,
                    OptimalTeklifTutari = HesaplaOptimalTeklif(ihale, teklifler, benzerIhaleler, hedefKarOrani),
                    KazanmaOlasiligi = HesaplaKazanmaOlasiligi(ihale, teklifler, benzerIhaleler),
                    RiskAnalizi = RiskAnaliziYap(ihale, teklifler),
                    Oneriler = OptimalTeklifOnerilerOlustur(ihale, teklifler, hedefKarOrani)
                };

                return sonuc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Optimal teklif hesaplanırken hata oluştu. İhale ID: {IhaleId}", ihaleId);
                throw;
            }
        }

        /// <summary>
        /// Teklif karşılaştırması yapar
        /// </summary>
        public async Task<List<TeklifKarsilastirma>> TeklifKarsilastirmaYapAsync(int ihaleId)
        {
            try
            {
                var ihale = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .FirstOrDefaultAsync(i => i.Id == ihaleId);

                if (ihale == null)
                    throw new ArgumentException("İhale bulunamadı");

                var teklifler = ihale.IhaleTeklifleri.Where(t => t.TeklifDurumu == TeklifDurumu.Verildi).ToList();
                var karsilastirmaListesi = new List<TeklifKarsilastirma>();

                foreach (var teklif in teklifler)
                {
                    var karsilastirma = new TeklifKarsilastirma
                    {
                        TeklifId = teklif.Id,
                        FirmaAdi = teklif.FirmaAdi,
                        TeklifTutari = teklif.TeklifTutari,
                        YaklasikMaliyetOrani = (double)((teklif.TeklifTutari / ihale.YaklasikMaliyet) * 100),
                        Siralama = 0, // Sonra hesaplanacak
                        FarkYuzdesi = 0, // Sonra hesaplanacak
                        KazanmaOlasiligi = HesaplaTeklifKazanmaOlasiligi(teklif, teklifler),
                        RiskDurumu = TeklifRiskDurumunuBelirle(teklif, ihale.YaklasikMaliyet)
                    };
                    karsilastirmaListesi.Add(karsilastirma);
                }

                // Sıralama ve fark hesaplama
                var siraliListe = karsilastirmaListesi.OrderBy(k => k.TeklifTutari).ToList();
                for (int i = 0; i < siraliListe.Count; i++)
                {
                    siraliListe[i].Siralama = i + 1;
                    if (i > 0)
                    {
                        siraliListe[i].FarkYuzdesi = (double)(((siraliListe[i].TeklifTutari - siraliListe[0].TeklifTutari) / siraliListe[0].TeklifTutari) * 100);
                    }
                }

                return siraliListe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklif karşılaştırması yapılırken hata oluştu. İhale ID: {IhaleId}", ihaleId);
                throw;
            }
        }

        /// <summary>
        /// Geçici teminat hesaplar
        /// </summary>
        public async Task<GecimiTeminatHesapla> GecimiTeminatHesaplaAsync(decimal yaklasikMaliyet, IhaleTuru ihaleTuru)
        {
            // 4734 sayılı Kamu İhale Kanunu'na göre geçici teminat oranları
            double teminatOrani = ihaleTuru switch
            {
                IhaleTuru.YapimIsi => 0.02, // %2
                IhaleTuru.MalAlimi => 0.015, // %1.5
                IhaleTuru.HizmetAlimi => 0.015, // %1.5
                _ => 0.02
            };

            var teminatTutari = yaklasikMaliyet * (decimal)teminatOrani;

            return new GecimiTeminatHesapla
            {
                YaklasikMaliyet = yaklasikMaliyet,
                TeminatOrani = teminatOrani,
                TeminatTutari = teminatTutari,
                IhaleTuru = ihaleTuru,
                Aciklama = $"{ihaleTuru} için geçici teminat oranı %{teminatOrani * 100:F1}"
            };
        }

        /// <summary>
        /// Kesin teminat hesaplar
        /// </summary>
        public async Task<KesinTeminatHesapla> KesinTeminatHesaplaAsync(decimal teklifTutari, double teminatOrani)
        {
            var teminatTutari = teklifTutari * (decimal)(teminatOrani / 100);

            return new KesinTeminatHesapla
            {
                TeklifTutari = teklifTutari,
                TeminatOrani = teminatOrani,
                TeminatTutari = teminatTutari,
                Aciklama = $"Kesin teminat oranı %{teminatOrani}"
            };
        }

        /// <summary>
        /// İhale istatistikleri getirir
        /// </summary>
        public async Task<List<IstatistikVeri>> IhaleIstatistikleriGetirAsync()
        {
            var istatistikler = new List<IstatistikVeri>();

            try
            {
                // Toplam ihale sayısı
                var toplamIhale = await _context.Ihaleler.CountAsync();
                istatistikler.Add(new IstatistikVeri { Baslik = "Toplam İhale", Deger = toplamIhale.ToString(), Tur = "Sayı" });

                // Aktif ihaleler
                var aktifIhaleler = await _context.Ihaleler
                        .CountAsync(i => i.IhaleDurumu == IhaleDurumu.TeklifAlma.ToString() || i.IhaleDurumu == IhaleDurumu.IlanEdildi.ToString());
                istatistikler.Add(new IstatistikVeri { Baslik = "Aktif İhale", Deger = aktifIhaleler.ToString(), Tur = "Sayı" });

                // Toplam ihale hacmi
                var toplamHacim = await _context.Ihaleler.SumAsync(i => i.YaklasikMaliyet);
                istatistikler.Add(new IstatistikVeri { Baslik = "Toplam İhale Hacmi", Deger = toplamHacim.ToString("N0") + " TL", Tur = "Para" });

                // Ortalama teklif sayısı
                var ortalamaTeklifSayisi = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .Where(i => i.IhaleTeklifleri.Any())
                    .AverageAsync(i => i.IhaleTeklifleri.Count);
                istatistikler.Add(new IstatistikVeri { Baslik = "Ortalama Teklif Sayısı", Deger = ortalamaTeklifSayisi.ToString("F1"), Tur = "Sayı" });

                return istatistikler;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstatistikler getirilirken hata oluştu");
                throw;
            }
        }



        #region Private Helper Methods

        private decimal HesaplaOrtanca(List<decimal> degerler)
        {
            var siraliDegerler = degerler.OrderBy(d => d).ToList();
            int count = siraliDegerler.Count;
            if (count == 0) return 0;
            if (count % 2 == 0)
                return (siraliDegerler[count / 2 - 1] + siraliDegerler[count / 2]) / 2;
            else
                return siraliDegerler[count / 2];
        }

        private double HesaplaRekabetIndeksi(List<IhaleTeklif> teklifler)
        {
            if (teklifler.Count < 2) return 0;

            var enDusuk = teklifler.Min(t => t.TeklifTutari);
            var enYuksek = teklifler.Max(t => t.TeklifTutari);

            if (enDusuk == 0) return 0;
            return (double)((enYuksek - enDusuk) / enDusuk * 100);
        }

        private string PiyasaAnalizYap(List<IhaleTeklif> teklifler, decimal yaklasikMaliyet)
        {
            if (!teklifler.Any()) return "Henüz teklif verilmemiş";

            var ortalama = teklifler.Average(t => t.TeklifTutari);
            var yaklasikMaliyetOrani = (double)(ortalama / yaklasikMaliyet * 100);

            return yaklasikMaliyetOrani switch
            {
                < 80 => "Agresif fiyatlandırma - Yüksek rekabet",
                < 90 => "Rekabetçi piyasa - Orta risk",
                < 100 => "Dengeli piyasa - Düşük risk",
                _ => "Yüksek fiyatlandırma - Düşük rekabet"
            };
        }

        private decimal HesaplaOptimalTeklif(Ihale ihale, List<IhaleTeklif> teklifler, List<Ihale> benzerIhaleler, decimal hedefKarOrani)
        {
            // Maliyet + Hedef kar marjı
            var hedefTutar = ihale.YaklasikMaliyet * (1 + hedefKarOrani / 100);

            // Rakip analizi
            if (teklifler.Any())
            {
                var enDusukTeklif = teklifler.Min(t => t.TeklifTutari);
                var optimalTeklif = Math.Min(hedefTutar, enDusukTeklif * 0.99m); // %1 daha düşük
                return optimalTeklif;
            }

            // Benzer ihale analizi
            if (benzerIhaleler.Any())
            {
                var benzerOrtalamaOran = benzerIhaleler
                    .Where(i => i.IhaleTeklifleri.Any())
                    .Average(i => (double)(i.IhaleTeklifleri.Min(t => t.TeklifTutari) / i.YaklasikMaliyet));

                var benzerTabanliTeklif = ihale.YaklasikMaliyet * (decimal)benzerOrtalamaOran;
                return Math.Min(hedefTutar, benzerTabanliTeklif);
            }

            return hedefTutar;
        }

        private double HesaplaKazanmaOlasiligi(Ihale ihale, List<IhaleTeklif> teklifler, List<Ihale> benzerIhaleler)
        {
            // Basit olasılık hesaplaması
            if (!teklifler.Any()) return 80.0; // Teklif yoksa yüksek şans

            var rekabetSayisi = teklifler.Count + 1; // +1 bizim teklif
            return Math.Max(10.0, 100.0 / rekabetSayisi);
        }

        private double HesaplaTeklifKazanmaOlasiligi(IhaleTeklif teklif, List<IhaleTeklif> tumTeklifler)
        {
            var siralama = tumTeklifler.OrderBy(t => t.TeklifTutari).ToList().FindIndex(t => t.Id == teklif.Id) + 1;
            return siralama == 1 ? 90.0 : Math.Max(5.0, 50.0 / siralama);
        }

        private string TeklifRiskDurumunuBelirle(IhaleTeklif teklif, decimal yaklasikMaliyet)
        {
            var oran = (double)(teklif.TeklifTutari / yaklasikMaliyet * 100);
            return oran switch
            {
                < 70 => "Yüksek Risk",
                < 85 => "Orta Risk",
                < 100 => "Düşük Risk",
                _ => "Güvenli"
            };
        }

        private string RiskAnaliziYap(Ihale ihale, List<IhaleTeklif> teklifler)
        {
            var riskler = new List<string>();

            if (teklifler.Count > 10)
                riskler.Add("Yüksek rekabet");

            if (teklifler.Any(t => t.TeklifTutari < ihale.YaklasikMaliyet * 0.7m))
                riskler.Add("Anormal düşük teklifler");

            if (DateTime.Now > ihale.TeklifSonTarihi.AddDays(-3))
                riskler.Add("Süre baskısı");

            return riskler.Any() ? string.Join(", ", riskler) : "Düşük risk";
        }

        private List<string> OptimalTeklifOnerilerOlustur(Ihale ihale, List<IhaleTeklif> teklifler, decimal hedefKarOrani)
        {
            var oneriler = new List<string>();

            oneriler.Add($"Hedef kar oranı: %{hedefKarOrani}");

            if (teklifler.Any())
            {
                var enDusuk = teklifler.Min(t => t.TeklifTutari);
                oneriler.Add($"En düşük tekliften %1-2 düşük teklif verin");
                oneriler.Add($"Mevcut en düşük teklif: {enDusuk:N0} TL");
            }

            oneriler.Add("Geçici teminat mektubunuzu hazırlayın");
            oneriler.Add("Teknik şartnameleri dikkatli inceleyin");

            return oneriler;
        }

        private decimal HesaplaStandartSapma(List<decimal> teklifler, decimal ortalama)
        {
            if (teklifler.Count < 2) return 0;

            var karelerToplami = teklifler.Sum(t => Math.Pow((double)(t - ortalama), 2));
            var varyans = karelerToplami / (teklifler.Count - 1);
            return (decimal)Math.Sqrt(varyans);
        }

        #endregion
    }

    #region DTO Classes

    public class IhaleAnalizSonucu
    {
        public int IhaleId { get; set; }
        public string IhaleAdi { get; set; } = string.Empty;
        public decimal YaklasikMaliyet { get; set; }
        public int ToplamTeklifSayisi { get; set; }
        public decimal EnDusukTeklif { get; set; }
        public decimal EnYuksekTeklif { get; set; }
        public decimal OrtalamaTeklif { get; set; }
        public decimal Ortanca { get; set; }
        public decimal KazananTeklif { get; set; }
        public string KazananFirma { get; set; } = string.Empty;
        public double RekabetIndeksi { get; set; }
        public string PiyasaAnalizi { get; set; } = string.Empty;
    }

    public class OptimalTeklifSonucu
    {
        public int IhaleId { get; set; }
        public decimal YaklasikMaliyet { get; set; }
        public decimal HedefKarOrani { get; set; }
        public decimal OptimalTeklifTutari { get; set; }
        public double KazanmaOlasiligi { get; set; }
        public string RiskAnalizi { get; set; } = string.Empty;
        public List<string> Oneriler { get; set; } = new List<string>();
    }

    public class TeklifKarsilastirma
    {
        public int TeklifId { get; set; }
        public string FirmaAdi { get; set; }
        public decimal TeklifTutari { get; set; }
        public double YaklasikMaliyetOrani { get; set; }
        public int Siralama { get; set; }
        public double FarkYuzdesi { get; set; }
        public double KazanmaOlasiligi { get; set; }
        public string RiskDurumu { get; set; } = string.Empty;
    }

    public class GecimiTeminatHesapla
    {
        public decimal YaklasikMaliyet { get; set; }
        public double TeminatOrani { get; set; }
        public decimal TeminatTutari { get; set; }
        public IhaleTuru IhaleTuru { get; set; }
        public string Aciklama { get; set; } = string.Empty;
    }

    public class KesinTeminatHesapla
    {
        public decimal TeklifTutari { get; set; }
        public double TeminatOrani { get; set; }
        public decimal TeminatTutari { get; set; }
        public string Aciklama { get; set; } = string.Empty;
    }

    public class IstatistikVeri
    {
        public string Baslik { get; set; } = string.Empty;
        public string Deger { get; set; } = string.Empty;
        public string Tur { get; set; } = string.Empty;
    }

    #endregion
}