using Mesfel.Sabitler;

namespace Mesfel.Models
{
    public class TeklifStratejisi
    {
        public int Id { get; set; }
        public int IhaleId { get; set; }
        public decimal OnerilenTeklif { get; set; }
        public decimal KazanmaOlasiligi { get; set; }
        public RiskSeviyesi RiskSeviyesi { get; set; }
        public decimal KarMarji { get; set; }
        public string StratejiAciklamasi { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }

        // Navigation property
        public virtual Ihale Ihale { get; set; }
    }
}
