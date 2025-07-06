namespace Mesfel.Models
{
    public class TeklifTrendAnalizi
    {
        public int IhaleId { get; set; }
        public string IhaleAdi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public int ToplamTeklifSayisi { get; set; }
        public decimal Egim { get; set; } // Regresyon eğimi
        public decimal YKesmeNoktasi { get; set; }
        public string TrendYonu { get; set; }
        public List<TeklifTrendVeriNoktasi> VeriNoktalari { get; set; }
    }

}
