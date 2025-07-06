using Mesfel.Models;
using Mesfel.Services;

namespace Mesfel.ViewModel
{
    /// <summary>
    /// Teklif karşılaştırma sayfası için view model
    /// </summary>
    public class TeklifKarsilastirmaViewModel
    {
        public Ihale Ihale { get; set; } = new Ihale();
        public List<TeklifKarsilastirma> TeklifKarsilastirmalari { get; set; } = new List<TeklifKarsilastirma>();
    }
}
