namespace Mesfel.Models
{
    public class IhaleKarsilastirmaSonucu
    {
        public Ihale Ihale1 { get; set; }
        public Ihale Ihale2 { get; set; }
        public IhaleAnaliz Analiz1 { get; set; }
        public IhaleAnaliz Analiz2 { get; set; }
        public decimal BenzerlikOrani { get; set; } // 0-1 arası
        public List<string> Farkliliklar { get; set; }

    }
}
