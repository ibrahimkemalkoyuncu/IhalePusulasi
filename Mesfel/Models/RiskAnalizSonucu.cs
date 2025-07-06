namespace Mesfel.Models
{
    public class RiskAnalizSonucu
    {
        public decimal TeklifTutari { get; set; }
        public decimal AsiriDusukTeklifSiniri { get; set; }
        public decimal AsiriDusukKalmaOlasiligi { get; set; }
        public decimal UstundeKalmaOlasiligi { get; set; }
        public TeklifAraligi OptimalTeklifAraligi { get; set; }
        public RiskSeviyesi RiskSeviyesi { get; set; }
        public List<RiskSenaryo> Senaryolar { get; set; }
        public List<string> Oneriler { get; set; }
    }
}
