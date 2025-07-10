namespace Mesfel.ViewModel
{
    public class IhaleKalemViewModel
    {
        public int Id { get; set; }
        public string KalemAdi { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal Miktar { get; set; }
        public string Birim { get; set; }
        public decimal ToplamTutar => BirimFiyat * Miktar;
    }
}
