using Mesfel.Models;
using Mesfel.Utilities;

namespace Mesfel.ViewModel
{
    public class TrendAnalizViewModel
    {
        public TeklifTrendAnalizi TeklifTrendleri { get; set; }
        public List<IhaleTrendAnalizi> IhaleTrendleri { get; set; }
        public IEnumerable<Ihale> TumIhaleler { get; set; }
        public IhaleTuru? SeciliIhaleTuru { get; set; }
    }
}
