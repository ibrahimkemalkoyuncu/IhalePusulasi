using System.ComponentModel.DataAnnotations;

namespace Mesfel.Utilities
{
    // Teklif Durumu Enum
    public enum TeklifDurumu
    {
        [Display(Name = "Verildi", Description = "Teklif verildi ama değerlendirilmedi")]
        Verildi = 1,

        [Display(Name = "Değerlendiriliyor", Description = "Teklif değerlendirme sürecinde")]
        Degerlendiriliyor = 2,

        [Display(Name = "Kabul Edildi", ShortName = "Onay")]
        KabulEdildi = 3,

        [Display(Name = "Reddedildi", ShortName = "Ret")]
        Reddedildi = 4,

        [Display(Name = "Geçersiz", Description = "Teknik nedenlerle geçersiz")]
        Gecersiz = 5,

        [Display(Name = "İptal Edildi")]
        IptalEdildi = 6
    }
}
