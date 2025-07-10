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
        /// Belirtilen ihale ID'sine göre ihale analizi yapar
        /// </summary>
        /// <param name="ihaleId">Analiz yapılacak ihale ID'si</param>
        /// <returns>İhale analiz sonuçlarını içeren nesne</returns>
        public async Task<IhaleAnalizSonucu> IhaleAnalizYapAsync(int ihaleId)
        {
            try
            {
                // İhaleyi veritabanından getir (teklifler ve kalemler dahil)
                var ihale = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .Include(i => i.IhaleKalemleri)
                    .FirstOrDefaultAsync(i => i.Id == ihaleId);

                if (ihale == null)
                    throw new ArgumentException("İhale bulunamadı");

                // Sadece verilmiş teklifleri filtrele
                var teklifler = ihale.IhaleTeklifleri.Where(t => t.TeklifDurumu == TeklifDurumu.Verildi).ToList();

                // Analiz sonuçlarını oluştur
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
        /// Belirtilen kar oranına göre optimal teklif hesaplar
        /// </summary>
        /// <param name="ihaleId">Hesaplama yapılacak ihale ID'si</param>
        /// <param name="hedefKarOrani">Hedeflenen kar oranı (yüzde olarak)</param>
        /// <returns>Optimal teklif sonuçlarını içeren nesne</returns>
        public async Task<OptimalTeklifSonucu> OptimalTeklifHesaplaAsync(int ihaleId, decimal hedefKarOrani)
        {
            try
            {
                // İhaleyi ve tekliflerini getir
                var ihale = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .FirstOrDefaultAsync(i => i.Id == ihaleId);

                if (ihale == null)
                    throw new ArgumentException("İhale bulunamadı");

                var teklifler = ihale.IhaleTeklifleri.Where(t => t.TeklifDurumu == TeklifDurumu.Verildi).ToList();

                // Benzer ihaleleri getir (türüne göre)
                var benzerIhaleler = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .Where(i => i.IhaleTuru == ihale.IhaleTuru && i.Id != ihaleId)
                    .Take(10)
                    .ToListAsync();

                // Optimal teklif sonuçlarını oluştur
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
        /// İhaleye verilen teklifleri karşılaştırır
        /// </summary>
        /// <param name="ihaleId">Karşılaştırma yapılacak ihale ID'si</param>
        /// <returns>Teklif karşılaştırma sonuçlarını içeren liste</returns>
        public async Task<List<TeklifKarsilastirma>> TeklifKarsilastirmaYapAsync(int ihaleId)
        {
            try
            {
                // İhaleyi ve tekliflerini getir
                var ihale = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .FirstOrDefaultAsync(i => i.Id == ihaleId);

                if (ihale == null)
                    throw new ArgumentException("İhale bulunamadı");

                // Sadece verilmiş teklifleri al
                var teklifler = ihale.IhaleTeklifleri.Where(t => t.TeklifDurumu == TeklifDurumu.Verildi).ToList();
                var karsilastirmaListesi = new List<TeklifKarsilastirma>();

                // Her teklif için karşılaştırma verileri oluştur
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

                // Teklifleri sırala ve fark yüzdelerini hesapla
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
        /// Geçici teminat tutarını hesaplar
        /// </summary>
        /// <param name="yaklasikMaliyet">İhalenin yaklaşık maliyeti</param>
        /// <param name="ihaleTuru">İhale türü</param>
        /// <returns>Geçici teminat hesaplama sonuçları</returns>
        public async Task<GecimiTeminatHesapla> GecimiTeminatHesaplaAsync(decimal yaklasikMaliyet, IhaleTuru ihaleTuru)
        {
            // 4734 sayılı Kamu İhale Kanunu'na göre geçici teminat oranları
            double teminatOrani = ihaleTuru switch
            {
                IhaleTuru.YapimIsi => 0.02, // %2 (inşaat işleri)
                IhaleTuru.MalAlimi => 0.015, // %1.5 (mal alımları)
                IhaleTuru.HizmetAlimi => 0.015, // %1.5 (hizmet alımları)
                _ => 0.02 // Diğer türler için varsayılan %2
            };

            // Teminat tutarını hesapla
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
        /// Kesin teminat tutarını hesaplar
        /// </summary>
        /// <param name="teklifTutari">Teklif edilen tutar</param>
        /// <param name="teminatOrani">Teminat oranı (yüzde olarak)</param>
        /// <returns>Kesin teminat hesaplama sonuçları</returns>
        public async Task<KesinTeminatHesapla> KesinTeminatHesaplaAsync(decimal teklifTutari, double teminatOrani)
        {
            // Teminat tutarını hesapla (teklif tutarı x oran)
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
                var toplamIhaleSayisi = await _context.Ihaleler.CountAsync();
                istatistikler.Add(new IstatistikVeri
                {
                    Baslik = "Toplam İhale Sayısı",
                    Deger = toplamIhaleSayisi,
                    Tur = 0
                });

                // Aktif ihaleler
                var aktifIhaleSayisi = await _context.Ihaleler
                    .CountAsync(i => i.IhaleDurumu == IhaleDurumu.TeklifAlma.ToString() ||
                                     i.IhaleDurumu == IhaleDurumu.IlanEdildi.ToString());
                istatistikler.Add(new IstatistikVeri
                {
                    Baslik = "Aktif İhale Sayısı",
                    Deger = aktifIhaleSayisi,
                    Tur = 0
                });

                // Tamamlanan ihaleler
                var tamamlananIhaleSayisi = await _context.Ihaleler
                    .CountAsync(i => i.IhaleDurumu == IhaleDurumu.Sonuclandi.ToString());
                istatistikler.Add(new IstatistikVeri
                {
                    Baslik = "Tamamlanan İhale Sayısı",
                    Deger = tamamlananIhaleSayisi,
                    Tur = 0
                });

                // Ortalama teklif farkı (örnek hesaplama)
                var tamamlananIhaleler = await _context.Ihaleler
                    .Include(i => i.IhaleTeklifleri)
                    .Where(i => i.IhaleDurumu == IhaleDurumu.Sonuclandi.ToString() && i.IhaleTeklifleri.Count > 0)
                    .ToListAsync();

                if (tamamlananIhaleler.Any())
                {
                    var ortalamaTeklifFarki = (double)tamamlananIhaleler
                        .Average(i => (i.YaklasikMaliyet - i.IhaleTeklifleri.Min(t => t.TeklifTutari)) /
                                      i.YaklasikMaliyet * 100);
                    istatistikler.Add(new IstatistikVeri
                    {
                        Baslik = "Ortalama Teklif Farkı (%)",
                        Deger = ((int)ortalamaTeklifFarki),
                        Tur = 1
                    });
                }

                return istatistikler;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstatistikler getirilirken hata oluştu");
                throw;
            }
        }



        #region Private Helper Methods

        /// <summary>
        /// Verilen liste için ortanca (medyan) değeri hesaplar
        /// </summary>
        /// <param name="degerler">Hesaplanacak değerler listesi</param>
        /// <returns>Ortanca değer</returns>
        private decimal HesaplaOrtanca(List<decimal> degerler)
        {
            // Listeyi küçükten büyüğe sırala
            var siraliDegerler = degerler.OrderBy(d => d).ToList();
            int count = siraliDegerler.Count;

            // Liste boşsa 0 döndür
            if (count == 0) return 0;

            // Çift sayıda eleman varsa ortadaki iki sayının ortalamasını al
            if (count % 2 == 0)
                return (siraliDegerler[count / 2 - 1] + siraliDegerler[count / 2]) / 2;
            // Tek sayıda eleman varsa ortadaki sayıyı döndür
            else
                return siraliDegerler[count / 2];
        }

        /// <summary>
        /// Teklifler arasındaki rekabet düzeyini ölçer
        /// </summary>
        /// <param name="teklifler">Teklif listesi</param>
        /// <returns>Rekabet indeksi değeri</returns>
        private double HesaplaRekabetIndeksi(List<IhaleTeklif> teklifler)
        {
            // En az 2 teklif yoksa rekabet yok say
            if (teklifler.Count < 2) return 0;

            // En düşük ve en yüksek teklifleri bul
            var enDusuk = teklifler.Min(t => t.TeklifTutari);
            var enYuksek = teklifler.Max(t => t.TeklifTutari);

            // Sıfıra bölme hatasını önle
            if (enDusuk == 0) return 0;

            // Fark yüzdesini hesapla: ((en yüksek - en düşük) / en düşük) * 100
            return (double)((enYuksek - enDusuk) / enDusuk * 100);
        }

        /// <summary>
        /// Tekliflere göre piyasa durumunu analiz eder
        /// </summary>
        /// <param name="teklifler">Teklif listesi</param>
        /// <param name="yaklasikMaliyet">İhalenin yaklaşık maliyeti</param>
        /// <returns>Piyasa durumu açıklaması</returns>
        private string PiyasaAnalizYap(List<IhaleTeklif> teklifler, decimal yaklasikMaliyet)
        {
            // Teklif yoksa bilgi ver
            if (!teklifler.Any()) return "Henüz teklif verilmemiş";

            // Ortalama teklifi hesapla
            var ortalama = teklifler.Average(t => t.TeklifTutari);

            // Ortalama teklifin yaklaşık maliyete oranını hesapla
            var yaklasikMaliyetOrani = (double)(ortalama / yaklasikMaliyet * 100);

            // Orana göre piyasa durumunu belirle
            return yaklasikMaliyetOrani switch
            {
                < 80 => "Agresif fiyatlandırma - Yüksek rekabet",
                < 90 => "Rekabetçi piyasa - Orta risk",
                < 100 => "Dengeli piyasa - Düşük risk",
                _ => "Yüksek fiyatlandırma - Düşük rekabet"
            };
        }

        /// <summary>
        /// Optimal teklif tutarını hesaplar
        /// </summary>
        /// <param name="ihale">İhale bilgileri</param>
        /// <param name="teklifler">Mevcut teklifler</param>
        /// <param name="benzerIhaleler">Benzer ihaleler</param>
        /// <param name="hedefKarOrani">Hedeflenen kar oranı</param>
        /// <returns>Optimal teklif tutarı</returns>
        private decimal HesaplaOptimalTeklif(Ihale ihale, List<IhaleTeklif> teklifler, List<Ihale> benzerIhaleler, decimal hedefKarOrani)
        {
            // Temel hedef: Maliyet + Hedef kar marjı
            var hedefTutar = ihale.YaklasikMaliyet * (1 + hedefKarOrani / 100);

            // Rakip teklifleri analiz et
            if (teklifler.Any())
            {
                var enDusukTeklif = teklifler.Min(t => t.TeklifTutari);
                // En düşük tekliften %1 daha düşük teklif ver
                var optimalTeklif = Math.Min(hedefTutar, enDusukTeklif * 0.99m);
                return optimalTeklif;
            }

            // Benzer ihaleleri analiz et
            if (benzerIhaleler.Any())
            {
                // Benzer ihalelerdeki ortalama (en düşük teklif / yaklaşık maliyet) oranını bul
                var benzerOrtalamaOran = benzerIhaleler
                    .Where(i => i.IhaleTeklifleri.Any())
                    .Average(i => (double)(i.IhaleTeklifleri.Min(t => t.TeklifTutari) / i.YaklasikMaliyet));

                // Bu orana göre teklif hesapla
                var benzerTabanliTeklif = ihale.YaklasikMaliyet * (decimal)benzerOrtalamaOran;
                return Math.Min(hedefTutar, benzerTabanliTeklif);
            }

            // Başka veri yoksa hedef tutarı döndür
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

        /// <summary>
        /// İhale için risk analizi yapar
        /// </summary>
        /// <param name="ihale">İhale bilgileri</param>
        /// <param name="teklifler">Mevcut teklifler</param>
        /// <returns>Risk durumu açıklaması</returns>
        private string RiskAnaliziYap(Ihale ihale, List<IhaleTeklif> teklifler)
        {
            var riskler = new List<string>();

            // Çok fazla teklif varsa yüksek rekabet riski
            if (teklifler.Count > 10)
                riskler.Add("Yüksek rekabet");

            // Anormal düşük teklifler varsa
            if (teklifler.Any(t => t.TeklifTutari < ihale.YaklasikMaliyet * 0.7m))
                riskler.Add("Anormal düşük teklifler");

            // Süre daralmışsa
            if (DateTime.Now > ihale.TeklifSonTarihi.AddDays(-3))
                riskler.Add("Süre baskısı");

            // Risk varsa listele, yoksa düşük risk bildir
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

    /// <summary>
    /// İhale analiz sonuçlarını tutan sınıf
    /// </summary>
    public class IhaleAnalizSonucu
    {
        public int IhaleId { get; set; } // İhale ID'si
        public string IhaleAdi { get; set; } = string.Empty; // İhale adı
        public decimal YaklasikMaliyet { get; set; } // Yaklaşık maliyet
        public int ToplamTeklifSayisi { get; set; } // Toplam teklif sayısı
        public decimal EnDusukTeklif { get; set; } // En düşük teklif tutarı
        public decimal EnYuksekTeklif { get; set; } // En yüksek teklif tutarı
        public decimal OrtalamaTeklif { get; set; } // Ortalama teklif tutarı
        public decimal Ortanca { get; set; } // Ortanca (medyan) teklif tutarı
        public decimal KazananTeklif { get; set; } // Kazanan teklif tutarı
        public string KazananFirma { get; set; } = string.Empty; // Kazanan firma adı
        public double RekabetIndeksi { get; set; } // Rekabet düzeyi indeksi
        public string PiyasaAnalizi { get; set; } = string.Empty; // Piyasa durumu analizi
    }

    /// <summary>
    /// Optimal teklif hesaplama sonuçlarını tutan sınıf
    /// </summary>
    public class OptimalTeklifSonucu
    {
        public int IhaleId { get; set; } // İhale ID'si
        public decimal YaklasikMaliyet { get; set; } // Yaklaşık maliyet
        public decimal HedefKarOrani { get; set; } // Hedeflenen kar oranı (%)
        public decimal OptimalTeklifTutari { get; set; } // Hesaplanan optimal teklif tutarı
        public double KazanmaOlasiligi { get; set; } // Kazanma olasılığı (%)
        public string RiskAnalizi { get; set; } = string.Empty; // Risk analizi sonucu
        public List<string> Oneriler { get; set; } = new List<string>(); // Öneriler listesi
    }

    /// <summary>
    /// Teklif karşılaştırma bilgilerini tutan sınıf
    /// </summary>
    public class TeklifKarsilastirma
    {
        public int TeklifId { get; set; } // Teklif ID'si
        public string FirmaAdi { get; set; } // Firma adı
        public decimal TeklifTutari { get; set; } // Teklif tutarı
        public double YaklasikMaliyetOrani { get; set; } // Yaklaşık maliyete oranı (%)
        public int Siralama { get; set; } // Sıralama (1 en iyi)
        public double FarkYuzdesi { get; set; } // En düşük teklife göre fark (%)
        public double KazanmaOlasiligi { get; set; } // Kazanma olasılığı (%)
        public string RiskDurumu { get; set; } = string.Empty; // Risk durumu
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
        public int Deger { get; set; } 
        public int Tur { get; set; } 
    }

    public class IhaleIstatistikleri
    {
        public int ToplamIhaleSayisi { get; set; }
        public int AktifIhaleSayisi { get; set; }
        public int TamamlananIhaleSayisi { get; set; }
        public double OrtalamaTeklifFarki { get; set; }
    }

    #endregion
}