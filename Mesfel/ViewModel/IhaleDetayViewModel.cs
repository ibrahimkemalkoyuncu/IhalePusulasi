using Mesfel.Models;
using Mesfel.Services;

namespace Mesfel.ViewModel
{
    /// <summary>
    /// İhale detay sayfası için view model
    /// </summary>
    public class IhaleDetayViewModel
    {
        public Ihale Ihale { get; set; } = new Ihale();
        public IhaleAnalizSonucu AnalizSonucu { get; set; } = new IhaleAnalizSonucu();
        public List<TeklifKarsilastirma> TeklifKarsilastirma { get; set; } = new List<TeklifKarsilastirma>();
        public GecimiTeminatHesapla GeciciTeminat { get; set; } = new GecimiTeminatHesapla();
    }
}
