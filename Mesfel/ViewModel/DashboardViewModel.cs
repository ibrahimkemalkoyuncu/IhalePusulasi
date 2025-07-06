using Mesfel.Models;
using Mesfel.Services;

namespace Mesfel.ViewModel
{
    /// <summary>
    /// Dashboard ana sayfa için view model
    /// </summary>
    public class DashboardViewModel
    {
        public List<Ihale> SonIhaleler { get; set; } = new List<Ihale>();
        public List<Ihale> AktifIhaleler { get; set; } = new List<Ihale>();
        public List<IstatistikVeri> Istatistikler { get; set; } = new List<IstatistikVeri>();
    }
}
