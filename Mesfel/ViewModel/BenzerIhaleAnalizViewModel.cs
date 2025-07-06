using Mesfel.Models;

namespace Mesfel.ViewModel
{
    public class BenzerIhaleAnalizViewModel
    {
        public Ihale HedefIhale { get; set; }
        public List<IhaleBenzerlikAnalizi> BenzerIhaleler { get; set; }
    }
}
