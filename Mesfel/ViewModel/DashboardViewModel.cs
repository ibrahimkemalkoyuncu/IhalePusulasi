using Mesfel.Models;
using System;
using System.Collections.Generic;

namespace Mesfel.ViewModel
{
    /// <summary>
    /// Dashboard ana sayfa için view model
    /// </summary>
    public class DashboardViewModel
    {
        // İhale istatistikleri
        public int ToplamIhale { get; set; }
        public int AktifIhale { get; set; }
        public int ToplamTeklif { get; set; }
        public decimal ToplamDeger { get; set; }

        // Risk analizi istatistikleri
        public int DusukRisk { get; set; }
        public int OrtaRisk { get; set; }
        public int YuksekRisk { get; set; }

        // Kategori dağılımları
        public Dictionary<string, int> KategoriDagilimlari { get; set; } = new Dictionary<string, int>();

        // Trend analizi verileri
        public List<IhaleTrendAnalizi> IhaleTrendleri { get; set; } = new List<IhaleTrendAnalizi>();

        // Son aktiviteler
        public List<Aktivite> SonAktiviteler { get; set; } = new List<Aktivite>();

        // Öne çıkan ihaleler
        public List<Ihale> OneCikanIhaleler { get; set; } = new List<Ihale>();

        // Son ihaleler
        public List<Ihale> SonIhaleler { get; set; } = new List<Ihale>();

        // Aktif ihaleler
        public List<Ihale> AktifIhaleler { get; set; } = new List<Ihale>();

        // İstatistikler
        public List<IstatistikVeri> Istatistikler { get; set; } = new List<IstatistikVeri>();
    }

    /// <summary>
    /// Dashboard'da gösterilecek aktiviteler için model
    /// </summary>
    public class Aktivite
    {
        public string Baslik { get; set; }
        public string Aciklama { get; set; }
        public DateTime Tarih { get; set; }
        public string Icon { get; set; }
    }

    /// <summary>
    /// İstatistik verileri için model
    /// </summary>
    public class IstatistikVeri
    {
        public string Baslik { get; set; }
        public int Deger { get; set; }
        public string Icon { get; set; }
        public string Renk { get; set; }
    }
}