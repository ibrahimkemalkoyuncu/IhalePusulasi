using Mesfel.Models;

namespace Mesfel.ViewModel
{
    public class KarsilastirmaViewModel
    {
        public IhaleKarsilastirmaSonucu Sonuc { get; set; }
        public IEnumerable<Ihale> TumIhaleler { get; set; }
        public List<IhaleBenzerlikAnalizi> BenzerIhaleler1 { get; set; }
        public List<IhaleBenzerlikAnalizi> BenzerIhaleler2 { get; set; }
    }
}
