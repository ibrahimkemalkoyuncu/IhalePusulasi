using Mesfel.Models;
using Mesfel.Services;
using System.ComponentModel.DataAnnotations;

namespace Mesfel.ViewModel
{
    /// <summary>
    /// Optimal teklif sayfası için view model
    /// </summary>
    public class OptimalTeklifViewModel
    {
        public Ihale Ihale { get; set; } = new Ihale();

        [Required(ErrorMessage = "Hedef kar oranı zorunludur")]
        [Range(0, 100, ErrorMessage = "Kar oranı 0-100 arasında olmalıdır")]
        [Display(Name = "Hedef Kar Oranı (%)")]
        public decimal HedefKarOrani { get; set; }

        public OptimalTeklifSonucu? OptimalSonuc { get; set; }
        public KesinTeminatHesapla? KesinTeminat { get; set; }
    }
}
