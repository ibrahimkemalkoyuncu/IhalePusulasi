using System.ComponentModel.DataAnnotations;

namespace Mesfel.Sabitler
{
    // Teklif Durumu Enum
    public enum TeklifDurumu
    {
        [Display(Name = "Verildi")]
        Verildi = 1,
        [Display(Name = "Değerlendiriliyor")]
        Degerlendiriliyor = 2,
        [Display(Name = "Kabul Edildi")]
        KabulEdildi = 3,
        [Display(Name = "Reddedildi")]
        Reddedildi = 4,
        [Display(Name = "Geçersiz")]
        Gecersiz = 5
    }
}
