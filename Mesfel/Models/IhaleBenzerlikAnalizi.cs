namespace Mesfel.Models
{
    public class IhaleBenzerlikAnalizi
    {
        public Ihale Ihale { get; set; }
        public IhaleAnaliz Analiz { get; set; }
        public decimal BenzerlikOrani { get; set; }
        public decimal OrtalamaTeklifFarki { get; set; }
    }
}
