using Mesfel.Models;

namespace Mesfel.ViewModel
{
    public class IhaleAnalizViewModel
    {
        public Ihale Ihale { get; set; }
        public IhaleAnaliz Analiz { get; set; }
        public TeklifStratejisi Strateji { get; set; }
        public List<IhaleTeklif> Teklifler { get; set; }
    }
}
