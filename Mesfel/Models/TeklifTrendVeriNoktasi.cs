namespace Mesfel.Models
{
    public class TeklifTrendVeriNoktasi
    {
        public DateTime Tarih { get; set; }
        public int TeklifSayisi { get; set; }
        public decimal OrtalamaTeklif { get; set; }
        public decimal MinimumTeklif { get; set; }
        public decimal MaximumTeklif { get; set; }
        public decimal TahminiTeklif { get; set; }
    }
}
