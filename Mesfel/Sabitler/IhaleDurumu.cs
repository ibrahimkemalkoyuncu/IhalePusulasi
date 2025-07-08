using System.ComponentModel.DataAnnotations;

namespace Mesfel.Sabitler
{
    // İhale Durumu Enum
    public enum IhaleDurumu
    {
        [Display(Name = "Planlanmakta")]
        Planlanmakta = 1,

        [Display(Name = "İlan Edildi")]
        IlanEdildi = 2,

        [Display(Name = "Teklif Alma")]
        TeklifAlma = 3,

        [Display(Name = "Değerlendirme")]
        Degerlendirme = 4,

        [Display(Name = "Sonuçlandı")]
        Sonuclandi = 5,

        [Display(Name = "İptal Edildi")]
        IptalEdildi = 6
    }
}
